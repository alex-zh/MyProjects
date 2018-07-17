using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Classes.General
{
    public class SimpleAtr
    {
        private readonly Candle[] _candles;
        private readonly AveragingMethod _averagingMethod;

        public SimpleAtr(IEnumerable<Candle> candles, AveragingMethod averagingMethod = AveragingMethod.Exponentional)
        {
            _candles = candles.ToArray();
            _averagingMethod = averagingMethod;

            if (_candles.Length < 1)
            {
                throw new ArgumentException("Atr calculation: Candles is less than 1");
            }
        }

        public double Calculate()
        {
            var trueRanges = new SortedList<DateTime, double>();

            if (_candles[0] != null)
            {
                trueRanges.Add(_candles[0].Date, _candles[0].High - _candles[0].Low);
            }

            for (int i = 1; i < _candles.Length; i++)
            {
                trueRanges.Add(_candles[i].Date, GetTrueRange(_candles[i], _candles[i - 1]));
            }

            var values = trueRanges.OrderBy(x => x.Key).Select(x => x.Value).ToArray();

            switch (_averagingMethod)
            {
                case AveragingMethod.Simple:
                    return values.Average();
                case AveragingMethod.Exponentional:
                    return CalculateExponentionalAverage(values);
                default:
                    throw new NotSupportedException(_averagingMethod.ToString());
            }
        }

        public double CalculateExponentionalAverage(double[] values, int divisor = 5)
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

        private double GetTrueRange(Candle candle, Candle previousCandle)
        {
            double highMinusLow = candle.High - candle.Low;
            double highMinusPreviousClose = candle.High - previousCandle.Close;
            double previousCloseMinusLow = previousCandle.Close - candle.Low;

            return Math.Max(Math.Max(highMinusLow, highMinusPreviousClose), previousCloseMinusLow);
        }
    }
}