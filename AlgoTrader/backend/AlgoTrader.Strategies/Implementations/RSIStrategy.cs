using AlgoTrader.Core.Models;
using AlgoTrader.Strategies.Interfaces;

namespace AlgoTrader.Strategies.Implementations
{
    public class RSIStrategy : IStrategy
    {
        public string Name => "RSI Strategy";

        private readonly List<double> prices = [];
        private bool hasPosition;

        public async Task<TradeActionDecision?> EvaluateAsync(MarketTick tick)
        {
            prices.Add(tick.Price);

            if (prices.Count < 15)
                return null;

            double rsi = await CalculateRsi();

            if (rsi < 30 && !hasPosition)
            {
                hasPosition = true;

                return new TradeActionDecision(TradeAction.Buy, tick.Symbol, 1, tick.Price, "RSI Oversold");
            }

            if (rsi > 70 && hasPosition)
            {
                hasPosition = false;

                return new TradeActionDecision(TradeAction.Sell, tick.Symbol, 1, tick.Price, "RSI Overbrought");
            }

            return null;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<double> CalculateRsi()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            double gains = 0, losses = 0;

            for (int i = prices.Count - 14; i < prices.Count - 1; i++)
            {
                double diff = prices[i + 1] - prices[i];

                if (diff > 0) gains += diff;
                else losses -= diff;
            }

            double rs = gains / (losses == 0 ? 1 : losses);

            return 100 - (100 / 1 + rs);
        }
    }
}
