using AlgoTrader.Core.Models;
using AlgoTrader.Strategies.Interfaces;

namespace AlgoTrader.Strategies.Implementations
{
    public class ShortStraddleStrategy : IStrategy
    {
        public string Name => "Short Straddle Strategy";

        private bool executed;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<TradeActionDecision?> EvaluateAsync(MarketTick tick)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (executed)
                return null;

            double atmStrike = Math.Round(tick.Price / 100) * 100;

            executed = true;

            return new TradeActionDecision(TradeAction.Sell, $"{tick.Symbol}{atmStrike}CE+PE", 50, 0, "ATM Short Straddle");
        }
    }
}
