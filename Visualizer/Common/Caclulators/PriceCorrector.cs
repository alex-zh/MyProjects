using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Caclulators
{
    /// <summary>
    /// Corrects price to increase probablity of order execution
    /// 63549 - > 63556
    /// 63214 - > 63216
    /// </summary>
    public class PriceCorrector
    {
        public static double GetPrice(double price, Direction direction)
        {
            //return price;
            var lastDigit = price % 10;
            
            if (direction == Direction.Buy)
            {
                return ((int)(price / 10)) * 10 + (lastDigit < 6 ? 6 : 16);
            }
            else if (direction == Direction.Sell)
            {
                return ((int)(price / 10)) * 10 + (lastDigit < 4 ?  -6 : 4);
            }

            return price;
        }
    }
}
