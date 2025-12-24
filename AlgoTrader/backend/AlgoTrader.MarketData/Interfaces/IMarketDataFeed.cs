using AlgoTrader.Core.Models;

namespace AlgoTrader.MarketData.Interfaces
{
    public interface IMarketDataFeed
    {
        public bool IsConnected { get; }

        public DateTime LastReceivedAt { get; }

        void OnConnect();

        Task OnTickAsync(string symbol, object tick);

        void OnDisconnect();

        IAsyncEnumerable<MarketTick> Stream(string symbol, CancellationToken cancellationToken = default);
    }
}
