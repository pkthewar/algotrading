using AlgoTrader.Core.Models;

namespace AlgoTrader.Core.Risk.Interfaces
{
    public interface IRiskManager
    {
        /// <summary>
        /// Check if trade can be performed or not, based on TradeRequest.
        /// </summary>
        /// <param name="tradeRequest">TradeRequest object having the trade details</param>
        /// <returns>Boolean flag which specifies trade eligibility.</returns>
        bool CanTrade(TradeRequest tradeRequest);

        /// <summary>
        /// Record the trade committed by updating the daily Profit & Loss.
        /// </summary>
        /// <param name="tradeResult">TradeResult object containing the result of trade execution</param>
        void RecordTrade(TradeResult tradeResult);
    }
}
