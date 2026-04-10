namespace AlgoTrader.Core.Models
{
    public record PortfolioSnapshot(double Cash, IDictionary<string, PositionSnapshot> Positions);

    // Event publisher interface is placed here to avoid adding a new file in the Core project. This interface allows higher-level layers (API) to implement broadcasting of trades and portfolio snapshots without creating direct dependencies from Engine/Trading to API.
    public interface IEventPublisher
    {
        Task BroadcastTradeAsync(object trade);

        Task BroadcastPortfolioAsync(PortfolioSnapshot portfolio);
    }
}
