using AlgoTrader.Core.Models;
using AlgoTrader.Trading.Brokers.Interfaces;
using System.Collections.Concurrent;

namespace AlgoTrader.Trading.Brokers.Implementations
{
    public class PaperTradingBroker : ITradingBroker
    {
        private readonly object lockObj = new();

        private double cash;

        private readonly ConcurrentDictionary<string, int> positions;
        private readonly ConcurrentDictionary<string, double> avgPrice;

        public PaperTradingBroker()
        {
            cash = 1_00_000;
            positions = new ConcurrentDictionary<string, int>();
            avgPrice = new ConcurrentDictionary<string, double>();
        }

        public Task<TradeResult> PlaceOrderAsync(TradeRequest tradeRequest)
        {
            lock (lockObj)
            {
                double cost = tradeRequest.Price * tradeRequest.Quantity;

                switch (tradeRequest.TradeAction)
                {
                    case TradeAction.Buy:
                        if (cash < cost)
                            return Task.FromResult(new TradeResult(false, tradeRequest.Price, DateTime.UtcNow, 0));

                        cash -= cost;

                        int prevQty = positions.GetValueOrDefault(tradeRequest.Symbol);
                        double prevAvg = avgPrice.GetValueOrDefault(tradeRequest.Symbol);

                        int newQty = prevQty + tradeRequest.Quantity;
                        double newAvg = ((prevQty * prevAvg) + cost) / newQty;

                        positions[tradeRequest.Symbol] = newQty;
                        avgPrice[tradeRequest.Symbol] = newAvg;
                        break;

                    case TradeAction.Sell:
                        int heldQty = positions.GetValueOrDefault(tradeRequest.Symbol);

                        if (heldQty < tradeRequest.Quantity)
                            return Task.FromResult(new TradeResult(false, tradeRequest.Price, DateTime.UtcNow, 0));

                        positions[tradeRequest.Symbol] = heldQty - tradeRequest.Quantity;

                        cash += cost;

                        break;
                }

                return Task.FromResult(new TradeResult(true, tradeRequest.Price, DateTime.UtcNow, cost));
            }
        }

        public PortfolioSnapshot GetPortfolio()
        {
            lock (lockObj)
            {
                IDictionary<string, PositionSnapshot> _positions = new Dictionary<string, PositionSnapshot>();

                foreach (KeyValuePair<string, int> kvp in positions)
                {
                    string symbol = kvp.Key;

                    int qty = kvp.Value;

                    if (qty == 0)
                        continue;

                    double avg = avgPrice.GetValueOrDefault(symbol);

                    _positions[symbol] = new PositionSnapshot(symbol, qty, avg);
                }

                return new PortfolioSnapshot(cash, _positions);
            }
        }

        public Task<bool> PingAsync()
        {
            throw new NotImplementedException();
        }
    }
}
