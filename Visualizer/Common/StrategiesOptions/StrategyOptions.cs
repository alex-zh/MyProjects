using Common.Classes;

namespace Common.StrategiesOptions
{
    public class Smoothers
    {
        public bool Hour { get; set; }
        public bool Day { get; set; }
        public bool Aggregation6 { get; set; }
        public bool Aggregation12 { get; set; }
    }

    public class StrategyOptions: IStrategyOptions
    {
       public StrategyOptions()
       {
            PositionSize = 1;
            MaxPositionSize = 1;
            MinPositionSize = -1;
            NumberOfCandles = 100;
            LongNumberOfCandles = 500;
       }

        public TimeFrames TimeFrame { get; set; }

        public int FmaPeriod { get; set; }
        public int SmaPeriod { get; set; }

        public int NumberOfCandles { get; set; } //operative number of candles
        public int LongNumberOfCandles { get; set; } //number of candles used for long term statistics

        public int PositionSize { get; set; }
        public int MaxPositionSize { get; set; }
        public int MinPositionSize { get; set; }
        public int HalfPositionSize { get; set; }

        public bool UseStopLoss { get; set; }
        public bool UseTakeProfit { get; set; }

        public Smoothers Smoothers { get; set; }

        public double DeviationToOpen { get; set; }
        public double TakeProfitToClose { get; set; } //price goes along out position
        public double DeviationToTakeProfit { get; set; } //price goes against out position        
        public double DeviationToClose { get; set; } //price goes against out position        
        public bool UseDeviationToClose { get; set; }
        public int NumberOfCandlesAfterMarginClose { get; set; }
        public bool UseCandlesCrossingMovings { get; set; }

        public bool UseTwoCandlesClosedAboveOfBelowMoving { get; set; }
        public bool UseFoundDeviationsToTakeProfit { get; set; }
    }
}
