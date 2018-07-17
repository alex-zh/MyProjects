using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Common.Classes.General;

namespace Common.Finders
{
    public class AtrStatisticsFinder
    {
        private readonly List<Candle> _candles;
        private readonly int _atrPeriod;

        public AtrStatisticsFinder(List<Candle> candles, int atrPeriod)
        {
            _candles = candles;
            _atrPeriod = atrPeriod;
        }

        public List<double> Find()
        {
            var result = new List<double>();

            for (int i = _atrPeriod; i < _candles.Count; i++)
            {
                var atrValue = new SimpleAtr(_candles.TakeRange(i - _atrPeriod, i).ToArray()).Calculate();

               result.Add(atrValue);
            }

            return result;
        }
    }
}