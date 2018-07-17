using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Core;

namespace Strategies.UnitTests
{
    public class VolatilityCalculatorStub : IVolatilityCalculator
    {
        private readonly List<Candle> _candles;

        public VolatilityCalculatorStub(IEnumerable<Candle> candles )
        {
            _candles = candles.ToList();
        }

        public double Get(int index)
        {
            return Math.Abs(_candles[index].Close - _candles[index].Open);
        }

        public SortedList<DateTime, double> GetAll()
        {
            var result = new SortedList<DateTime, double>();

            for (int i=0; i<  _candles.Count; i++)
            {
                result.Add(_candles[i].Date.AddHours(i),  Get(i));
            }

            return result;
        }
    }
}