using System;
using System.Collections.Generic;
using Common.Robots.Common;
using Robot.Core.MarketDataProviders;
using Robot.Core.MarketDataProviders.Quik;
using Robot.Core.Providers.Quik;

namespace Robot.Core.Keepers
{
    public enum MarketDataProviderTypes
    {
        Operative,
        Logging,
        Testing
    }

    public class MarketDataProviderKeeper
    {
        private static QuikMarketDataProvider _quikMarketDataProvider;
        private static QuikLoggingMarketDataProvider _loggingMarketDataProvider;
        private static TestMarketDataProvider _testMarketDataProvider;

        public static void Reset()
        {
            _quikMarketDataProvider = null;
            _loggingMarketDataProvider = null;
            _testMarketDataProvider = null;
        }

        public static IMarketDataProvider Get(RobotNames robotName, MarketDataProviderTypes type, IEnumerable<LimitOrder> initialOrders = null)
        {
            switch (type)
            {
                case MarketDataProviderTypes.Operative:
                    return _quikMarketDataProvider ?? (_quikMarketDataProvider = new QuikMarketDataProvider());
                case MarketDataProviderTypes.Logging:
                    return _loggingMarketDataProvider ?? (_loggingMarketDataProvider = new QuikLoggingMarketDataProvider(initialOrders));
                case MarketDataProviderTypes.Testing:
                    return _testMarketDataProvider ?? (_testMarketDataProvider = new TestMarketDataProvider());
                default:
                    throw new NotSupportedException($"For robot {robotName} there is not appropriate market data provider");
            }
            
        }
    }
}