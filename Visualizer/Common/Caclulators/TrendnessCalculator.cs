using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Classes.General;

namespace Common.Caclulators
{
    /// <summary>
    /// First methods:
    /// 1. Calculate deltas
    /// 2. Calculate "one sign deltas" ranges
    /// 3. Take squared sum of ranges
    /// 4. Devide the sum on squared number of deltas
    /// 5. Multipy on 100 to produce result within [1-100] range
    /// Second method:
    /// Sum amount of 2-elements up, 2-elements down
    /// Sum amount of 3-elements up, 3-elements down
    /// 
    /// </summary>
    public class TrendnessCalculator
    {
        private List<Candle> _candles;
        private readonly Func<Candle, double> _getPrice;
        private List<double> _deltas;

        public TrendnessCalculator(List<Candle> candles, Func<Candle, double> getPrice)
        {
            _candles = candles;
            _getPrice = getPrice;
            _deltas = CalculateDeltas();
        }
        
        public int Calculate()
        {
            var count = _deltas.Count;
            var candlesCount = _candles.Count;

            if (count == 0) return 0;

            var oneSignRanges = new List<int>();

            var oneSignRangeNumber = (_deltas[0] > 0) ? 1 : -1;

            for (var i = 1; i < count; i++)
            {
                var delta = _deltas[i];

                if (oneSignRangeNumber > 0 && delta > 0)
                {
                    oneSignRangeNumber++;
                }
                else if(oneSignRangeNumber < 0 && delta < 0)
                {
                    oneSignRangeNumber--;
                }
                else
                {
                    oneSignRanges.Add(oneSignRangeNumber);
                    oneSignRangeNumber = (delta > 0) ? 1 : -1;
                }                
            }

            oneSignRanges.Add(oneSignRangeNumber);

            return (int)(100 * (candlesCount - oneSignRanges.Count) * oneSignRanges.Sum(x => Math.Pow(Math.Abs(x), 1.5) / 
                                           ((candlesCount - 1) * Math.Pow(count, 1.5))));
        }

        private List<double> CalculateDeltas()
        {
            var result = new List<double>();

            for (var i = 1; i < _candles.Count; i++)
            {
                result.Add(_getPrice( _candles[i]) -_getPrice(_candles[i - 1]));
            }

            return result;
        }
    }
}
