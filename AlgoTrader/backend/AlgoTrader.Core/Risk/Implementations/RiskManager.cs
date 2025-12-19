using AlgoTrader.Core.Models;
using AlgoTrader.Core.Risk.Interfaces;

namespace AlgoTrader.Core.Risk.Implementations
{
    public class RiskManager : IRiskManager
    {
        private double dailyPnL;

        private const double max_daily_loss = -50000;

        public bool CanTrade(TradeRequest tradeRequest) => dailyPnL > max_daily_loss && tradeRequest.Quantity <= 500;

        /// <summary>
        /// Record the trade committed by updating the daily Profit & Loss.
        /// </summary>
        /// <param name="tradeResult">TradeResult object containing the result of trade execution</param>
        public void RecordTrade(TradeResult tradeResult) => dailyPnL += tradeResult.Notional;
    }
}
