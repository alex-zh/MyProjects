using Common.Classes.Logger;
using Robot.Quik2Net;

namespace JointAssetsViewer
{
    public class DdeServerKeeper
    {
        private static QuikDdeServer _ddeServer;

        public static QuikDdeServer Get()
        {
            if (_ddeServer == null)
            {
                _ddeServer = new QuikDdeServer(JointAssertsSettings.DdeServerName,
                                               JointAssertsSettings.QuikDdeServerTimeTopicName,
                                               JointAssertsSettings.QuikDdeTradesTopicName,
                                               JointAssertsSettings.QuikDdeLimitedOrdersTopicName,
                                               JointAssertsSettings.GetDdeCandlesTopicNames(),
                                               new LoggerToFile());
            }
            return _ddeServer;
        }
    }
}