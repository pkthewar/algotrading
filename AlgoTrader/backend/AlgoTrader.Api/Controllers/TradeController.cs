using AlgoTrader.Core.Models;
using AlgoTrader.Trading.Brokers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlgoTrader.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradeController(ITradingBroker tradingBroker) : ControllerBase
    {
        private readonly ITradingBroker tradingBroker = tradingBroker;

        [HttpPost("execute")]
        public async Task<IActionResult> Execute([FromBody] TradeRequest tradeRequest)
        {
            TradeResult result = await tradingBroker.PlaceOrderAsync(tradeRequest);

            return Ok(result);
        }

        [HttpGet("GetPortfolio")]
        public IActionResult GetPortfolio()
        {
            PortfolioSnapshot snapshot = tradingBroker.GetPortfolio();

            return Ok(snapshot);
        }
    }
}
