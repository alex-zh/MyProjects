using System;

namespace Strategies
{
    [Serializable]
    public class VolatilityGroup
    {
        public int Index { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public int Count { get; set; }

        public bool IsInGroup(double currentVolatility)
        {
            return currentVolatility >= MinValue && currentVolatility < MaxValue;
        }

        public override string ToString()
        {
            return String.Format("Index: {0}; MinValue: {1}; MaxValue: {2}; Count: {3}", Index, MinValue, MaxValue,
                Count);
        }
    }
}