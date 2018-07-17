using Common;
using Common.Classes;
using Common.StrategiesOptions;

namespace Robot.Strategies.Agents.Configs
{
    public static partial class StrategyConfigsKeeper
    {
        public static readonly NetThrowerOptions NetThrower_63_0015 = new NetThrowerOptions()
        {
            PositionSize = 100,
            MaxPositionSize = 700,
            MinPositionSize = -700,            
            PercentOrdersOffset = 0.00015,
            TimeFrame = TimeFrames.OneMinute,
            NumberOfCandles = 1,
        };

        public static readonly NetThrowerOptions NetThrower_1023_00075 = new NetThrowerOptions()
        {
            PositionSize = 1,
            MaxPositionSize = 1023,
            MinPositionSize = -1023,
            PercentOrdersOffset = 0.00075,
            TimeFrame = TimeFrames.OneMinute,
            NumberOfCandles = 1,
        };

        public static readonly NetThrowerOptions NetThrower_1023_001 = new NetThrowerOptions()
        {
            PositionSize = 1,
            MaxPositionSize = 1023,
            MinPositionSize = -1023,
            PercentOrdersOffset = 0.001,
            TimeFrame = TimeFrames.OneMinute,
            NumberOfCandles = 1,
        };

    }
}
