using AlgoTrader.Core.Models;

namespace AlgoTrader.Core.Risk.Interfaces
{
    public interface IRiskManager
    {
        bool CanTrade(TradeRequest tradeRequest);

        /// <summary>
        /// Record the trade committed by updating the daily Profit & Loss.
        /// </summary>
        /// <param name="tradeResult">TradeResult object containing the result of trade execution</param>
        void RecordTrade(TradeResult tradeResult);
    }
}
