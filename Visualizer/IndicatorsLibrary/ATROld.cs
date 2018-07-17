using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Common.Classes.General;

namespace Core
{   
    public class VolatilityCalculator : IVolatilityCalculator
    {
        private readonly List<Candle> _candles;
        private const int AveragePeriod = 8; // 8 hours x 2 days

        public VolatilityCalculator(IEnumerable<Candle> candles)
        {
            _candles = candles.ToList();
        }

        public double Get(int index)
        {
            if (index <= AveragePeriod)
            {
                return 0;
            }

            double result = 0;

            for (int ci = index; ci >= index - AveragePeriod; ci--)
            {
                result += GetOneCandleVolatility(_candles[ci], _candles[ci - 1]);
            }

            return result / AveragePeriod;
            
        }

        public static double GetOneCandleVolatility(Candle candle, Candle previousCandle)
        {
            double highMinusLow = candle.High - candle.Low;
            double highMinusPreviousClose = candle.High - previousCandle.Close;
            double previousCloseMinusLow = previousCandle.Close - candle.Low;

            double deviation = Math.Max(Math.Max(highMinusLow, highMinusPreviousClose), previousCloseMinusLow);
            double candleMean = (candle.Close + candle.Open)/2;
            return Math.Abs(deviation) * 100 / candleMean;
        }

        public SortedList<DateTime, double> GetAll()
        {
            var values = new SortedList<DateTime, double>(_candles.Count);

            for (int i = 0; i < _candles.Count; i++)
            {
                var value = Get(i);

                values.Add(_candles[i].Date, value);
            }

            return values;
        }
    }
}
