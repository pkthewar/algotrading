using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoTrader.Core.Models
{
    public class MacdConfig
    {
        public int FastPeriod { get; set; }

        public int SlowPeriod { get; set; }

        public int SignalPeriod { get; set; }

        public int MinDataPoints => SlowPeriod + SignalPeriod;
    }
}
