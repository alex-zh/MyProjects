using Common;
using Common.StrategiesOptions;

namespace Robot.Strategies.Agents.Configs
{
    public static partial class StrategyConfigsKeeper
    {
        public static readonly StrategyOptions Btc5Min = new StrategyOptions
        {
            FmaPeriod = 11,
            SmaPeriod = 21,
            DeviationToOpen = 0.005,
            TakeProfitToClose = 0.01,
            DeviationToClose = 0.0075, //1.5% from sma, will close position
            UseDeviationToClose = false,
            UseCandlesCrossingMovings = true,
            NumberOfCandlesAfterMarginClose = 42,
            UseFoundDeviationsToTakeProfit = false,
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
