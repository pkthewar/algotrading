using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoTrader.Core.Risk.Implementations
{
    public class KillSwitch
    {
        public bool IsTriggered { get; set; }

        public void Trigger() => IsTriggered = true;
    }
}
