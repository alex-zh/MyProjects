namespace Robot.Core
{
    public class RobotExecutionOptions
    {
        public RobotExecutionOptions()
        {            
            FmaPeriod = 11;
            SmaPeriod = 21;
            UseTakeProfit = true;

            OperationalCandlesCount = 1000;
            LongSeriesCandlesCount = 1000;
        }

        public int FmaPeriod { get; set; }

        public int SmaPeriod { get; set; }

        public int PositionSize { get; set; }

        public int HalfPositionSize { get; set; }

        public int MaxPositionSize { get; set; }

        public int MinPositionSize { get { return -1 * MaxPositionSize; } }

        public int TakeProfitFactor { get; set; }

        public bool UseTakeProfit { get; set; }

        public int  LongSeriesCandlesCount { get; set; }
        public int OperationalCandlesCount { get; set; }
    }
}