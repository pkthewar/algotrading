namespace AlgoTrader.Core.Models
{
    public record PositionSnapshot(string Symbol, int Quantity, double AvgPrice);
}
