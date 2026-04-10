using AlgoTrader.Core.Models;

namespace AlgoTrader.MarketData.Interfaces
{
    public interface IMarketDataFeed
    {
        public bool IsConnected { get; }

        public DateTime LastReceivedAt { get; }

        // Raised when a market tick is received from the feed
        public event Action<MarketTick>? TickReceived;

        void OnConnect();

        Task OnTickAsync(string symbol, object tick);

        void OnDisconnect();

        IAsyncEnumerable<MarketTick> Stream(string symbol, CancellationToken cancellationToken = default);
    }
}
