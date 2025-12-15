namespace AlgoTrader.Core.Models
{
    public record PortfolioSnapshot(double Cash, IDictionary<string, PositionSnapshot> Positions);
}
