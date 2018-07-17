using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Classes.General
{
    public static class CandleExtensions
    {
        public static bool IsUp(this Candle candle)
        {
            return candle.Close > candle.Open;
        }

        public static bool IsDown(this Candle candle)
        {
            return candle.Close < candle.Open;
        }

        public static double Get(this Candle candle, PriceTypes priceType)
        {
            switch (priceType)
            {
                case PriceTypes.Close:
                    return candle.Close;
                case PriceTypes.Open:
                    return candle.Open;
                case PriceTypes.Mean:
                    return (candle.Open + candle.Close)/2;
                case PriceTypes.Body:
                    return Math.Abs(candle.Close - candle.Open);
                case PriceTypes.Volume:
                    return candle.Volume;
                default:
                    throw new NotSupportedException(priceType +" is not supported");
            }
        }

        public static IEnumerable<DatePrice> Ema(this IEnumerable<DatePrice> series, int period)
        {
            var values = series.ToList();

            var result = new List<DatePrice>();

            if (values.Any() == false) return result;

            var k = ((double)2 / (period + 1));

            var emaPreviousValue = values[0].Value;

            result.Add(new DatePrice { Date = values[0].Date, Value = emaPreviousValue });

            for (int index = 1; index < values.Count; index++)
            {
                var candle = values[index];

                var emaValue = emaPreviousValue + k * (candle.Value - emaPreviousValue);

                result.Add(new DatePrice { Date = candle.Date, Value = emaValue });

                emaPreviousValue = emaValue;
            }

            return result;
        }

        public static IEnumerable<Candle> GetRange(this IEnumerable<Candle> series, Candle startCandle, Candle endCandle)
        {
            var candles = series.ToList();

            var startIndex = candles.IndexOf(startCandle);
            var endIndex = candles.IndexOf(endCandle);

            if (startIndex > 0 && endIndex > startIndex)
            {
                return candles.TakeRange(startIndex, endIndex);
            }

            return new Candle[0];     
        }


        public static IEnumerable<DatePrice> Ema(this IEnumerable<Candle> series, int period, PriceTypes priceType = PriceTypes.Close, bool extended = false)
        {
            var candles = series.ToList();

            if (extended)
            {                
                var lastCandle = candles.Last();

                candles.Add(new Candle()
                {
                    Date = lastCandle.Date,
                    Open = lastCandle.Get(PriceTypes.Mean),
                    High = lastCandle.Get(PriceTypes.Mean),
                    Low = lastCandle.Get(PriceTypes.Mean),
                    Close = lastCandle.Get(PriceTypes.Mean)
                });

                candles.RemoveAt(0);
            }
            
            var result = new List<DatePrice>();

            if (candles.Any() == false) return result;

            var k = ((double)2/(period + 1));
            
            var emaPreviousValue = candles[0].Get(priceType);

            result.Add(new DatePrice { Date = candles[0].Date, Value = emaPreviousValue });

            for (int index = 1; index < candles.Count; index++)
            {
                var candle = candles[index];

                var emaValue = emaPreviousValue + k * (candle.Get(priceType) - emaPreviousValue);

                result.Add(new DatePrice { Date = candle.Date, Value = emaValue });

                emaPreviousValue = emaValue;
            }

            return result;
        }

        public static Candle Log(this Candle candle)
        {
            return  new Candle()
            {
                Date = candle.Date,
                Open = Math.Log(candle.Open),
                High = Math.Log(candle.High),
                Low = Math.Log(candle.Low),
                Close = Math.Log(candle.Close)
            };
        }

        public static IEnumerable<Candle> Log(this IEnumerable<Candle> series)
        {
            return series.Select(candle => candle.Log());
        }

        /// <summary>
        /// AggregationPeriod - кол-во свечей которые объединяются в одну. 
        /// Например, работая на 1-минутных свечах, можно построить ema для 5-минутной агрегации
        /// </summary>
        public static IEnumerable<DatePrice> Ema(this IEnumerable<Candle> series, int period, int aggregationPeriod)
        {            
            var values = series.ToList();

            var result = new List<DatePrice>();

            if (values.Any() == false) return result;

            var k = ((double)2 / (period + 1));

            var emaPreviousValue = values[0].Close;

            result.Add(new DatePrice { Date = values[0].Date, Value = emaPreviousValue });

            var startIndex = values.Count + aggregationPeriod - aggregationPeriod * (int)((double)values.Count / aggregationPeriod) - 1;

            for (int index = 1; index < values.Count; index++)
            {
                var candle = values[index];

                if ((index + aggregationPeriod - startIndex) % aggregationPeriod == 0)
                {                  
                    var emaValue = emaPreviousValue + k * (candle.Close - emaPreviousValue);

                    result.Add(new DatePrice {Date = candle.Date, Value = emaValue});

                    emaPreviousValue = emaValue;
                }
                else
                {
                    result.Add(new DatePrice { Date = candle.Date, Value = emaPreviousValue });
                }
            }

            return result;
        }

      

        public static IEnumerable<TSource> IndexRange<TSource>(this IList<TSource> source, int fromIndex, int toIndex)
        {
            int currIndex = fromIndex;
            while (currIndex <= toIndex)
            {
                yield return source[currIndex];
                currIndex++;
            }
        }

        public static double MeanSize(this IEnumerable<Candle> series)
        {
            var values = series.ToList().Select(x=>Math.Abs(x.Open-x.Close));

            var calculator = new StatisticsCalculator(values);

            return calculator.Mean();
        }

        //public static IEnumerable<Candle> GetRange(this IEnumerable<Candle> allCandles, int startIndex, int endIndex)
        //{
        //    var index = Math.Max(0, startIndex);
        //    var count = endIndex - startIndex;

        //    return allCandles.Skip(index).Take(count);
        //}

        public static int GetIndexByDate(this IEnumerable<Candle> allCandles, DateTime date, bool optimisticStrategy = true)
        {
            var candles = allCandles.ToList();

            for (int i = 1; i < candles.Count; i++)
            {
                var candle = candles[i];
                var prevCandle = candles[i - 1];

                if (optimisticStrategy)
                {
                    if (prevCandle.Date <= date && candle.Date >= date)
                        return i;
                }
                else
                {
                    if (prevCandle.Date <= date && candle.Date >= date)
                        return i - 1;
                }
            }

            return -1;
        }

        public static Candle GetByDate(this List<Candle> allCandles, DateTime date)
        {
            var index = GetIndexByDate(allCandles, date);

            if (index < 0 || index > allCandles.Count - 1)
            {
                return null;
            }

            return allCandles[index];            
        }

        public static double GetMax(this List<Candle> allCandles)
        {
            if (allCandles.Any() == false)
                return 0;

            return allCandles.Max(x => x.High);
        }

        public static double GetMin(this List<Candle> allCandles)
        {
            if (allCandles.Any() == false)
                return 0;

            return allCandles.Min(x => x.Low);                     
        }

        public static string ToText(this IEnumerable<Candle> series)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<DATE>;<TIME>;<OPEN>;<HIGH>;<LOW>;<CLOSE>;<VOL>");

            foreach (var candle in series)
            {
                builder.AppendLine(String.Format("{0}; {1}; {2}; {3}; {4}; {5}; {6}",
                    candle.Date.ToString("yyyyMMdd"),
                    candle.Date.TimeOfDay.ToString("hhmmss"),
                    candle.Open.ToString("F2"),
                    candle.High.ToString("F2"), 
                    candle.Low.ToString("F2"), 
                    candle.Close.ToString("F2"), 
                    candle.Volume.ToString("F2")));                
            }

            return builder.ToString();
        }
    }
}
