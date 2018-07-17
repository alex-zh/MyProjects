using System;
using System.Globalization;
using Common.Robots.Common;

namespace BitMexConnector
{
    public class BitMexPriceConverter
    {
        public static double Convert(double? orderPrice, string orderSymbol)
        {
            if (orderPrice.HasValue == false)
                return 0;

            if (orderSymbol == Symbols.BitMex.XBTUSD)
            {
                double integralPart = Math.Truncate(orderPrice.Value);
                return double.Parse(integralPart.ToString(CultureInfo.InvariantCulture) + ".0");
            }

            return double.Parse(orderPrice.Value.ToString("N8"));
        }
    }
}