using AlgoTrader.Core.Models;
using AlgoTrader.Core.Risk.Implementations;
using AlgoTrader.Core.Risk.Interfaces;
using AlgoTrader.Strategies.Implementations;
using AlgoTrader.Strategies.Interfaces;
using AlgoTrader.Trading.Brokers.Interfaces;
using AlgoTrader.MarketData.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlgoTrader.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StrategyController(ITradingBroker tradingBroker, IRiskManager riskManager, KillSwitch killSwitch, IMarketDataFeed marketDataFeed) : ControllerBase
    {
        private readonly ITradingBroker tradingBroker = tradingBroker;
        private readonly IRiskManager riskManager = riskManager;
        private readonly KillSwitch killSwitch = killSwitch;
        private readonly IMarketDataFeed marketDataFeed = marketDataFeed;

        private static CancellationTokenSource? cts;

        [HttpPost("start")]
        public IActionResult Start([FromQuery] string strategy)
        {
            if (cts != null)
                return BadRequest("Strategy already running");

            IStrategy selectedStrategy = strategy.ToLower() switch
            {
                "rsi" => new RSIStrategy(),
                "macd" => new MACDStrategy(),
                "coveredcall" => new CoveredCallStrategy(),
                "shortstraddle" => new ShortStraddleStrategy(),
                _ => throw new ArgumentException("Invalid strategy")
            };

            cts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                await foreach (MarketTick tick in marketDataFeed.Stream("NIFTY"))
                {
                    if (cts.IsCancellationRequested || killSwitch.IsTriggered)
                        break;

                    TradeActionDecision? action = await selectedStrategy.EvaluateAsync(tick);

                    if (action!.TradeAction == TradeAction.Hold)
                        continue;

                    TradeRequest request = new(tick.Symbol, 50, action!.TradeAction, tick.Price);

                    if (!riskManager.CanTrade(request))
                        continue;

                    TradeResult result = await tradingBroker.PlaceOrderAsync(request);
                    riskManager.RecordTrade(result);
                }
            });

            return Ok("Strategy created!");
        }


        [HttpPost("stop")]
        public IActionResult Stop()
        {
            cts?.Cancel();
            cts = null;

            return Ok("Strategy stopped!");
        }
    }
}
