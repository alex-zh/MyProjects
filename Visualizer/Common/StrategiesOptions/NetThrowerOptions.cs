using Common.Classes;

namespace Common.StrategiesOptions
{
    public class NetThrowerOptions: IStrategyOptions
    {
        public NetThrowerOptions()
        {
            PositionSize = 1;
            MaxPositionSize = 3;
            MinPositionSize = -3;
            PercentOrdersOffset = 0.002; // 0.2%
            TimeFrame = TimeFrames.OneMinute;
            NumberOfCandles = 1; //we need info of current candle
        }

        public int PositionSize { get; set; }
        public int MaxPositionSize { get; set; }
        public int MinPositionSize { get; set; }       
        public double PercentOrdersOffset { get; set; }
        public TimeFrames TimeFrame { get; set; }
        public int NumberOfCandles { get; set; }
    }
}