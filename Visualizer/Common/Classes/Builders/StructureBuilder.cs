using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;
using Common.Finders;

namespace Common.Classes.Builders
{
    /// <summary>
    /// Builds structure, moving back from endIndex
    /// </summary>
    public class StructureBuilder
    {
        private const int MaxItemLength = 20;

        private readonly List<Candle> _candles;
        private List<DatePrice> _emas;
            
        private int MinItemLength = 3;

        public StructureBuilder(List<Candle> candles)
        {
            _candles = candles;
            _emas = candles.Ema(2, PriceTypes.Close).ToList();
        }

        public Structure Build(int endIndex, int length = 3)
        {
            var result = new Structure();           

            var index = endIndex;

            for (int i = 0; i < length; i++)
            {
                var atr = new SimpleAtr(_candles.TakeRange(Math.Max(0, index - 30), endIndex)).Calculate();

                var item = BuildStructureItem(index, atr);

                if (item == null) return null;

                index = item.StartIndex;

                result.Add(item);
            }

            return result;
        }

        public StructureItem BuildStructureItem(int endIndex, double atr)
        {            
            var startIndex = Math.Max(0, endIndex - MaxItemLength);
            
            for (int i = endIndex; i > startIndex; i--)
            {
                if (IsEmaDeviationCorrect(i, endIndex, atr) == false)
                {                                
                    if (endIndex - i < MinItemLength)
                    {
                        return null;
                    }

                    return new StructureItem() { StartIndex = i + 1, EndIndex = endIndex };
                }
            }

            return new StructureItem() { StartIndex = startIndex, EndIndex = endIndex };
        }

        private double CalculateError(int startIndex, int endIndex)
        {
            var range = _emas.TakeRange(startIndex, endIndex).ToList();
            var change = (range.Last().Value - range.First().Value) / range.First().Value;
            var stepChange = change / (endIndex - startIndex + 1);

            var deviations = range.Select((candle, index) =>
            {
                var trendValue = ((index + 1) * stepChange + 1) * range.First().Value;
                return candle.Value - trendValue;
            }).ToList();

            var error = Math.Sqrt(deviations.Sum(x => x*x));

            return error;
        }

        private bool IsDeviationCorrect(int startIndex, int endIndex, double atr)
        {
            var range = _candles.TakeRange(startIndex, endIndex).ToList();
            var change = (range.Last().Close - range.First().Open)/range.First().Open;
            var stepChange =  change /  (endIndex - startIndex + 1);

            var deviations = range.Select((candle, index) =>
            {
                var trendValue = ((index + 1) * stepChange + 1) * range.First().Open;
                return candle.Close - trendValue;
            }).ToList();

            var maxDeviation = deviations.Select(Math.Abs).Max();

            if (maxDeviation > 1.5 * atr)
            {
                return false;
            }

            return true;
        }

        private bool IsEmaDeviationCorrect(int startIndex, int endIndex, double atr)
        {
            var range = _emas.TakeRange(startIndex, endIndex).ToList();
            var change = (range.Last().Value - range.First().Value) / range.First().Value;
            var stepChange = change / (endIndex - startIndex + 1);

            var deviations = range.Select((candle, index) =>
            {
                var trendValue = ((index + 1) * stepChange + 1) * range.First().Value;
                return candle.Value - trendValue;
            }).ToList();

            var maxDeviation = deviations.Select(Math.Abs).Max();

            if (maxDeviation > 0.75 * atr)
            {
                return false;
            }

            return true;
        }
    }
}
