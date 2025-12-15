using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoTrader.Core.Models
{
    public record TradeRequest(string Symbol, int Quantity, TradeAction TradeAction, double Price);
}
