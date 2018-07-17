using Robot.Core;
using Robot.TradeApplication;

namespace BitMex.TradeApplication.Classes
{
    public class MarketConnectorKeeper
    {
        private static  MarketConnector _connector;

        public static IConnector Get()
        {
            return _connector ?? (_connector = new MarketConnector(LoggerKeeper.Get()));
        }
    }
}