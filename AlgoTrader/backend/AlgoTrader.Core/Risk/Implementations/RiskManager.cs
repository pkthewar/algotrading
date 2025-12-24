using AlgoTrader.Core.Models;
using AlgoTrader.Core.Risk.Interfaces;
using AlgoTrader.Core.Risk.Models;

namespace AlgoTrader.Core.Risk.Implementations
{
    public class RiskManager : IRiskManager
    {
        private double dailyPnL;

        private const double max_daily_loss = -50000;

        private RiskConfig config = new();

        private double dailyLoss;

        public void UpdateConfig(RiskConfig config)
        {
            this.config = config;
        }

        public bool CanTrade(TradeRequest tradeRequest) => dailyPnL <= -config.MaxDailyLoss && tradeRequest.Quantity <= config.MaxQuantityPerTrade;

        /// <summary>
        /// Record the trade committed by updating the daily Profit & Loss.
        /// </summary>
        /// <param name="tradeResult">TradeResult object containing the result of trade execution</param>
        public void RecordTrade(TradeResult tradeResult) => dailyPnL += dailyLoss;
    }
}
