using Common.Classes.Robot;

namespace Robot.Core
{
    public class ExecutionOptionsKeeper
    {
        public static RobotExecutionOptions GetFor(RobotNames robotName)
        {
            if (robotName == RobotNames.RobotSi1)
            {
                return new RobotExecutionOptions
                {
                    MaxPositionSize = 5,
                    HalfPositionSize = 5,
                    PositionSize = 5,
                    TakeProfitFactor = 7,
                    UseTakeProfit = true
                };
            }
            else if (robotName == RobotNames.RobotSiNtp1)
            {
                return new RobotExecutionOptions
                {
                    MaxPositionSize = 3,
                    HalfPositionSize = 3,
                    PositionSize = 3,
                    UseTakeProfit = true,
                    TakeProfitFactor = 12
                };
            }
            else if (robotName == RobotNames.RobotSi_9_17_1)
            {
                return new RobotExecutionOptions
                {
                    MaxPositionSize = 3,
                    HalfPositionSize = 3,
                    PositionSize = 3,
                    TakeProfitFactor = 7,
                    FmaPeriod = 9,
                    SmaPeriod = 17
                };
            }
            
            return new RobotExecutionOptions();
        }

    }
}