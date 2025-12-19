namespace AlgoTrader.Core.Risk.Implementations
{
    public class KillSwitch
    {
        public bool IsTriggered { get; set; }

        public void Trigger() => IsTriggered = true;
    }
}
