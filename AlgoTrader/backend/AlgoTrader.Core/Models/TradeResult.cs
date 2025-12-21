namespace AlgoTrader.Core.Models
{
    public record TradeResult(bool Success, double FillPrice, DateTime Time, double Notional);
}
