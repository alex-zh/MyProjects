using Robot.TradeApplication.Keepers;

namespace Robot.Core.Keepers
{
    public class QuikConnectorKeeper
    {
        private static IQuikConnector _quikConnector;

        public static IQuikConnector GetConnector()
        {
            return _quikConnector ?? (_quikConnector = new QuikConnector(LoggerKeeper.Get()));
        }
    }
}