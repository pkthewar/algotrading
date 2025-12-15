using AlgoTrader.Core.Models;
using AlgoTrader.Core.Strategies.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoTrader.Core.Strategies.Implementations
{
    public class RSIStrategy : IStrategy
    {
        private readonly Queue<double> prices = new();

        public TradeAction OnTick(MarketTick tick)
        {
            prices.Enqueue(tick.Price);

            if (prices.Count < 14) return TradeAction.Hold;

            double gains = 0.0;
            double losses = 0.0;

            double[] arr = prices.ToArray();

            for (int i = 1; i < arr.Length; i++)
            {
                double diff = arr[i] - arr[i - 1];

                if (diff > 0)
                    gains += diff;
                else
                    losses -= diff;
            }

            double rs = gains / Math.Max(losses, 1);

            double rsi = 100 - (100 / (1 + rs));

            return rsi < 30 ? TradeAction.Buy : rsi > 70 ? TradeAction.Sell : TradeAction.Hold;
        }
    }
}
