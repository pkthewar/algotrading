using AlgoTrader.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AlgoTrader.Core.Strategies.Interfaces
{
    public interface IStrategy
    {
        TradeAction OnTick(MarketTick tick);
    }
}
