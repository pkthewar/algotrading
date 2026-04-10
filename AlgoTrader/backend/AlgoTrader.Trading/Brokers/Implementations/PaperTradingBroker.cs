using AlgoTrader.Core.Models;
using AlgoTrader.MarketData.Interfaces;
using AlgoTrader.Trading.Brokers.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;
// no direct dependency on API/SignalR to avoid circular references

namespace AlgoTrader.Trading.Brokers.Implementations
{
    public class PaperTradingBroker : ITradingBroker
    {
        private readonly object lockObj = new();

        private readonly IMarketDataFeed marketDataFeed;
        private readonly AlgoTrader.Core.Models.IEventPublisher? eventPublisher;
        private decimal cash;
        private decimal startingCash;

        private decimal maxLoss;
        private decimal maxPositionValue;

        private readonly List<Position> positionsList = new();

        private readonly ConcurrentDictionary<string, int> positions = new();
        private readonly ConcurrentDictionary<string, decimal> avgPrice = new();

        public PaperTradingBroker(IMarketDataFeed marketDataFeed, AlgoTrader.Core.Models.IEventPublisher? eventPublisher = null)
        {
            this.marketDataFeed = marketDataFeed;
            this.eventPublisher = eventPublisher;

            // subscribe to feed ticks so paper broker reflects real-time prices
            this.marketDataFeed.TickReceived += OnTickReceived;
        }

        public void Initialize(double startingCash)
        {
            this.startingCash = (decimal)startingCash;
            cash = (decimal)startingCash;
            positionsList.Clear();
        }

        public void SetRisk(double maxLoss, double maxPositionValue)
        {
            this.maxLoss = (decimal)maxLoss;
            this.maxPositionValue = (decimal)maxPositionValue;
        }

        public Task<TradeResult> PlaceOrderAsync(TradeRequest tradeRequest)
        {
            return tradeRequest.TradeAction switch
            {
                TradeAction.Buy => Buy(tradeRequest),
                TradeAction.Sell => Sell(tradeRequest),
                _ => throw new NotSupportedException()
            };

            #region Commented code for now. Let's see how things take shape.
            //lock (lockObj)
            //{
            //    double cost = tradeRequest.Price * tradeRequest.Quantity;

            //    //switch (tradeRequest.TradeAction)
            //    //{
            //    //    case TradeAction.Buy:
            //    //        if (cash < cost)
            //    //            throw new InvalidOperationException("Insufficient funds");

            //    //        if (cost > maxPositionValue)
            //    //            throw new InvalidOperationException("Position size limit exceeded");

            //    //        cash -= cost;

            //    //        int prevQty = positions.GetValueOrDefault(tradeRequest.Symbol);
            //    //        double prevAvg = avgPrice.GetValueOrDefault(tradeRequest.Symbol);

            //    //        int newQty = prevQty + tradeRequest.Quantity;
            //    //        double newAvg = ((prevQty * prevAvg) + cost) / newQty;

            //    //        positions[tradeRequest.Symbol] = newQty;
            //    //        avgPrice[tradeRequest.Symbol] = newAvg;

            //    //        return Task.FromResult(new TradeResult(true, tradeRequest.Price, DateTime.UtcNow, 0));

            //    //        break;

            //    //    case TradeAction.Sell:
            //    //        int heldQty = positions.GetValueOrDefault(tradeRequest.Symbol);

            //    //        if (heldQty == 0)
            //    //            throw new InvalidOperationException("No position to sell");

            //    //        if (heldQty < tradeRequest.Quantity)
            //    //            throw new InvalidOperationException("Requested quantity is more than available sell quantity");

            //    //        positions[tradeRequest.Symbol] = heldQty - tradeRequest.Quantity;

            //    //        cash += cost;

            //    //        double realizedPnL = (tradeRequest.Price - avgPrice[tradeRequest.Symbol]) * tradeRequest.Quantity;

            //    //        CheckMaxLoss(tradeRequest.Symbol);

            //    //        return Task.FromResult(new TradeResult(tradeRequest.Symbol, tradeRequest.Quantity, tradeRequest.Price, true, DateTime.UtcNow, ))

            //    //        break;
            //    //}

            //    return Task.FromResult(new TradeResult(true, tradeRequest.Price, DateTime.UtcNow, 0));
            //} 
            #endregion
        }

        public PortfolioSnapshot GetPortfolio()
        {
            lock (lockObj)
            {
                Dictionary<string, PositionSnapshot> _positions = new Dictionary<string, PositionSnapshot>();

                foreach (KeyValuePair<string, int> kvp in positions)
                {
                    string symbol = kvp.Key;

                    int qty = kvp.Value;

                    if (qty == 0)
                        continue;

                    decimal avg = avgPrice.GetValueOrDefault(symbol);

                    // PositionSnapshot expects decimal avg after models were updated
                    _positions[symbol] = new PositionSnapshot(symbol, qty, (double)avg);
                }

                return new PortfolioSnapshot((double)cash, _positions);
            }
        }

        public Task<bool> PingAsync() => Task.FromResult(true);

        public double GetUnrealizedPnL()
        {
            lock (lockObj)
                return (double)positionsList.Sum(p => (decimal)p.UnrealizedPnL);
        }

        private void OnTickReceived(MarketTick tick)
        {
            lock (lockObj)
            {
                Position? pos = positionsList.FirstOrDefault(p => p.Symbol == tick.Symbol);

                pos?.LastPrice = tick.Price;
            }
        }

        private Task<TradeResult> Buy(TradeRequest tradeRequest)
        {
            TradeResult result;

            lock (lockObj)
            {
                decimal cost = tradeRequest.Quantity * (decimal)tradeRequest.Price;

                if (cash < cost)
                    throw new InvalidOperationException("Insufficient Funds");

                if (cost > maxPositionValue)
                    throw new InvalidOperationException("Position size limit exceeded");

                cash -= cost;

                Position? position = positionsList.FirstOrDefault(p => p.Symbol == tradeRequest.Symbol);

                if (position is null)
                {
                    position = new()
                    {
                        Symbol = tradeRequest.Symbol,
                        Quantity = tradeRequest.Quantity,
                        AvgPrice = tradeRequest.Price,
                        LastPrice = tradeRequest.Price
                    };

                    positionsList.Add(position);
                }
                else
                {
                    int totalQty = position.Quantity + tradeRequest.Quantity;

                    // update avg price using decimal math
                    decimal prevAvg = (decimal)position.AvgPrice;
                    decimal newAvg = ((prevAvg * position.Quantity) + cost) / totalQty;

                    position.AvgPrice = (double)newAvg;

                    position.Quantity = totalQty;
                }

                positions.AddOrUpdate(tradeRequest.Symbol, position.Quantity, (_, __) => position.Quantity);
                avgPrice.AddOrUpdate(tradeRequest.Symbol, (decimal)position.AvgPrice, (_, __) => (decimal)position.AvgPrice);

                Debug.WriteLine($"[PaperTradingBroker] Buy executed: {tradeRequest.Symbol} qty={tradeRequest.Quantity} price={tradeRequest.Price}");

                result = new TradeResult(tradeRequest.Symbol, tradeRequest.Quantity, tradeRequest.Price, true, DateTime.UtcNow, 0.0);
            }

            // broadcasting is handled by the API layer (MarketHub); skip here
            Debug.WriteLine("[PaperTradingBroker] Broadcast skipped (handled by API layer)");

            return Task.FromResult(result);
        }

        private Task<TradeResult> Sell(TradeRequest tradeRequest)
        {
            TradeResult result;

            lock (lockObj)
            {
                Position position = positionsList.FirstOrDefault(p => p.Symbol == tradeRequest.Symbol) ?? throw new InvalidOperationException("No positions to sell");

                if (position.Quantity < tradeRequest.Quantity)
                    throw new InvalidOperationException("Available quantity is lesser than requested quantity");

                decimal realizedPnL = ((decimal)tradeRequest.Price - (decimal)position.AvgPrice) * tradeRequest.Quantity;

                position.Quantity -= tradeRequest.Quantity;

                cash += (decimal)tradeRequest.Price * tradeRequest.Quantity;

                if (position.Quantity == 0)
                {
                    positionsList.Remove(position);

                    positions.TryRemove(tradeRequest.Symbol, out _);

                    avgPrice.TryRemove(tradeRequest.Symbol, out _);
                }
                else
                {
                    positions.AddOrUpdate(tradeRequest.Symbol, position.Quantity, (_, __) => position.Quantity);

                    avgPrice.AddOrUpdate(tradeRequest.Symbol, (decimal)position.AvgPrice, (_, __) => (decimal)position.AvgPrice);
                }

                CheckMaxLoss();

                Debug.WriteLine($"[PaperTradingBroker] Sell executed: {tradeRequest.Symbol} qty={tradeRequest.Quantity} price={tradeRequest.Price} realizedPnL={(double)realizedPnL}");

                result = new TradeResult(tradeRequest.Symbol, tradeRequest.Quantity, tradeRequest.Price, true, DateTime.UtcNow, (double)realizedPnL);
            }

            // broadcasting is handled by the API layer (MarketHub); skip here
            Debug.WriteLine("[PaperTradingBroker] Broadcast skipped (handled by API layer)");

            return Task.FromResult(result);
        }

        private void CheckMaxLoss()
        {
            decimal totalPnL = positionsList.Sum(p => (decimal)p.UnrealizedPnL);

            if (totalPnL <= -maxLoss)
                throw new InvalidOperationException("Max loss breached");
        }
    }
}
