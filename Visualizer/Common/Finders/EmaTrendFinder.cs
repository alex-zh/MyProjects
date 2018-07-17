using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Common.Classes.General;

namespace Common.Finders
{
    public class EmaTrendFinder
    {
        private readonly List<Candle> _candles;
        private readonly int _fmaPeriod;
        private readonly int _smaPeriod;
        private readonly double _minProfit;
        private readonly int _minLength;
        private List<DatePrice> _fma;
        private List<DatePrice> _sma;

        public EmaTrendFinder(List<Candle> candles,  int fmaPeriod, int smaPeriod, double minProfit, int minLength)
        {
            _candles = candles;
            _fmaPeriod = fmaPeriod;
            _smaPeriod = smaPeriod;
            _minProfit = minProfit;
            _minLength = minLength;
            _fma = _candles.Ema(_fmaPeriod, PriceTypes.Mean).ToList();
            _sma = _candles.Ema(_smaPeriod, PriceTypes.Mean).ToList();

        }

        public List<StructureItem> FindAll()
        {
            var differences = _fma.Select((v, index) => v.Value - _sma[index].Value).ToList();

            var result = new List<StructureItem>();

            if (differences.Any() == false)
            {
                return result;
            }

            var startIndex = 0;
            var previousSign = Math.Sign(differences[0]);
            
            for (int i = 1; i < differences.Count; i++)
            {
                var sign =  Math.Sign(differences[i]);

                if (previousSign != sign)
                {
                    result.Add(new StructureItem { StartIndex = startIndex, EndIndex = i - 1, IsUp = previousSign > 0 });

                    startIndex = i;
                    previousSign = sign;
                }
            }

            foreach (var trend in result)
            {
                var range = _fma.TakeRange(trend.StartIndex, trend.EndIndex).ToList();

                var refinedEndIndex = trend.StartIndex +
                                     (trend.IsUp ? range.IndexOfMax(x => x.Value) : range.IndexOfMin(x => x.Value));

                trend.EndIndex = refinedEndIndex;
            }

            return result.Where(x => x.EndIndex - x.StartIndex >= _minLength)
                         .Where(x => CalculateChange(x) >= _minProfit)
                         .Select(x => new StructureItem { StartIndex = x.StartIndex - 1, EndIndex = x.EndIndex - 1 }).ToList();
        }

        private double CalculateChange(StructureItem x)
        {
            return Math.Abs((_candles[x.EndIndex].Close - _candles[x.StartIndex].Open) / _candles[x.StartIndex].Open);
        }
    }
}
