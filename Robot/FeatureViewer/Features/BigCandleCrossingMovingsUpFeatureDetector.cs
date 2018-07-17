using System;
using System.Collections.Generic;
using Common;
using Common.Classes;
using Common.Classes.Analizers;
using Common.Classes.General;

namespace FeatureViewer.Features
{
    public class BigCandleCrossingMovingsUpFeatureDetector : IFeatureDetector<int>
    {
        private readonly List<Candle> _candles;
        private readonly int _windowSize;
        private MovingsAnalizer _movingsAnalizer;

        public BigCandleCrossingMovingsUpFeatureDetector(List<Candle> candles, int windowSize, int fmaPeriod, int smaPeriod)
        {
            _candles = candles;
            _windowSize = windowSize;
            _movingsAnalizer = new MovingsAnalizer(_candles, fmaPeriod, smaPeriod);
        }

        public List<int> Detect()
        {
            var result = new List<int>();
            for (int i = _windowSize; i < _candles.Count; i++)
            {
                if (IsPresent(i))
                {
                    result.Add(i);
                }
            }

            return result;
        }

        public bool IsPresent(int index)
        {
            if (_candles.Count < 2) return false;

            var candlesStatistics = new CandlesStatisticsCalculator(_candles.TakeRange(index - _windowSize, index - 1)).Calculate();

            var currentCandle = _candles[index];
            var prevCandle = _candles[index - 1];

            bool areMovingsCloseEnough = Math.Abs(_movingsAnalizer.GetFma(index) - _movingsAnalizer.GetSma(index)) <
                                          candlesStatistics.Body.Median ||
                                          _movingsAnalizer.GetFma(index) < _movingsAnalizer.GetSma(index);

            bool isCloseAboveMovings = currentCandle.Close >
                                       Math.Max(_movingsAnalizer.GetSma(index), _movingsAnalizer.GetFma(index));

            bool isOpenBelowMovings = currentCandle.Open <=
                                       Math.Min(_movingsAnalizer.GetSma(index), _movingsAnalizer.GetFma(index));
            bool isSmallTailUp = currentCandle.High - Math.Max(currentCandle.Open, currentCandle.Close) <=
                                       (currentCandle.Close-currentCandle.Open)/3;

            if (isOpenBelowMovings &&
                isSmallTailUp &&
                currentCandle.IsUp() &&
                isCloseAboveMovings &&
                areMovingsCloseEnough &&
                Math.Max(prevCandle.Open, prevCandle.Close) <= Math.Max(_movingsAnalizer.GetFma(index - 1), _movingsAnalizer.GetSma(index - 1)))
            {
                if (currentCandle.Volume > candlesStatistics.Volume.Median &&
                    currentCandle.Get(PriceTypes.Body) > 1.25 * candlesStatistics.Body.Median)
                    return true;
            }

            return false;
        }

    }
}