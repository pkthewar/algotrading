using AlgoTrader.Core.Models;
using AlgoTrader.Trading.Brokers.Interfaces;
using KiteConnect;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Reflection;

namespace AlgoTrader.Trading.Brokers.Implementations
{
    public class ZerodhaLiveBroker : ITradingBroker
    {
        private readonly Kite kiteClient;
        private readonly ILogger<ZerodhaLiveBroker> logger;

        public ZerodhaLiveBroker(string apiKey, string accessToken, ILogger<ZerodhaLiveBroker> logger)
        {
            this.logger = logger;

            kiteClient = new Kite(apiKey);
            kiteClient.SetAccessToken(accessToken);
        }

        private static IEnumerable ToEnumerable(object? obj)
        {
            if (obj == null)
                return Array.Empty<object>();

            if (obj is IEnumerable e)
                return e;

            return new object[] { obj };
        }

        private static string GetStringProp(object src, params string[] names)
        {
            foreach (var n in names)
            {
                var pi = src.GetType().GetProperty(n, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    var v = pi.GetValue(src);
                    if (v != null)
                        return v.ToString() ?? string.Empty;
                }
            }

            return string.Empty;
        }

        private static int GetIntProp(object src, params string[] names)
        {
            foreach (var n in names)
            {
                var pi = src.GetType().GetProperty(n, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    var v = pi.GetValue(src);
                    if (v is int i) return i;
                    if (v is long l) return (int)l;
                    if (v is decimal d) return (int)d;
                    if (v is double db) return (int)db;
                    if (v != null && int.TryParse(v.ToString(), out int parsed)) return parsed;
                }
            }

            return 0;
        }

        private static double GetDoubleProp(object src, params string[] names)
        {
            foreach (var n in names)
            {
                var pi = src.GetType().GetProperty(n, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    var v = pi.GetValue(src);
                    if (v is double db) return db;
                    if (v is float f) return f;
                    if (v is decimal d) return (double)d;
                    if (v is int i) return i;
                    if (v != null && double.TryParse(v.ToString(), out double parsed)) return parsed;
                }
            }

            return 0.0;
        }

        public PortfolioSnapshot GetPortfolio()
        {
            // Map holdings/positions from kiteClient to PortfolioSnapshot
            try
            {
                var holdings = kiteClient.GetHoldings();
                var positions = kiteClient.GetPositions();

                Dictionary<string, PositionSnapshot> map = new();

                // Map holdings as positions with avg price from holdings
                foreach (object h in ToEnumerable(holdings))
                {
                    string sym = GetStringProp(h, "tradingSymbol", "TradingSymbol", "tradingsymbol");
                    int qty = GetIntProp(h, "quantity", "Quantity", "qty");
                    double avg = GetDoubleProp(h, "averagePrice", "AveragePrice", "avgPrice");

                    if (!string.IsNullOrEmpty(sym))
                        map[sym] = new PositionSnapshot(sym, qty, avg);
                }

                // For positions API entries, ensure last price and quantities are reflected
                foreach (object p in ToEnumerable(positions))
                {
                    string sym = GetStringProp(p, "tradingSymbol", "TradingSymbol", "tradingsymbol");
                    int qty = GetIntProp(p, "quantity", "Quantity", "qty");
                    double avg = GetDoubleProp(p, "averagePrice", "AveragePrice", "avgPrice");

                    if (!string.IsNullOrEmpty(sym))
                        map[sym] = new PositionSnapshot(sym, qty, avg);
                }

                // Cash is not directly available via these calls; return 0.0 as placeholder
                return new PortfolioSnapshot(0.0, map);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetPortfolio failed");
                return new PortfolioSnapshot(0.0, new Dictionary<string, PositionSnapshot>());
            }
        }

        public async Task<bool> PingAsync()
        {
            try
            {
                Profile profile = kiteClient.GetProfile();

                return !string.IsNullOrEmpty(profile.UserName) ? await Task.FromResult(true) : await Task.FromResult(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ping failed");
                return await Task.FromResult(false);
            }
        }

        public Task<TradeResult> PlaceOrderAsync(TradeRequest tradeRequest)
        {
            // Map TradeRequest to Kite.PlaceOrder parameters and call the API
            try
            {
                // Kite.PlaceOrder signature (from KiteConnect) is long; call using named parameters for clarity
                // We'll call the synchronous PlaceOrder method and map the returned order id / status

                // For market orders: order_type = "MARKET" and price-related params are null
                string tradingsymbol = tradeRequest.Symbol;
                string exchange = "NSE";
                string transactionType = tradeRequest.TradeAction == TradeAction.Buy ? "BUY" : "SELL";
                int quantity = tradeRequest.Quantity;
                string orderType = "MARKET";
                string product = "MIS";

                // Call Kite.PlaceOrder with the common overload. Use defaults for optional params.
                // Note: If the KiteConnect version in the project exposes different overloads, update accordingly.
                var orderId = kiteClient.PlaceOrder(tradingsymbol, exchange, transactionType, quantity, null, orderType, product, null, null, null, null, null, null, null, null, null, null);

                logger.LogInformation("Placed order via Kite: {Symbol} {Qty} {Side} => {OrderId}", tradingsymbol, quantity, transactionType, orderId);

                return Task.FromResult(new TradeResult(tradeRequest.Symbol, tradeRequest.Quantity, tradeRequest.Price, true, DateTime.UtcNow, 0.0));
            }
            catch (TargetInvocationException tie)
            {
                logger.LogError(tie, "Order placement failed (invocation) for {Symbol}", tradeRequest.Symbol);
                return Task.FromResult(new TradeResult(tradeRequest.Symbol, tradeRequest.Quantity, tradeRequest.Price, false, DateTime.UtcNow, 0.0));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Order placement failed for {Symbol}", tradeRequest.Symbol);
                return Task.FromResult(new TradeResult(tradeRequest.Symbol, tradeRequest.Quantity, tradeRequest.Price, false, DateTime.UtcNow, 0.0));
            }
        }

        public double GetUnrealizedPnL()
        {
            try
            {
                var positions = kiteClient.GetPositions();

                double total = 0.0;

                foreach (object p in ToEnumerable(positions))
                {
                    double lastPrice = GetDoubleProp(p, "lastPrice", "LastPrice", "ltp");
                    double avg = GetDoubleProp(p, "averagePrice", "AveragePrice", "avgPrice");
                    int qty = GetIntProp(p, "quantity", "Quantity", "qty");

                    total += (lastPrice - avg) * qty;
                }

                return total;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUnrealizedPnL failed");
                return 0.0;
            }
        }
    }
}
