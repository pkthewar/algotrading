using AlgoTrader.Core.Models;
using AlgoTrader.Strategies.Interfaces;

namespace AlgoTrader.Strategies.Implementations
{
    public class CoveredCallStrategy : IStrategy
    {
        public string Name => "Covered Call Strategy";

        private bool executed;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<TradeActionDecision?> EvaluateAsync(MarketTick tick)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (executed)
                return null;

            executed = true;

            double callStrike = Math.Round(tick.Price * 1.05 / 100) * 100;

            return new TradeActionDecision(TradeAction.Buy, tick.Symbol, 1, tick.Price, "Buy underlying for Covered Call");
        }
    }
}
