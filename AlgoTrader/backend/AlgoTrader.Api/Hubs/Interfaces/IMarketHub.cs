namespace AlgoTrader.Api.Hubs.Interfaces
{
    public interface IMarketHub
    {
        Task Subscribe(string symbol);

        Task Unsubscribe(string symbol);

        Task BroadcastTick(string symbol, double price, DateTime time);

        Task BroadcastTrade(object trade);

        Task BroadcastPortfolio(object portfolio);

        /// <summary>
        /// Broadcast the kill switch activation
        /// </summary>
        /// <returns>Task that represents an asynchronous operation of broadcasting the kill switch activation.</returns>
        Task BroadcastKillSwitch();
    }
}
