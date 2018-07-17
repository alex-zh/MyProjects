using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Scatterplot.Core.Classes
{
    public static class CandleExtensions
    {
        public static bool IsUp(this Candle candle)
        {
            return candle.Close > candle.Open;
        }

        public static double GetPrice(this Candle candle, PriceTypes priceType)
        {
            switch (priceType)
            {
                case PriceTypes.Close:
                    return candle.Close;
                case PriceTypes.Open:
                    return candle.Open;
                case PriceTypes.Mean:
                    return (candle.Open + candle.Close)/2;
                default:
                    throw new NotSupportedException(priceType +" is not supported");
            }
        }


        public static IEnumerable<DatePrice> Ema(this IEnumerable<Candle> series, int period, PriceTypes priceType = PriceTypes.Close)
        {
            var result = new List<DatePrice>();

            if (series.Any() == false) return result;

            var k = ((double)2/(period + 1));

            var candles = series.ToList();

            var emaPreviousValue = candles[0].GetPrice(priceType);

            result.Add(new DatePrice { Date = candles[0].Date, Price = emaPreviousValue });

            for (int index = 1; index < candles.Count; index++)
            {
                var candle = candles[index];

                var emaValue = emaPreviousValue + k * (candle.GetPrice(priceType) - emaPreviousValue);

                result.Add(new DatePrice { Date = candle.Date, Price = emaValue });

                emaPreviousValue = emaValue;
            }

            return result;
        }


    }
}
