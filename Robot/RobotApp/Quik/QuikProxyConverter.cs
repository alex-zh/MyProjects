using Common;
using Common.Classes.Robot;
using Common.Robots.Common;
using Robot.Trans2Quik.Entities.Transaction.Order;

namespace Robot.Core
{
    public class QuikProxyConverter
    {
        public static OrderTradeParams ToOrderTradeParams(Securities security, OrderParams orderParams)
        {
            var quikSecurity = QuikSecurityFactory.Get(security);
            var minimapPriceStep = GetMinimalPriceStep(security);

            var direction = orderParams.Direction == Direction.Buy ? 
                            Trans2Quik.Entities.Direction.Buy: Trans2Quik.Entities.Direction.Sell;

            return new OrderTradeParams(quikSecurity, direction, orderParams.Size, (decimal)orderParams.Price, minimapPriceStep);
        }

        private static decimal GetMinimalPriceStep(Securities security)
        {

            switch (security)
            {
                case Securities.Si:
                    return 1;                
            }

            return 1;
        }
    }
}