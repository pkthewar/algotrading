using AlgoTrader.Core.Models;
using AlgoTrader.Trading.Brokers.Interfaces;
using KiteConnect;

namespace AlgoTrader.Trading.Brokers.Implementations
{
    public class ZerodhaLiveBroker : ITradingBroker
    {
        private readonly Kite kiteClient;

        public ZerodhaLiveBroker(string apiKey, string accessToken)
        {
            kiteClient = new Kite(apiKey);
            kiteClient.SetAccessToken(accessToken);
        }

        public PortfolioSnapshot GetPortfolio()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PingAsync()
        {
            try
            {
                Profile profile = kiteClient.GetProfile();

                return !string.IsNullOrEmpty(profile.UserName) ? await Task.FromResult(true) : await Task.FromResult(false);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }

        public Task<TradeResult> PlaceOrderAsync(TradeRequest tradeRequest)
        {
            throw new NotImplementedException();
        }
    }
}
