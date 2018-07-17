using System.Collections;
using System.Collections.Generic;
using Common.Classes.Robot;

namespace Robot.TradeApplication.Controls
{
    internal class LimitedOrdersConverter
    {
        public static IEnumerable ConvertToDisplay(List<LimitedOrder> orders)
        {
            var result = new List<RepresentingLimitedOrder>();

            for (int i = 0; i < orders.Count; i++)
            {
                result.Add(new RepresentingLimitedOrder(orders[i]) { IsFirst = (i == 0), IsLast = (i == orders.Count - 1) });
            }

            return result;
        }
    }
}