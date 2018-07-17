using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;

namespace Common.Classes
{
    public class ReturnsCalculator
    {
        private readonly IEnumerable<Candle> _candles;

        public ReturnsCalculator(IEnumerable<Candle> candles)
        {
            _candles = candles;
        }

        public IEnumerable<double> Get(ReturnTypes returnType)
        {
            switch (returnType)
            {
                case ReturnTypes.Normal:
                    return GetNormalReturns();
                default:
                    throw new NotSupportedException();
            }
        }

        private IEnumerable<double> GetNormalReturns()
        {
            return _candles.Select(candle => (candle.Close - candle.Open) * 100 / candle.Open);
        }
    }
}