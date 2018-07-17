using Robot.Core.MarketDataProviders;

namespace BitMex.TradeApplication.Classes
{
    public class MarketDataProviderKeeper
    {
        private static  MarketDataProvider _provider;

        public static IMarketDataProvider Get()
        {
            return _provider ?? (_provider = new MarketDataProvider());
        }
    }
}