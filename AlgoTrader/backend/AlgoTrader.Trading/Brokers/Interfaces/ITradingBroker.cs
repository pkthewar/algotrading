using AlgoTrader.Core.Models;

namespace AlgoTrader.Trading.Brokers.Interfaces
{
    public interface ITradingBroker
    {
        Task<TradeResult> PlaceOrderAsync(TradeRequest tradeRequest);

        PortfolioSnapshot GetPortfolio();

        /// <summary>
        /// Returns total unrealized PnL across all positions (based on last prices received from feed).
        /// </summary>
        double GetUnrealizedPnL();

        Task<bool> PingAsync();
    }
}
