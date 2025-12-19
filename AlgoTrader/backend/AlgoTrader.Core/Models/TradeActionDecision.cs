namespace AlgoTrader.Core.Models
{
    public record TradeActionDecision(TradeAction TradeAction, string Symbol, int Quantity, double Price, string Reason);
}
