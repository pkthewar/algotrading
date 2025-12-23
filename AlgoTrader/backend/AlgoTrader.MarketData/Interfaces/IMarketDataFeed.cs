using AlgoTrader.Core.Models;

namespace AlgoTrader.MarketData.Interfaces
{
    public interface IMarketDataFeed
    {
        public bool IsConnected { get; }

        IAsyncEnumerable<MarketTick> Stream(string symbol, CancellationToken cancellationToken = default);
    }
}
