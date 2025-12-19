using AlgoTrader.Core.Models;
using AlgoTrader.Strategies.Interfaces;

namespace AlgoTrader.Strategies.Implementations
{
    public class MACDStrategy : IStrategy
    {
        public string Name => "MACD Strategy";

        private readonly List<double> prices = [];

        private bool hasPosition;

        private const int fastPeriod = 12;
        private const int slowPeriod = 26;
        private const int signalPeriod = 9;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<TradeActionDecision?> EvaluateAsync(MarketTick tick)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            prices.Add(tick.Price);

            if (prices.Count < 35)
                return null;

            double macd = EMA(fastPeriod) - EMA(slowPeriod);

            double signal = EMA(signalPeriod);

            if (macd > signal && !hasPosition)
            {
                hasPosition = true;

                return new TradeActionDecision(TradeAction.Buy, tick.Symbol, 1, tick.Price, "MACD Bullish Crossover");
            }

            if (macd < signal && hasPosition)
            {
                hasPosition = false;

                return new TradeActionDecision(TradeAction.Sell, tick.Symbol, 1, tick.Price, "MACD Bearish Crossover");
            }

            return null;
        }

        private double EMA(int period)
        {
            double k = 2.0 / (period + 1);

            double ema = prices[^period];

            for (int i = prices.Count - period + 1; i < prices.Count; i++)
                ema = prices[i] * k + ema * (1 - k);

            return ema;
        }
    }
}