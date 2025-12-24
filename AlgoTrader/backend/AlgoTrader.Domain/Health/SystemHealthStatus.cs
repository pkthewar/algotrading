namespace AlgoTrader.Domain.Health
{
    public class SystemHealthStatus
    {
        public string OverallStatus { get; set; } = "Unknown";
        public bool KillSwitchTriggered { get; set; }

        public bool MarketFeedConnected { get; set; }
        public bool BrokerConnected { get; set; }
        public bool StrategyEngineRunning { get; set; }

        public long SystemLatencyMs { get; set; }
        public DateTime ServerTimeUtc { get; set; }

        public List<string> Issues { get; set; } = [];
    }
}
