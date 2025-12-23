namespace AlgoTrader.Core.Models
{
    public record TradeRequest(string Symbol, int Quantity, TradeAction TradeAction, double Price);
}
