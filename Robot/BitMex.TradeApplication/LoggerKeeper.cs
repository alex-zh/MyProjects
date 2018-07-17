using System.Collections.Generic;
using BitMex.Robot.DataLayer.Repositories;
using Common.Classes.Logger;
using Common.Robots.Common;

namespace Robot.TradeApplication
{
    public class LoggerKeeper
    {
        private static Dictionary<string, ILogger> _loggers = new Dictionary<string, ILogger>();

        private static Logger _logger;

        public static ILogger Get()
        {
            return _logger ?? (_logger = new Logger(new LogsRepository()));
        }

        public static ILogger GetFor(string robotId)
        {
            if (_loggers.ContainsKey(robotId) == false)
            {
                _loggers.Add(robotId, new Logger(robotId, new LogsRepository()));
            }

            return _loggers[robotId];
        }
    }
}