using Robot.Quik2Net;

namespace Robot.Core.Keepers
{
    public class DdeServerKeeper
    {
        private static QuikDdeServer _ddeServer;

        public static QuikDdeServer Get()
        {
            if (_ddeServer == null)
            {
                _ddeServer = new QuikDdeServer(RobotSettings.DdeServerName,
                                               RobotSettings.QuikDdeServerTimeTopicName,
                                               RobotSettings.QuikDdeTradesTopicName,
                                               RobotSettings.QuikDdeLimitedOrdersTopicName,
                                               RobotSettings.GetDdeCandlesTopicNames(),
                                               LoggerKeeper.Get());
            }
            return _ddeServer;
        }
    }
}