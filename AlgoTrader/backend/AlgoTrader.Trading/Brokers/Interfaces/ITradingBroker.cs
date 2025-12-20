using AlgoTrader.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoTrader.Trading.Brokers.Interfaces
{
    public interface ITradingBroker
    {
        Task<TradeResult> PlaceOrderAsync(TradeRequest tradeRequest);

        PortfolioSnapshot GetPortfolio();

        Task<bool> PingAsync();
    }
}
