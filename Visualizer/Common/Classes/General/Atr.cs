using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Classes.General
{
    /// <summary>
    /// Средний Истинный Диапазон (average true range, ATR) 
    /// Это среднее Истинного Диапазона (True Range, TR)
    /// </summary>
    /// <remarks>
    ///  Истинный диапазон - это наибольшее из следующих 3х значений:
    ///1. TR = H - L
    ///2. TR = H - C.1
    ///3. TR = C.1 - L
    ///Где TR - true range, истинный диапазон
    ///H - сегодняшнее high
    ///L - сегодняшнее low
    ///C.1 - вчерашний уровень close
    /// </remarks>
    public class Atr
    {
        private readonly Candle[] _candles;
        private readonly int _rangeLength;
        private readonly Func<DateTime, bool> _isDateInRange;

        public Atr(Candle[] candles, int rangeLength) : this(candles, rangeLength, null)
        {          
        }

        public Atr(Candle[] candles, int rangeLength, Func<DateTime, bool> isDateInRange)
        {
            _candles = candles;
            _rangeLength = rangeLength;
            _isDateInRange = isDateInRange;
        }


        public static double Calculate(Candle[] candles)
        {
            if (candles.Length < 2)
            {
                throw new ArgumentException("Atr calculation: Candles is less than 2");
            };

            int notNullCandlesCount = candles.Count(x => x != null);

            if (notNullCandlesCount < 2)
            {
                throw new ArgumentException("Atr calculation: Non null candles is less than 2");
            };            

            var allTRs = new SortedList<DateTime, double>();

            if (candles[0] != null)
            {
                allTRs.Add(candles[0].Date, candles[0].High - candles[0].Low);
            }

            for (int ci = 1; ci < candles.Length; ci++)
            {
                if (candles[ci] != null && candles[ci - 1] != null)
                {
                    allTRs.Add(candles[ci].Date, GetTrueRange(candles[ci], candles[ci - 1]));
                }
            }

            var skipCount = allTRs.Count/10;

            var values = allTRs.OrderBy(x => x.Value).Skip(skipCount)
                               .OrderByDescending(x => x.Value).Skip(skipCount)
                               .OrderBy(x => x.Key).Select(x => x.Value).ToArray();

            return CalculateExponentionalAverage(values);

            //CalculateSimpleAverage();
        }

        public double Calculate()
        {
            return Calculate(_candles.Length - 1);
        }

        public double Calculate(int candleIndex)
        {          
            if (candleIndex > _candles.Length)
            {
                throw new ArgumentException("Candle index is bigger than number of candles");
            }

            var tempCandles = new Candle[_rangeLength]; //нужен непрерывный кусок свечей

            for (int i = candleIndex, j = 0; i >= 0 && j < _rangeLength; i--, j++)
            {
                //if (_isDateInRange(_candles[i].DateTime))
                {
                    tempCandles[j] = _candles[i];
                }               
            }

            return Calculate(tempCandles);
        }

        private static double CalculateSimpleAverage(List<double> values, int notNullCandlesCount)
        {
            double result = 0;

            values.Sort();

            var count = 0;
            var tenPercents = notNullCandlesCount / 10;
            //var index = notNullCandlesCount / 10;

            //Убираем 10% минимальных значений и 10% максимальных.
            for (int ci = tenPercents; ci < (values.Count - tenPercents); ci++, count++)
            {
                result += values[ci];
            }

            return result / count;
        }

        public static double CalculateExponentionalAverage(double[] values, int divisor = 3)
        {
            if (values.Length < 1) return 0;

            var k = ((double)divisor / (values.Length + 1));

            var previousValue = values[0];            

            for (int index = 1; index < values.Length; index++)
            {
                var value = values[index];

                var emaValue = previousValue + k * (value - previousValue);                

                previousValue = emaValue;
            }

            return previousValue;
        }

        private static double GetTrueRange(Candle candle, Candle previousCandle)
        {          
            double highMinusLow = candle.High - candle.Low;
            double highMinusPreviousClose = candle.High - previousCandle.Close;
            double previousCloseMinusLow = previousCandle.Close - candle.Low;

            return Math.Max(Math.Max(highMinusLow, highMinusPreviousClose), previousCloseMinusLow);
        }      
    }
}
