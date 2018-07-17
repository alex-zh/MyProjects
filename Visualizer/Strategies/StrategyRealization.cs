using System;
using System.Collections.Generic;
using Common.Classes;

namespace Strategies
{
    public class StrategyRealization
    {
        public StrategyRealization()
        {
            Offsets = new List<double>();
        }

        public Candle StartCandle { get; set; }

        public Candle CenterConditionCandle { get; set; }

        public List<Double> Offsets { get; private set; }
    }
}
