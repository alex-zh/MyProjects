using BitMexConnector;
using Robot.TradeApplication;

namespace BitMex.TradeApplication.Classes
{
    public class ConnectorKeeper
    {
        private static Connector _connector;

        public static Connector GetConnector()
        {
            
            return _connector ?? (_connector = new Connector(LoggerKeeper.Get()));
        }
    }
}