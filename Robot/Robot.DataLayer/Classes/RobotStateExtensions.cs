using Common.Classes.Robot;
using Common.Robots.Common;
using Common.Robots.TrendFollower;

namespace Robot.DataLayer.Classes
{
    public static class RobotStateExtensions
    {
        public static DbRobotState ToDbRobotState(this RobotState robotState, int robotId)
        {            
            return new DbRobotState()
            {
                RobotId =  robotId,
                PositionSize = robotState.Size
            };
        }
    }
}