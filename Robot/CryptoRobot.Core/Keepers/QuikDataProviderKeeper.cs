using System.Collections.Generic;
using Common.Classes.Robot;
using Robot.Core.Providers;

namespace Robot.Core.Keepers
{
    public class QuikDataProviderKeeper
    {
        private static BitMexDataProvider _quikDataProvider;
        private static BitMexLogDataProvider _loggingQuikDataProvider;

        public static BitMexDataProvider GetOperationalOne()
        {
            return _quikDataProvider ?? (_quikDataProvider = new BitMexDataProvider());
        }

        public static BitMexLogDataProvider GetLoggingOne(IEnumerable<LimitedOrder> initialOrders )
        {
            return _loggingQuikDataProvider ?? (_loggingQuikDataProvider = new BitMexLogDataProvider(initialOrders));
        }
    }
}