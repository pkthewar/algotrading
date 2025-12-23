using AlgoTrader.Core.Risk.Implementations;
using AlgoTrader.Engine.Interfaces;
using AlgoTrader.MarketData.Interfaces;
using AlgoTrader.Trading.Brokers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlgoTrader.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController(KillSwitch killSwitch, IMarketDataFeed marketDataFeed, ITradingBroker tradingBroker, IStrategyExecutionEngine strategyExecutionEngine) : ControllerBase
    {
        private readonly KillSwitch killSwitch = killSwitch;
        private readonly IMarketDataFeed marketDataFeed = marketDataFeed;
        private readonly ITradingBroker tradingBroker = tradingBroker;
        private readonly IStrategyExecutionEngine strategyExecutionEngine = strategyExecutionEngine;

        [HttpPost("kill")]
        public IActionResult Kill()
        {
            killSwitch.Trigger();

            return Ok("Kill switch activated. All trading has been halted");
        }

        [HttpGet("getHealth")]
        public async Task<IActionResult> GetHealth()
        {
            List<string> issues = [];

            if (killSwitch.IsTriggered)
                issues.Add("Kill switch triggered");

            if (!marketDataFeed.IsConnected)
                issues.Add("Market feed is disconnected");

            if (!await tradingBroker.PingAsync())
                issues.Add("Broker not reachable");

            return Ok(new
            {
                Status = issues.Count > 0 ? "Unhealthy" : "Healthy",
                KillSwitch = killSwitch.IsTriggered,
                MarketFeed = marketDataFeed.IsConnected,
                Broker = await tradingBroker.PingAsync(),
                StrategyEngine = strategyExecutionEngine, //Check for IsRunning property
                Issues = issues,
                ServerTimeUtc = DateTime.UtcNow
            });

            //return Ok(new { Status = "Running", KillSwitch = killSwitch.IsTriggered ? "Triggered" : "Active" });
        }
    }
}
