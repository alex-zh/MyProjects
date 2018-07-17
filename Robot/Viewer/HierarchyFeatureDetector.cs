using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes;
using Common.Classes.General;

namespace Viewer
{
    public class HierarchyFeatureDetector
    {
        private readonly List<Candle> _candles;
        private readonly int _maxSplitsCount;
        private readonly int _workingSize;

        public HierarchyFeatureDetector(List<Candle> candles, int workingSize, int maxSplitsCount)
        {
            _candles = candles;
            _maxSplitsCount = maxSplitsCount;
            _workingSize = workingSize;
        }

        public Dictionary<int, TrendFeature>  Detect()
        {
            return Detect(_candles);
        }
        
        public List<Dictionary<int, TrendFeature>> DetectAll()
        {
            var result = new List<Dictionary<int, TrendFeature>>();

            for (int index = 0; index < (_candles.Count - _workingSize); index++)
            {
                var nextIndex = index + _workingSize - 1;

                var candles = _candles.TakeRange(index, nextIndex).ToList();

                var feature = Detect(candles);                     
                
                result.Add(feature);
            }

            return result;
        }

        private Dictionary<int, TrendFeature> Detect(List<Candle> candles)
        {
            var result = new Dictionary<int, TrendFeature>();

            for (int splitIndex = 0; splitIndex <= _maxSplitsCount; splitIndex ++)
            {
                var feautureDetector = new MaxMinBasedFeatureDetector(candles, _workingSize, splitIndex + 1);

                result.Add(splitIndex, feautureDetector.Detect());
            }
          
            return result;
        }
    }    
}