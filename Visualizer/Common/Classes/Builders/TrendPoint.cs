using Common.Classes.General;

namespace Common.Classes.Builders
{
    public class TrendPoint
    {
        public Candle Candle { get; private set; }
        public double Value { get; private set; }

        public TrendPoint(Candle candle, double value)
        {
            Candle = candle;
            Value = value;
        }
    }
}