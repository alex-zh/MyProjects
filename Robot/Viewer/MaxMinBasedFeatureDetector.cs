using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes;
using Common.Classes.General;

namespace Viewer
{
    public class MaxMinBasedFeatureDetector : IFeatureDetector
    {
        private readonly int _splitCount;
        private readonly List<Candle> _logCandles;
        private readonly int _workingSize;
        private readonly int _splitSize;

        public MaxMinBasedFeatureDetector(List<Candle> candles, int workingSize, int splitCount)
        {
            _splitCount = splitCount;
            _workingSize = workingSize;
            
            _logCandles = candles.Log().ToList();
            _splitSize = _workingSize / _splitCount;
        }

        public TrendFeature Detect()
        {
            return Detect(_logCandles);
        }
        
        public List<TrendFeature> DetectAll()
        {
            var result = new List<TrendFeature>();

            for (int index = 0; index < _logCandles.Count; index++)
            {
                var nextIndex = index + _workingSize - 1;

                var candles = _logCandles.TakeRange(index, nextIndex).ToList();

                var feature = Detect(candles);                     
                
                result.Add(feature);
            }

            return result;
        }
        
        private TrendFeature Detect(List<Candle> candles)
        {
            var result = new TrendFeature();

            for (int splitCountIndex = 1; splitCountIndex <= _splitCount; splitCountIndex++)
            {
                var index = _splitSize * (splitCountIndex - 1);
                var nextIndex = index + _splitSize - 1;

                var splitRangeCandles = candles.TakeRange(index, nextIndex).ToList();

                var firstCandle = splitRangeCandles.First();
                var endCandle = splitRangeCandles.Last();
                var candleWithMinClose = splitRangeCandles.OrderBy(x => x.Close).First();
                var candleWithMaxClose = splitRangeCandles.OrderByDescending(x => x.Close).First();

                result.Values.Add(new DateRangeValue
                {
                    StartDate = splitRangeCandles[0].Date,
                    EndDate = splitRangeCandles.Last().Date,
                    Value = (endCandle.Close - firstCandle.Close)
                });

                result.Values.Add(new DateRangeValue()
                {
                    StartDate = splitRangeCandles[0].Date,
                    EndDate = splitRangeCandles.Last().Date,
                    Value = (candleWithMaxClose.Close - candleWithMinClose.Close) * (candleWithMaxClose.Date > candleWithMinClose.Date ? 1 : -1)
                });
            }
          
            return result;
        }
    }    
}