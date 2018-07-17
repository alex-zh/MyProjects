using Common;
using Common.StrategiesOptions;

namespace Robot.Strategies.Agents.Configs
{
    public static partial class StrategyConfigsKeeper
    {
        public static readonly StrategyOptions XyzBtc1Hour = new StrategyOptions
        {
            FmaPeriod = 11,
            SmaPeriod = 21,
            DeviationToOpen = 0.005,
            TakeProfitToClose = 0.0065,
            DeviationToClose = 0.0075, //1.5% from sma, will close position
            DeviationToTakeProfit = 0.0002, // % from the candle'close on which we decide to take profit 
            UseDeviationToClose = false,
            UseCandlesCrossingMovings = true,
            NumberOfCandlesAfterMarginClose = 0,
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
