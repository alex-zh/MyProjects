using System.Collections;
using System.Collections.Generic;
using Common.Robots.Common;
using Robot.TradeApplication.Controls;

namespace BitMex.TradeApplication.Controls
{
    internal class LimitedOrdersConverter
    {
        public static IEnumerable ConvertToDisplay(List<LimitOrderOld> orders)
        {
            var result = new List<RepresentingLimitOrderOld>();

            for (int i = 0; i < orders.Count; i++)
            {
                result.Add(new RepresentingLimitOrderOld(orders[i]) { IsFirst = (i == 0), IsLast = (i == orders.Count - 1) });
            }

            return result;
        }
    }
}