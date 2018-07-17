using Common.Classes;

namespace Robot.Strategies
{
    public class StrategyOptionsX
    {
        public int FmaPeriod { get; set; }
        public int SmaPeriod { get; set; }
        public int TakeProfitFactor { get; set; }
        public  TimeFrames TimeFrames { get; set; }
        public int NumberOfCandlesAfterMarginClose { get; set; }
        public int MarginUpPercentToClose { get; set; }
        public int MarginDownPercentToClose { get; set; }
    }
}