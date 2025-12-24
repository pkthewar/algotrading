using AlgoTrader.Core.Models;

namespace AlgoTrader.Engine.Interfaces
{
    public interface IStrategyExecutionEngine
    {
        public bool IsRunning { get; }

        Task StartAsync();

        Task StopAsync();

        Task<TradeResult> ExecuteAsync(TradeRequest tradeRequest);
    }
}
