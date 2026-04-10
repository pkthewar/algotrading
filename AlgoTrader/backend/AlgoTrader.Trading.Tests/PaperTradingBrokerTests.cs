using System;
using Xunit;
using AlgoTrader.Trading.Brokers.Implementations;
using AlgoTrader.Core.Models;

namespace AlgoTrader.Trading.Tests
{
    public class PaperTradingBrokerTests
    {
        [Fact]
        public void Buy_and_Sell_Workflow()
        {
            var broker = new PaperTradingBroker();

            broker.Initialize(10000);
            broker.SetRisk(5000, 5000);

            var buyReq = new TradeRequest("ABC", 10, TradeAction.Buy, 100);

            var buyResult = broker.PlaceOrderAsync(buyReq).GetAwaiter().GetResult();

            Assert.True(buyResult.Success);

            var sellReq = new TradeRequest("ABC", 5, TradeAction.Sell, 110);

            var sellResult = broker.PlaceOrderAsync(sellReq).GetAwaiter().GetResult();

            Assert.True(sellResult.Success);
            Assert.True(sellResult.PnL > 0);
        }
    }
}
