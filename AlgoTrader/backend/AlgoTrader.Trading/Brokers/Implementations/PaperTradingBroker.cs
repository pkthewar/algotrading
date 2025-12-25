using AlgoTrader.Core.Models;
using AlgoTrader.Trading.Brokers.Interfaces;
using System.Collections.Concurrent;

namespace AlgoTrader.Trading.Brokers.Implementations
{
    public class PaperTradingBroker : ITradingBroker
    {
        private readonly object lockObj = new();

        private double cash;
        private double startingCash;

        private double maxLoss;
        private double maxPositionValue;

        private readonly List<Position> positionsList = [];

        private readonly ConcurrentDictionary<string, int> positions = new();
        private readonly ConcurrentDictionary<string, double> avgPrice = new();

        public void Initialize(double startingCash)
        {
            this.startingCash = startingCash;
            cash = startingCash;
            positionsList.Clear();
        }

        public void SetRisk(double maxLoss, double maxPositionValue)
        {
            this.maxLoss = maxLoss;
            this.maxPositionValue = maxPositionValue;
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

        public Task<bool> PingAsync() => Task.FromResult(true);

        private Task<TradeResult> Buy(TradeRequest tradeRequest)
        {
            double cost = tradeRequest.Quantity * tradeRequest.Price;

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

                position.AvgPrice = ((position.AvgPrice * position.Quantity) + cost) / totalQty;

                position.Quantity = totalQty;
            }

            positions[tradeRequest.Symbol] = position.Quantity;
            avgPrice[tradeRequest.Symbol] = position.AvgPrice;

            return Task.FromResult(new TradeResult(tradeRequest.Symbol, tradeRequest.Quantity, tradeRequest.Price, true, DateTime.UtcNow, 0));
        }

        private Task<TradeResult> Sell(TradeRequest tradeRequest)
        {
            Position position = positionsList.FirstOrDefault(p => p.Symbol == tradeRequest.Symbol) ?? throw new InvalidOperationException("No positions to sell");

            if (position.Quantity < tradeRequest.Quantity)
                throw new InvalidOperationException("Available quantity is lesser than requested quantity");

            double realizedPnL = (tradeRequest.Price - position.AvgPrice) * tradeRequest.Quantity;

            position.Quantity -= tradeRequest.Quantity;

            cash += tradeRequest.Price * tradeRequest.Quantity;

            if (position.Quantity == 0)
            {
                positionsList.Remove(position);

                positions.Remove(tradeRequest.Symbol, out int quantity);

                avgPrice.Remove(tradeRequest.Symbol, out double price);
            }
            else
            {
                positions[tradeRequest.Symbol] = position.Quantity;

                avgPrice[tradeRequest.Symbol] = position.AvgPrice;
            }

            CheckMaxLoss();


            return Task.FromResult(new TradeResult(tradeRequest.Symbol, tradeRequest.Quantity, tradeRequest.Price, true, DateTime.UtcNow, realizedPnL));
        }

        private void CheckMaxLoss()
        {
            double totalPnL = positionsList.Sum(p => p.UnrealizedPnL);

            if (totalPnL <= -maxLoss)
                throw new InvalidOperationException("Max loss breached");
        }
    }
}
