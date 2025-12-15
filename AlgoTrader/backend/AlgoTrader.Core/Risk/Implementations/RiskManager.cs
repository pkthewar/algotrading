using AlgoTrader.Core.Models;
using AlgoTrader.Core.Risk.Interfaces;

namespace AlgoTrader.Core.Risk.Implementations
{
    public class RiskManager : IRiskManager
    {
        private double dailyPnL;

        private const double max_daily_loss = -50000;

        public bool CanTrade(TradeRequest tradeRequest) => dailyPnL > max_daily_loss && tradeRequest.Quantity <= 500;

        //To-Do: Need to refactor this method after fixing the compile time error.
        public void RecordTrade(TradeResult tradeResult)
        {
            throw new NotImplementedException();
        }

        //public void RecordTrade(TradeResult tradeResult) => dailyPnL += tradeResult.PnL;
    }
}
