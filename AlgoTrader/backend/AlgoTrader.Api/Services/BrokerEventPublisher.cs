using AlgoTrader.Api.Hubs.Implementations;
using AlgoTrader.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace AlgoTrader.Api.Services
{
    public class BrokerEventPublisher : IEventPublisher
    {
        private readonly IHubContext<MarketHub> hubContext;

        public BrokerEventPublisher(IHubContext<MarketHub> hubContext) => this.hubContext = hubContext;

        public Task BroadcastTradeAsync(object trade) => hubContext.Clients.All.SendAsync("trade", trade);

        public Task BroadcastPortfolioAsync(PortfolioSnapshot portfolio) => hubContext.Clients.All.SendAsync("portfolio", portfolio);
    }
}
