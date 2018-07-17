using System;

namespace Robot.Core
{
    public interface IRobotExecutor
    {        
        void Start(IExecutableRobot robot);
        void Stop();

        bool IsExecutionStarted { get; }

        DateTime GetLastIterationTime();
    }
}