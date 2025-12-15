using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoTrader.Core.Models
{
    public record TradeResult(bool Success, double FillPrice, DateTime Time, double Notional);
}
