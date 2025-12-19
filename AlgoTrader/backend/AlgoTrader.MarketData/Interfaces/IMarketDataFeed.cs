using AlgoTrader.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoTrader.MarketData.Interfaces
{
    public interface IMarketDataFeed
    {
        IAsyncEnumerable<MarketTick> Stream(string symbol, CancellationToken cancellationToken = default);
    }
}
