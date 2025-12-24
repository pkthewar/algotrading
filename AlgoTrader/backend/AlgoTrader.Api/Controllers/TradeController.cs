using AlgoTrader.Core.Models;
using AlgoTrader.Engine.Interfaces;
using AlgoTrader.Trading.Brokers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlgoTrader.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradeController(IStrategyExecutionEngine strategyExecutionEngine, ITradingBroker tradingBroker) : ControllerBase
    {
        private readonly IStrategyExecutionEngine strategyExecutionEngine = strategyExecutionEngine;

        private readonly ITradingBroker tradingBroker = tradingBroker;

        [HttpPost("execute")]
        public async Task<IActionResult> Execute([FromBody] TradeRequest tradeRequest)
        {
            TradeResult result = await strategyExecutionEngine.ExecuteAsync(tradeRequest);

            return Ok(result);
        }

        [HttpGet("getPortfolio")]
        public IActionResult GetPortfolio()
        {
            PortfolioSnapshot snapshot = tradingBroker.GetPortfolio();

            return Ok(snapshot);
        }
    }
}
