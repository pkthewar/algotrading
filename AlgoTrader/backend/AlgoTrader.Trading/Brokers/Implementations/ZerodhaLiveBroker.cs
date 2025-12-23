using AlgoTrader.Core.Models;
using AlgoTrader.Trading.Brokers.Interfaces;
using KiteConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoTrader.Trading.Brokers.Implementations
{
    public class ZerodhaLiveBroker : ITradingBroker
    {
        private readonly Kite kiteClient;

        public ZerodhaLiveBroker(string apiKey, string accessToken)
        {
            kiteClient = new Kite(apiKey, accessToken);
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

                return await Task.FromResult(!string.IsNullOrEmpty(profile.UserName));
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
