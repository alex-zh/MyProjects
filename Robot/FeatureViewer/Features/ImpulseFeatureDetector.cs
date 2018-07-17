using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes;
using Common.Classes.Analizers;
using Common.Classes.General;

namespace FeatureViewer.Features
{
    public class ImpulseFeatureDetector : IFeatureDetector<int>
    {
        private readonly List<Candle> _candles;
        private MovingsAnalizer _movingsAnalizer;
        private int _historyWindowSize = 600; ///24*60;
        private int _workingWindowSize = 60;

        public ImpulseFeatureDetector(List<Candle> candles, int fmaPeriod, int smaPeriod)
        {
            _candles = candles;
            _movingsAnalizer = new MovingsAnalizer(_candles, fmaPeriod, smaPeriod);
        }

        public List<int> Detect()
        {
            var result = new List<KeyValuePair<int, double>>();

            for (int i = _workingWindowSize; i < _candles.Count; i++)
            {
                double rangeSwing = double.MaxValue;
                if (IsPresent(i, out rangeSwing))
                {
                    result.Add(new KeyValuePair<int, double>(i, rangeSwing));
                  //  i += _workingWindowSize;
                }
            }


            var cleanResult = CleanOverlaped(result.OrderBy(x=>x.Key).ToList());

            return cleanResult.Select(x=>x.Key).ToList();
        }

        private List<KeyValuePair<int, double>> CleanOverlaped(List<KeyValuePair<int, double>> result)
        {
            var cleanResult = new List<KeyValuePair<int, double>>();
            var overlappedRanges = new List<KeyValuePair<int, double>>();

            overlappedRanges.Add(result[0]);

            for (int i = 1; i < result.Count; i++)
            {                
                var current = result[i];

                var prevEndIndex = overlappedRanges.Last().Key;
                var currentStartIndex = current.Key - _workingWindowSize;

                if (currentStartIndex <= prevEndIndex)
                {
                    overlappedRanges.Add(current);
                }
                else
                {
                    var index = overlappedRanges.IndexOfMin(x => x.Value);
                    cleanResult.Add(overlappedRanges[index]);                  
                    overlappedRanges.Clear();

                    overlappedRanges.Add(current);
                }
                
            }

            return cleanResult;
        }

        public bool IsPresent(int index, out double rangeSwing)
        {
            var workingCandles = _candles.TakeRange(index - _workingWindowSize, index - 1).ToList();
           // var historyCandles = _candles.TakeRange(index - _historyWindowSize, index - 1).ToList();

            var workingStatistics = new CandlesStatisticsCalculator(workingCandles).Calculate();
            //var historyStatistics = new CandlesStatisticsCalculator(historyCandles).Calculate();
            CandlesStatistics historyStatistics = null;

            var currentCandle = _candles[index];
            var prevCandle = _candles[index - 1];

            var isFlatLowVolatilityRange = IsFlatLowVolatilityRange(currentCandle, workingCandles, workingStatistics, historyStatistics,  out rangeSwing);
            var isCurrentBigCandle = IsBitCandle(currentCandle, workingStatistics, historyStatistics);


            return isFlatLowVolatilityRange && isCurrentBigCandle;
        }

        private bool IsFlatLowVolatilityRange(Candle currentCandle, List<Candle> workingCandles, CandlesStatistics workingStatistics, CandlesStatistics historyStatistics, out double rangeSwing)
        {
            var unit = currentCandle.Get(PriceTypes.Body);

            var lowLevel = workingCandles.Min(x => x.Close);
            var highLevel = workingCandles.Max(x => x.Close);
            rangeSwing = highLevel - lowLevel;


            bool isSwingOk = rangeSwing < 45;
            return isSwingOk;

            var averageLevel = lowLevel + rangeSwing/2;
            var countAbove = workingCandles.Count(x => x.Close > averageLevel);
            var countBelow = workingCandles.Count(x => x.Close < averageLevel);

            var max = Math.Max(countAbove, countBelow);
            var min = Math.Min(countAbove, countBelow);
            var isFlatProportion = max <= 2 * min;

            return isFlatProportion;

            //var isUpAndCloseAboveRange = currentCandle.IsUp() && currentCandle.Close > highLevel;
            //var isDownAndCloseBelowRange = currentCandle.IsDown() && currentCandle.Close < lowLevel;

            //if (rangeSwing < 2*unit && (isUpAndCloseAboveRange || isDownAndCloseBelowRange))
            //    return true;

            return false;
        }

        private bool IsBitCandle(Candle currentCandle, CandlesStatistics workingStatistics, CandlesStatistics historyStatistics)
        {
            return true;
            var bodySize = currentCandle.Get(PriceTypes.Body);
            if (bodySize> 3*workingStatistics.Body.Percentile99)
            {
                //if (bodySize > historyStatistics.Body.Percentile97)
                {
                    return true;
                }
            }

            return false;
        }

    }
}