using AlgoTrader.Core.Models;

namespace AlgoTrader.Engine.Interfaces
{
    public interface IStrategyExecutionEngine
    {
        Task ExecuteAsync(TradeActionDecision decision);
    }
}
