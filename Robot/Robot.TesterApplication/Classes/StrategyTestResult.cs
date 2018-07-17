using System;
using Common.Classes.StrategyTester;

namespace Robot.TesterApplication.Classes
{
    [Serializable]
    public class StrategyTestResult
    {
        public string Description { get; set; }
        public TradeDataStatistics Statistics { get; set; }
    }
}