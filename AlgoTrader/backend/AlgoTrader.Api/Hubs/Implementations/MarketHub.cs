using AlgoTrader.Api.Hubs.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AlgoTrader.Api.Hubs.Implementations
{
    public class MarketHub : Hub, IMarketHub
    {
        /// <summary>
        /// Called when a client connects
        /// </summary>
        /// <returns>Task that represents an asynchronous operation of connecting to the stream.</returns>
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("system", "Connected to AlgoTrader market stream");

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Join a symbol-specific stream (e.g:- NIFTY, SENSEX, etc.)
        /// </summary>
        /// <param name="symbol">Symbol that represents an Index</param>
        /// <returns>Task that represents an asynchronous operation of subscription.</returns>
        public async Task Subscribe(string symbol)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, symbol);

            await Clients.Caller.SendAsync("system", $"Subscribed to {symbol}");
        }

        /// <summary>
        /// Leave a symbol-specific stream
        /// </summary>
        /// <param name="symbol">Symbol that represents an Index</param>
        /// <returns>Task that represents an asynchronous operation of unsubscription.</returns>
        public async Task Unsubscribe(string symbol)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol);

            await Clients.Caller.SendAsync("system", $"Unsubscribed from {symbol}");
        }

        /// <summary>
        /// Broadcast the live market tick
        /// </summary>
        /// <param name="symbol">Symbol that represents an Index</param>
        /// <param name="price">Price at that instant</param>
        /// <param name="time">Time instance</param>
        /// <returns>Task that represents an asynchronous operation of broadcasting the tick.</returns>
        public async Task BroadcastTick(string symbol, double price, DateTime time) => await Clients.Group(symbol).SendAsync("tick", new { symbol, price, time });

        /// <summary>
        /// Broadcast the trade execution
        /// </summary>
        /// <param name="trade">JSON object which represents the parameters of the trade</param>
        /// <returns>Task that represents an asynchronous operation of trade being braodcasted.</returns>
        public async Task BroadcastTrade(object trade) => await Clients.All.SendAsync("trade", trade);

        /// <summary>
        /// Broadcast the portfolio snapshot
        /// </summary>
        /// <param name="portfolio">JSON object which represents the portfolio snapshot</param>
        /// <returns>Task that represents an asynchronous operation of broadcasting the portfolio snapshot.</returns>
        public async Task BroadcastPortfolio(object portfolio) => await Clients.All.SendAsync("portfolio", portfolio);

        /// <summary>
        /// Broadcast the kill switch activation
        /// </summary>
        /// <returns>Task that represents an asynchronous operation of broadcasting the kill switch activation.</returns>
        public async Task BroadcastKillSwitch() => await Clients.All.SendAsync("system", "Kill Switch Activated - Trading Halted!");
    }
}

