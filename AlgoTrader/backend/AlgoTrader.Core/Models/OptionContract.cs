namespace AlgoTrader.Core.Models
{
    public class OptionContract
    {
        public string Symbol { get; set; } = string.Empty;

        public DateTime Expiry { get; set; }

        public decimal Strike { get; set; }

        //public OptionType Type { get; set; } TO-DO: Work is pending on OptionType enum.

        public decimal LastPrice { get; set; }
    }
}
