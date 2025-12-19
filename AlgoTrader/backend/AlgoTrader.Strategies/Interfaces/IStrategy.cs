using AlgoTrader.Core.Models;

namespace AlgoTrader.Strategies.Interfaces
{
    public interface IStrategy
    {
        string Name { get; }

        Task<TradeActionDecision?> EvaluateAsync(MarketTick tick);
    }
}
