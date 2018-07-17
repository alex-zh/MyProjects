using Common.Classes.Robot;
using Robot.Core.Classes;

namespace Robot.Core
{
    public interface IExecutableRobot
    {        
        void Iterate();

        RobotState State { get; }
        RobotExecutionOptions ExecutionOptions { get; }
        RobotNames RobotName { get; }        
    }
}