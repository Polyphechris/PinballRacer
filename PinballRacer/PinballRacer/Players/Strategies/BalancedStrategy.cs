using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinballRacer.Players.Strategies
{
    class BalancedStrategy : Strategy
    {
        public BalancedStrategy()
        {
            base.Initialize();
            Initialize();
        }

        protected virtual void Initialize()
        {
            points_value = 1f;
            distance_value = 0.6f;
        }
    }
}
