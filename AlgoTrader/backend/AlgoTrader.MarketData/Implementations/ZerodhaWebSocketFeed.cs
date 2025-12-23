using AlgoTrader.Core.Models;
using AlgoTrader.MarketData.Interfaces;
using KiteConnect;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace AlgoTrader.MarketData.Implementations
{
    public class ZerodhaWebSocketFeed : IMarketDataFeed
    {
        private readonly Kite kite;
        private readonly ClientWebSocket webSocket = new();
        private readonly ConcurrentQueue<MarketTick> ticks = new();

        private readonly Dictionary<uint, string> tokenToSymbol = [];
        private readonly Uri wsUri;

        private readonly Ticker socket;

        public bool IsConnected => socket?.IsConnected == true;

        public ZerodhaWebSocketFeed(IConfiguration configuration)
        {
            string? apiKey = configuration["Zerodha:ApiKey"];
            string? accessToken = configuration["Zerodha:AccessToken"];

            kite = new Kite(apiKey);
            kite.SetAccessToken(accessToken);

            wsUri = new Uri($"wss://ws.kite.trade?api_key={apiKey}&access_token={accessToken}");

            socket = new Ticker(apiKey, accessToken);

            LoadInstrumentTokens();
            Connect();
        }

        private void LoadInstrumentTokens()
        {
            List<Instrument> instruments = kite.GetInstruments("NSE");

            MapIndex(instruments, "NIFTY 50", "NIFTY");
            MapIndex(instruments, "NIFTY BANK", "BANKNIFTY");
            MapIndex(instruments, "SENSEX", "SENSEX");
        }

        private void MapIndex(List<Instrument> instruments, string tradingSymbol, string alias)
        {
            Instrument inst = instruments.FirstOrDefault(i => i.TradingSymbol.Equals(tradingSymbol, StringComparison.OrdinalIgnoreCase));

            if (inst.InstrumentToken == 0)
                throw new Exception($"Instrument not found: {tradingSymbol}");

            tokenToSymbol[inst.InstrumentToken] = alias;
        }

        private async void Connect()
        {
            await webSocket.ConnectAsync(wsUri, CancellationToken.None);

            Subscribe();

            _ = ReceiveLoop();
        }

        private async void Subscribe()
        {
            uint[] tokens = [.. tokenToSymbol.Keys];
            ArraySegment<byte> payload = BuildSubscribePayload(tokens);

            await webSocket.SendAsync(payload, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private static ArraySegment<byte> BuildSubscribePayload(uint[] tokens)
        {
            string json = $$"""
                {
                    "a": "subscribe",
                    "v": [{{string.Join(",", tokens)}}]
                }
                """;

            return new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
        }

        private async Task ReceiveLoop()
        {
            byte[] buffer = new byte[8192];

            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Binary)
                    ParseBinaryTick(buffer[..result.Count]);
            }
        }

        private void ParseBinaryTick(byte[] data)
        {
            if (data.Length < 8)
                return;

            uint token = BitConverter.ToUInt32(data, 0);
            double price = BitConverter.ToInt32(data, 4) / 100.0;

            if (tokenToSymbol.TryGetValue(token, out string? symbol))
                ticks.Enqueue(new MarketTick(symbol, DateTime.UtcNow, price));
        }

        public async IAsyncEnumerable<MarketTick> Stream(string symbol, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (ticks.TryDequeue(out MarketTick? tick) && tick.Symbol == symbol)
                    yield return tick;
                else
                    await Task.Delay(10, cancellationToken);
            }
        }
    }
}
