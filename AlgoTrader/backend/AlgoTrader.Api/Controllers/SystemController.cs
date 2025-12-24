using AlgoTrader.Core.Risk.Implementations;
using AlgoTrader.Domain.Health;
using AlgoTrader.Engine.Interfaces;
using AlgoTrader.MarketData.Interfaces;
using AlgoTrader.Strategies.Interfaces;
using AlgoTrader.Trading.Brokers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AlgoTrader.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController(KillSwitch killSwitch, IMarketDataFeed marketDataFeed, ITradingBroker tradingBroker/*, IStrategyExecutionEngine strategyExecutionEngine*/) : ControllerBase
    {
        //private readonly KillSwitch killSwitch = killSwitch;

        [HttpPost("kill")]
        public IActionResult Kill()
        {
            killSwitch.Trigger();

            return Ok("Kill switch activated. All trading has been halted");
        }

        [HttpGet("getHealth")]
        public async Task<IActionResult> GetHealth()
        {
            SystemHealthStatus health = new()
            {
                KillSwitchTriggered = killSwitch.IsTriggered,
                ServerTimeUtc = DateTime.UtcNow,
            };

            var stopwatch = Stopwatch.StartNew();

            health.MarketFeedConnected = marketDataFeed.IsConnected;

            if (!health.MarketFeedConnected)
                health.Issues.Add("Market data feed is disconncted!");

            try
            {
                health.BrokerConnected = await tradingBroker.PingAsync();

                if (!health.BrokerConnected)
                    health.Issues.Add("Broker connectivity failed");
            }
            catch (Exception ex)
            {
                health.BrokerConnected = false;
                health.Issues.Add($"Broker ping exception: {ex.Message}");
            }

            //health.StrategyEngineRunning = strategyExecutionEngine.IsRunning; Work on this once merge conflicts are resolved.

            if (!health.StrategyEngineRunning)
                health.Issues.Add("Strategy engine not running");

            stopwatch.Stop();

            health.SystemLatencyMs = stopwatch.ElapsedMilliseconds;

            if (health.SystemLatencyMs > 1000)
                health.Issues.Add("System latency above threshold");

            if (!health.BrokerConnected || !health.MarketFeedConnected)
                health.Issues.Add("Critical dependency failure");

            health.OverallStatus = health.KillSwitchTriggered || health.Issues.Count > 0 ? "Degraded" : "Healthy";

            return Ok(health);
        }
    }
}
