using AlgoTrader.Core.Models;
using AlgoTrader.Core.Risk.Interfaces;
using AlgoTrader.Engine.Interfaces;
using AlgoTrader.Trading.Brokers.Interfaces;

namespace AlgoTrader.Engine.Implementations
{
    /// <summary>
    /// Engine for execution of the trade.
    /// </summary>
    /// <param name="tradingBroker">ITradingBroker object which abstracts the logic of communication with the trading API</param>
    /// <param name="riskManager"></param>
    public class StrategyExecutionEngine(ITradingBroker tradingBroker, IRiskManager riskManager) : IStrategyExecutionEngine
    {
        //TO-DO: Integration Hub code present in AlgoTrader.Api project
        public bool IsRunning { get; private set; }

        public Task StartAsync()
        {
            IsRunning = true;
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            IsRunning = false;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Execute the trade and place the order in exchange.
        /// </summary>
        /// <param name="tradeRequest">TradeRequest object which holds the details of trade to be performed in the exchange</param>
        /// <returns>TradeResult created object denoting whether the trade happened successfully or not.</returns>
        public async Task<TradeResult> ExecuteAsync(TradeRequest tradeRequest)
        {
            if (!riskManager.CanTrade(tradeRequest))
                return new TradeResult(false, 0.0, new DateTime(), 0.0);

            TradeResult tradeResult = await tradingBroker.PlaceOrderAsync(tradeRequest);

            riskManager.RecordTrade(tradeResult);

            //
            return tradeResult!;
        }
    }
}
