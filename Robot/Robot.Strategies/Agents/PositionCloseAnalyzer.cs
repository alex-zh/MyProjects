using System.Collections.Generic;
using Common;
using Common.Classes;
using Common.Classes.Analizers;
using Common.Classes.General;

namespace Robot.Strategies.Agents
{
    /// <summary>
    /// Analyzes position on conditions for closing
    /// </summary>
    public class PositionCloseAnalyzer
    {
        private List<Candle> _candles;
        private MovingsAnalizer _movingAnalizer;
        private DistributionParameters _upParameters;
        private DistributionParameters _downParameters;
        private CandlesStatistics _candlesStatistics;

        /// <summary>
        /// One or two candles crossing  fast and slow movings. 
        /// Size of candles is more than average, volume of candles is more than average
        /// </summary>
        public bool IsOneorTwoCandlesCrossingMovings(int currentIndex)
        {
            if (_candles.Count < 2) return false;
            if (_candlesStatistics == null) return false;

            var currentCandle = _candles[currentIndex];
            var prevCandle = _candles[currentIndex - 1];

            if (_movingAnalizer.IsFastAboveSlow())
            {
                if (currentCandle.Open > _movingAnalizer.GetFma(currentIndex) &&
                    currentCandle.Close +_candlesStatistics.Body.Median < _movingAnalizer.GetSma(currentIndex))
                {
                    if (currentCandle.Volume > 0*_candlesStatistics.Volume.Median &&
                        currentCandle.Get(PriceTypes.Body) > 1.25 * _candlesStatistics.Body.Median)
                    {
                        return true;
                    }
                }

                if (prevCandle.Open > _movingAnalizer.GetFma(currentIndex-1) &&
                    currentCandle.Close+_candlesStatistics.Body.Median < _movingAnalizer.GetSma(currentIndex))
                {
                    if (prevCandle.Volume + currentCandle.Volume > 0 * _candlesStatistics.Volume.Median &&
                        prevCandle.Get(PriceTypes.Body) + currentCandle.Get(PriceTypes.Body) > 2.5 * _candlesStatistics.Body.Median)
                        
                    {
                        return true;
                    }
                }
            }

            else if (_movingAnalizer.IsFastBelowSlow())
            {
                if (currentCandle.Open < _movingAnalizer.GetFma(currentIndex) &&
                    currentCandle.Close > _movingAnalizer.GetSma(currentIndex)+ _candlesStatistics.Body.Median)
                {
                    if (currentCandle.Volume > 0 * _candlesStatistics.Volume.Median &&
                        currentCandle.Get(PriceTypes.Body) > 1.25 * _candlesStatistics.Body.Median)
                    {
                        return true;
                    }
                }

                if (prevCandle.Open < _movingAnalizer.GetFma(currentIndex - 1) &&
                    currentCandle.Close > _movingAnalizer.GetSma(currentIndex)+ _candlesStatistics.Body.Median)
                {
                    if (prevCandle.Volume + currentCandle.Volume > 0 * _candlesStatistics.Volume.Median &&
                        prevCandle.Get(PriceTypes.Body) + currentCandle.Get(PriceTypes.Body) > 2.5 * _candlesStatistics.Body.Median)

                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Update(List<Candle> candles, MovingsAnalizer movingsAnalizer, DistributionParameters upParameters, DistributionParameters downParameters, CandlesStatistics candesStatistics)
        {
            _candles = candles;
            _movingAnalizer = movingsAnalizer;
            _upParameters = upParameters;
            _downParameters = downParameters;
            _candlesStatistics = candesStatistics;
        }

        public bool AreTwoClosedAboveOrBelowMoving(int currentIndex)
        {
            if (_candles.Count < 2) return false;
            if (_candlesStatistics == null) return false;

            var currentCandle = _candles[currentIndex];
            var prevCandle = _candles[currentIndex - 1];

            if (_movingAnalizer.IsFastAboveSlow())
            {
                if (currentCandle.Open < _movingAnalizer.GetSma(currentIndex) &&
                    currentCandle.Close < _movingAnalizer.GetSma(currentIndex) &&
                    prevCandle.Close  < _movingAnalizer.GetSma(currentIndex-1))
                {
                    return true;
                }
                
            }
            else if (_movingAnalizer.IsFastBelowSlow())
            {
                if (currentCandle.Open > _movingAnalizer.GetSma(currentIndex) &&
                     currentCandle.Close > _movingAnalizer.GetSma(currentIndex) &&
                     prevCandle.Close > _movingAnalizer.GetSma(currentIndex - 1))
                {
                    return true;
                }
            }

            return false;
        }

        public bool AreTwoCandlesClosedAboveOrBelowMoving(int index)
        {
            if (_candles.Count < 2) return false;
            if (_candlesStatistics == null) return false;

            var currentCandle = _candles[index];
            var prevCandle = _candles[index - 1];

            if (_movingAnalizer.IsFastAboveSlow())
            {
                if (currentCandle.Open < _movingAnalizer.GetSma(index) &&
                    currentCandle.Close < _movingAnalizer.GetSma(index) &&
                    currentCandle.Open > currentCandle.Close &&
                    prevCandle.Open < _movingAnalizer.GetSma(index - 1) &&
                    prevCandle.Close < _movingAnalizer.GetSma(index - 1) &&
                    prevCandle.Open> prevCandle.Close)
                {
                    return true;
                }

            }
            else if (_movingAnalizer.IsFastBelowSlow())
            {
                if (currentCandle.Open > _movingAnalizer.GetSma(index) &&
                     currentCandle.Close > _movingAnalizer.GetSma(index) &&
                     currentCandle.Close > currentCandle.Open &&
                     prevCandle.Open > _movingAnalizer.GetSma(index - 1) &&
                     prevCandle.Close > _movingAnalizer.GetSma(index - 1) &&
                     prevCandle.Close > prevCandle.Open)
                {
                    return true;
                }
            }

            return false;
        }

        public bool AreTwoCandlesClosedAboveMoving(int index)
        {
            if (_candles.Count < 2) return false;
            if (_candlesStatistics == null) return false;

            var currentCandle = _candles[index];
            var prevCandle = _candles[index - 1];

            if (currentCandle.Open > _movingAnalizer.GetSma(index) &&
                  currentCandle.Close > _movingAnalizer.GetSma(index) &&
                  currentCandle.Close > currentCandle.Open &&                  
                  prevCandle.Close > _movingAnalizer.GetSma(index - 1) &&
                  prevCandle.Close > prevCandle.Open)
            {
                return true;
            }
            

            return false;
        }

        public bool AreTwoCandlesClosedBelowMoving(int index)
        {
            if (_candles.Count < 2) return false;
            if (_candlesStatistics == null) return false;

            var currentCandle = _candles[index];
            var prevCandle = _candles[index - 1];

            if (currentCandle.Open < _movingAnalizer.GetSma(index) &&
                     currentCandle.Close < _movingAnalizer.GetSma(index) &&
                     currentCandle.Open > currentCandle.Close &&
                     prevCandle.Open < _movingAnalizer.GetSma(index - 1) &&
                     prevCandle.Close < _movingAnalizer.GetSma(index - 1) &&
                     prevCandle.Open > prevCandle.Close)
            {
                return true;
            }

            return false;
        }
    }
}