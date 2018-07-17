using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Classes.General
{
    /// <summary>
    /// Специальный ATR использоваться будет для оценки сильных движений. 
    /// Частичный выход может быть, например, по достижение 4*Atr
    /// </summary>
    public class SignificantMovementAtr
    {
        private readonly Candle[] _candles;
        private readonly int _rangeLength;

        public SignificantMovementAtr(Candle[] candles, int rangeLength)
        {
            _candles = candles;
            _rangeLength = rangeLength;
        }

        public double Calculate(int candleIndex)
        {
            if (candleIndex > _candles.Length)
            {
                throw new ArgumentException("Candle index is bigger than number of candles");
            }

            var tempCandles = new Candle[_rangeLength];

            for (int i = candleIndex, j = 0; i >= 0 && j < _rangeLength; i--, j++)
            {
                tempCandles[j] = _candles[i];
            }

            return Calculate(tempCandles);
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

            var skipCount = allTRs.Count / 4;
            
            var values = allTRs.OrderBy(x => x.Value).Skip(skipCount)                               
                .OrderBy(x => x.Key).Select(x => x.Value).ToArray();

            return CalculateExponentionalAverage(values);
        }      

        private static double CalculateExponentionalAverage(double[] values)
        {
            if (values.Length < 1) return 0;

            var k = ((double)3 / (values.Length + 1));

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