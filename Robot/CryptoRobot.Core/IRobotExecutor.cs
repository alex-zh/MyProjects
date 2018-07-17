using System;

namespace Robot.Core
{
    public interface IRobotExecutor
    {
        DateTime GetLastIterationTime();

        void Start(IExecutableRobot robot);
        void Stop();
        bool IsExecutionStarted { get; }
    }
}