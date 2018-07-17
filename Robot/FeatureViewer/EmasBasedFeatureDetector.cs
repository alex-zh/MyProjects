using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes.General;

namespace Viewer
{
    public class EmasBasedFeatureDetector : IFeatureDetector
    {
        private int _splitCount = 9;
        private readonly List<Candle> _candles;
        private readonly int _workingSize;
        private int _workingRange;
        private List<DatePrice> _fma;
        private List<DatePrice> _sma;

        public EmasBasedFeatureDetector(List<Candle> candles, int workingSize)
        {
            _candles = candles;
            _workingSize = workingSize;
            _workingRange = _workingSize / _splitCount;
            _fma = _candles.Log().Ema(3, PriceTypes.Mean).ToList();
            _sma = _candles.Log().Ema(17, PriceTypes.Mean).ToList();

        }

        public List<TrendFeature> DetectAll()
        {
            var result = new List<TrendFeature>();

            for (int i = _workingSize * 2; i < _candles.Count; i++)
            {
                var feature = new TrendFeature();

                for (int splitCountIndex = 1; splitCountIndex <= _splitCount; splitCountIndex++)
                {
                    var index = i - _workingRange * (_splitCount - splitCountIndex) - 1;
                    var prevIndex = index - _workingRange;

                    feature.FmaValues.Add(new DatePrice()
                    {
                        Date = _fma[index].Date,
                        Value = (_fma[index].Value - _fma[prevIndex].Value)
                    });

                    feature.DiffValues.Add(new DatePrice
                    {
                        Date = _fma[index].Date,
                        Value = (_fma[index].Value - _sma[index].Value)
                    });
                }

                result.Add(feature);
            }

            return result;
        }

        public TrendFeature Detect()
        {
            var result = new TrendFeature();

            for (int splitCountIndex = 1; splitCountIndex <= _splitCount; splitCountIndex++)
            {
                var index = _candles.Count - _workingRange * (_splitCount - splitCountIndex) - 1;
                var prevIndex = index - _workingRange;

                result.FmaValues.Add(new DatePrice()
                {
                    Date = _fma[index].Date,
                    Value = (_fma[index].Value - _fma[prevIndex].Value)
                });

                result.DiffValues.Add(new DatePrice { Date = _fma[index].Date, Value = (_fma[index].Value - _sma[index].Value) });
            }

            return result;
        }
    }
}