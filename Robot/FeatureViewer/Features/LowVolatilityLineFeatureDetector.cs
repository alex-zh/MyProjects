using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes;
using Common.Classes.Analizers;
using Common.Classes.General;
using Viewer;

namespace FeatureViewer
{
    public class LowVolatilityLineFeatureDetector : IFeatureDetector<LowVolatilityLine>
    {
        private readonly List<Candle> _candles;
        private readonly int _lineWidth;
        private MovingsAnalizer _movingsAnalizer;
        private CandlesStatistics _candlesStatistics;
        private int _statisticsWindow = 8 * 60; // 480 min
        private int _lastStatisticsIndex = 0;

        public LowVolatilityLineFeatureDetector(List<Candle> candles)
        {
            _candles = candles;
            _lineWidth = 16;
         //   _movingsAnalizer = new MovingsAnalizer(_candles, fmaPeriod, smaPeriod);
        }

        public List<LowVolatilityLine> Detect()
        {
            var result = new List<LowVolatilityLine>();


            for (int i = Math.Max(_lineWidth, _statisticsWindow); i < _candles.Count; i++)
            {
                if (i - _lastStatisticsIndex > _statisticsWindow / 20)
                {
                    _candlesStatistics =
                        new CandlesStatisticsCalculator(_candles.TakeRange(i - _statisticsWindow, i - 1)).Calculate();
                }


                if (IsPresent(i))
                {
                    result.Add(new LowVolatilityLine { StartIndex = i - _lineWidth, EndIndex = i });
                    i +=  _lineWidth;
                }
            }

            return result;
        }

        public bool IsPresent(int index)
        {
            if (_candles.Count < _lineWidth) return false;

            var range = _candles.TakeRange(index - _lineWidth, index).ToList();

            var firstCandle = range.First();
            var secondCandle = range.Last();

            var diff = secondCandle.Get(PriceTypes.Mean) - firstCandle.Get(PriceTypes.Mean);

            if (diff > 0)
            {
                diff = Math.Max(secondCandle.Close, secondCandle.Open) -
                       Math.Min(firstCandle.Close, firstCandle.Open);
            }
            else
            {
                diff = Math.Max(firstCandle.Close, firstCandle.Open) -
                       Math.Min(secondCandle.Close, secondCandle.Open);
            }

            var rangeMin = range.Min(x => Math.Min(x.Open, x.Close));
            var rangeMax = range.Max(x => Math.Max(x.Open, x.Close));


            if (diff < 1.5 * _candlesStatistics.Body.Median &&
                rangeMax - rangeMin <= 4 * _candlesStatistics.Body.Median)
            {
                return true;
            }

            return false;
        }

        //public bool IsPresent_v1(int index)
        //{
        //    if (_candles.Count < _lineWidth) return false;

        //    var range = _candles.TakeRange(index - _lineWidth, index).ToList();
        //    var rangeMin = range.Min(x => Math.Min(x.Open, x.Close));
        //    var rangeMax = range.Max(x => Math.Max(x.Open, x.Close));

        //    if (rangeMax - rangeMin < 2 * _candlesStatistics.Body.Percentile90)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

    }
}