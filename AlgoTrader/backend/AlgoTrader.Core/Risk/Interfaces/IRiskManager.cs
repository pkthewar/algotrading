using AlgoTrader.Core.Models;

namespace AlgoTrader.Core.Risk.Interfaces
{
    public interface IRiskManager
    {
        bool CanTrade(TradeRequest tradeRequest);

        void RecordTrade(TradeResult tradeResult);
    }
}
