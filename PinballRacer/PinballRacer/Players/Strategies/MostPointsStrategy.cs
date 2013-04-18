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
            base.Initialize();
            Initialize();
        }

        protected void Initialize()
        {
            points_value = -2f;
            distance_value = 2f;
        }
    }
}
