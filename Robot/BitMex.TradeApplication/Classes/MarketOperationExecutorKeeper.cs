using Robot.Core;
using Robot.TradeApplication;

namespace BitMex.TradeApplication.Classes
{
    public class MarketOperationExecutorKeeper
    {
        private static MarketOperationExecutor _executor;

        public static IMarketOperationExecutor Get()
        {
            var logger = LoggerKeeper.Get();
            return _executor ?? (_executor = new MarketOperationExecutor(logger));
        }
    }
}