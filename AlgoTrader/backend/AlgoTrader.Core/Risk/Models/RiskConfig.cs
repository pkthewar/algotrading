namespace AlgoTrader.Core.Risk.Models
{
    public sealed class RiskConfig
    {
        public double MaxDailyLoss { get; set; }

        public int MaxQuantityPerTrade { get; set; }
    }
}
