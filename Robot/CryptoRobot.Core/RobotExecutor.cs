using System;
using System.Threading;
using Common.Classes.Logger;

namespace Robot.Core
{
    public class RobotExecutor : IRobotExecutor
    {
        private IExecutableRobot _executableRobot;
        private readonly ILogger _logger;
        private bool _isExecutionStarted;
        private DateTime _lastIterationTime = DateTime.MinValue;

        public RobotExecutor(ILogger logger)
        {
            _logger = logger;
        }

        public DateTime GetLastIterationTime()
        {
            return _lastIterationTime;
        }
        /// <summary>
        /// Робот может находится в состоянии Started, при этом не находится в состоянии Executing.
        /// После креша работающего робота, загружается состояние, но выполнение автоматически не начинается
        /// </summary>
        public bool IsExecutionStarted
        {
            get { return _isExecutionStarted; }
        }

        public void Start(IExecutableRobot robot)
        {
            if (IsExecutionStarted)
            {
                _logger.Log("Attempt to start execution when IsExecutionStarted flag is true!", LogMessageTypes.Warning);
                return;
            }

            _executableRobot = robot;

            _isExecutionStarted = true;

            while (true)
            {
                try
                {
                    if (_isExecutionStarted == false)
                    {
                        break;
                    }

                    _executableRobot.Iterate();

                    _lastIterationTime = DateTime.Now;

                    Thread.Sleep(RobotSettings.IterationOffset);

                }
                catch (Exception ex)
                {
                    _logger.Log(ex.ToString(), LogMessageTypes.Error);
                }
            }
        }

        public void Stop()
        {
            if (IsExecutionStarted)
            {
                _isExecutionStarted = false;
            }
        }
    }
}