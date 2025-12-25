namespace AlgoTrader.Core.Models
{
    public record TradeResult(string Symbol, int Quantity, double FillPrice, bool Success, DateTime Time, double PnL);
}
