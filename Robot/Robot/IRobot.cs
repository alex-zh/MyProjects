using Common.Robots.Common;

namespace Robot.Core
{
    public interface IExecutableRobot
    {               
        string RobotId { get; }

        string Symbol { get; }

        RobotStateManager StateManager { get; }

        void Iterate();
    }
}