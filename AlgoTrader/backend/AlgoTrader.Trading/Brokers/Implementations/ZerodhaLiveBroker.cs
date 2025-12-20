using AlgoTrader.Core.Models;
using AlgoTrader.Trading.Brokers.Interfaces;

namespace AlgoTrader.Trading.Brokers.Implementations
{
    public class ZerodhaLiveBroker : ITradingBroker
    {
        public PortfolioSnapshot GetPortfolio()
        {
            throw new NotImplementedException();
        }

        public Task<bool> PingAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TradeResult> PlaceOrderAsync(TradeRequest tradeRequest)
        {
            throw new NotImplementedException();
        }
    }
}
