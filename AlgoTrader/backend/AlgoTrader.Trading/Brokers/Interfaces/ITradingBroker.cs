using AlgoTrader.Core.Models;

namespace AlgoTrader.Trading.Brokers.Interfaces
{
    public interface ITradingBroker
    {
        Task<TradeResult> PlaceOrderAsync(TradeRequest tradeRequest);

        PortfolioSnapshot GetPortfolio();

        Task<bool> PingAsync();
    }
}
