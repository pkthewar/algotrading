using AlgoTrader.Core.Models;
using AlgoTrader.Core.Risk.Interfaces;
using AlgoTrader.Engine.Interfaces;
using AlgoTrader.Trading.Brokers.Interfaces;

namespace AlgoTrader.Engine.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tradingBroker"></param>
    /// <param name="riskManager"></param>
    public class StrategyExecutionEngine(ITradingBroker tradingBroker, IRiskManager riskManager) : IStrategyExecutionEngine
    {
        public bool IsRunning => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="decision"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(TradeActionDecision decision)
        {
            TradeRequest tradeRequest = new(decision.Symbol, decision.Quantity, decision.TradeAction, decision.Price);

            if (!riskManager.CanTrade(tradeRequest))
                return;

            await tradingBroker.PlaceOrderAsync(tradeRequest);
        }
    }
}
