using System.Collections.Generic;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Robot.DataLayer.Repositories;

namespace Robot.Core.Keepers
{
    public class LoggerKeeper
    {
        private static Dictionary<RobotNames, ILogger> _loggers = new Dictionary<RobotNames, ILogger>();

        private static Logger _logger;

        public static ILogger Get()
        {
            return _logger ?? (_logger = new Logger(new LogsRepository()));
        }

        public static ILogger GetFor(RobotNames robotName)
        {
            if (_loggers.ContainsKey(robotName) == false)
            {
                _loggers.Add(robotName, new Logger(robotName, new LogsRepository()));
            }

            return _loggers[robotName];
        }
    }
}