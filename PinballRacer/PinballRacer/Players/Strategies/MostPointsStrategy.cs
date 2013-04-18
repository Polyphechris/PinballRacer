using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinballRacer.Players.Strategies
{
    public class MostPointsStrategy : Strategy
    {
        public MostPointsStrategy()
        {
            Initialize();
        }
        protected virtual void Initialize()
        {
            points_value = 0.0f;
            distance_value = 0.9f;
        }
    }
}
