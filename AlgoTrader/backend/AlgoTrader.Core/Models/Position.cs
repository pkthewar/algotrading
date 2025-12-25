namespace AlgoTrader.Core.Models
{
    public class Position
    {
        public string? Symbol { get; set; }

        public int Quantity { get; set; }

        public double AvgPrice { get; set; }

        public double LastPrice { get; set; }

        public double UnrealizedPnL => (LastPrice - AvgPrice) * Quantity;
    }
}