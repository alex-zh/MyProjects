using System;
using Robot.Core.Keepers;
using Robot.Core.OperationExecutors;

namespace Robot.Core
{
    public class MarketOperationExecutorKeeper
    {
        private static IMarketOperationExecutor _testMarketOperationExecutor;

        public static IMarketOperationExecutor Get(MarketDataProviderTypes type)
        {
            switch (type)
            {
                case MarketDataProviderTypes.Testing:
                    return _testMarketOperationExecutor ?? (_testMarketOperationExecutor = new MarketOperationTestExecutor());
                default:
                    throw new NotSupportedException(type.ToString());
            }

        }
    }
}