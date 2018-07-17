using Common;
using Common.Classes;
using Common.StrategiesOptions;

namespace Robot.Strategies.Agents.Configs
{
    public static partial class StrategyConfigsKeeper
    {
        public static readonly StrategyOptions XbtUsd1Min = new StrategyOptions
        {
            TimeFrame = TimeFrames.OneMinute,

            FmaPeriod = 11,
            SmaPeriod = 21,
            DeviationToOpen = 0.005,
            DeviationToClose = 0.01, //1.5% from sma, will close position
            DeviationToTakeProfit = 0.0005, // % from the candle'close on which we decide to take profit 
            TakeProfitToClose = 0.0065,
            UseDeviationToClose = false,
            UseCandlesCrossingMovings = true,
            NumberOfCandlesAfterMarginClose = 32,
            UseFoundDeviationsToTakeProfit = true,
            Smoothers = new Smoothers
            {
                Hour = false,
                Day = false,
                Aggregation6 = false,
                Aggregation12 = false
            }
        };
    }
}
