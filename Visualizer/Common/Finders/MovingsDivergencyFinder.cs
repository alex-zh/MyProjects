using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Classes;
using Common.Classes.General;

namespace Common.Finders
{
    /// <summary>
    /// Находит точки, где две среднии начинают расходится. 
    /// При этом до этого момента они должны были находится достаточно близко
    /// Алгоритм:
    /// 1. Берем разницу dy(candleIndex), между первой и второй средней в текущей момент и N свечей назад dy(candleIndex-N)
    /// Разница должна быть больше определенной величины |dy(candleIndex) - dy(candleIndex-N)| > % ATR (candleIndex)
    /// 2. Разница K (K>N) свечей назад должна быть достаточно малой, то есть наблюдалась некоторая консолидация
    /// </summary>
    public class MovingsDivergencyFinder
    {
        private readonly int _movingsProximityLookupPeriod = 10; // K
        private readonly int _fmaPeriod;
        private readonly int _smaPeriod;
        private Atr _atr;
        private Atr _longAtr;
        private readonly List<Candle> _candles;
        private List<DatePrice> _fma;
        private List<DatePrice> _sma;
        private const int LongAtrRangeLength = 16 * 5; // 
        private const int AtrRangeLength = 16 * 2; // должно быть достаточно большим, т.к если сильно увеличивается волатильность (на коротком интервале), фильтр резко усиливается
        private readonly Func<DateTime, bool> _isDateInValidRange = x => (x.Hour > 10 || x.Hour == 10 && x.Minute > 30) && (x.Hour < 23 || x.Hour == 23 && x.Minute<30);
        private List<DatePrice> _parentFma;
        private List<DatePrice> _parentSma;
        private List<DatePrice> _smoothedDifferenceFmaMinusSma;

        public MovingsDivergencyFinder(IEnumerable<Candle> candles, int fmaPeriod, int smaPeriod)
        {
            if (fmaPeriod >= smaPeriod)
                throw new ArgumentException("Fma period is bigger than Sma period");

            _candles = candles.ToList();

            if (_candles.Count() <= smaPeriod)
                throw new ArgumentException("Sma period is bigger than candles count");

            _fmaPeriod = fmaPeriod;
            _smaPeriod = smaPeriod;

            Parallel.Invoke(
               () => _fma = _candles.Ema(_fmaPeriod, PriceTypes.Mean, true).ToList(),
               () => _sma = _candles.Ema(_smaPeriod, PriceTypes.Mean, true).ToList(),
               () => _parentFma = _candles.Ema(_fmaPeriod, 12).ToList(),
               () => _parentSma = _candles.Ema(_smaPeriod, 12).ToList(),
               () => _atr = new Atr(_candles.ToArray(), AtrRangeLength, _isDateInValidRange),
               () => _longAtr = new Atr(_candles.ToArray(), LongAtrRangeLength, _isDateInValidRange)
            );

            _smoothedDifferenceFmaMinusSma = GetSmoothedDifferenceFmaMinusSma(5);
        }

        public double GetLongAtr(int index)
        {
            return _longAtr.Calculate(index);
        }

        private List<DatePrice> GetSmoothedDifferenceFmaMinusSma(int smoothingPeriod)
        {
            var difference = _fma.Select((fmaItem, i) => new DatePrice { Date = fmaItem.Date, 
                                                                         Value = fmaItem.Value - _sma[i].Value});
            return difference.Ema(smoothingPeriod).ToList();
        }

        public IEnumerable<DivergencyPoint> Find()
        {
            DivergencyPoint point;

            var result = new List<DivergencyPoint>();

            for (int i = _movingsProximityLookupPeriod; i < _candles.Count(); i++)
            {
                if (IsDivergencyPoint(i, out point))
                {
                    result.Add(point);
                }
            }

            return result;
        }

        public bool IsTimeInValidRange(DateTime date)
        {
            return _isDateInValidRange(date);
        }

        public bool IsBrentDivergencyPoint(DateTime date, out DivergencyPoint point)
        {
            point = null;

            var i = _candles.GetIndexByDate(date);

            /* в свечах может быть пропуски, поэтому есть вероятность не найти свечу по индексу*/
            if (i < 0) return false;

            var candle = _candles[i];

            if (_isDateInValidRange(candle.Date) == false)
            {
                return false;
            }

            Direction? direction = null;

            if (candle.Close > Math.Max(_fma[i].Value, _sma[i].Value) &&
                candle.Get(PriceTypes.Mean) > Math.Max(_fma[i].Value, _sma[i].Value))
            {
                direction = Direction.Buy;
            }
            else if (candle.Close < Math.Min(_fma[i].Value, _sma[i].Value) &&
                     candle.Get(PriceTypes.Mean) < Math.Min(_fma[i].Value, _sma[i].Value))
            {
                direction = Direction.Sell;
            }

            if ((direction == Direction.Sell && _fma[i].Value > _sma[i].Value) ||
                direction == Direction.Buy && _fma[i].Value < _sma[i].Value)
            {
                return false;
            }

            if (direction.HasValue == false) return false;

            var difference = CalculateDifference(i, direction.Value);

            var atr = _atr.Calculate(i);
            //if (IsDifferenceInLegitimateRange(atr, difference))
            {
               //if (IsPointExtremelyFarFromMoving(_candles[candleIndex], _fma[candleIndex].Value, _sma[candleIndex].Value, atr, direction.Value) == false)
                {
                    //if (IsPointCloseCorrect(_candles[candleIndex], _fma[candleIndex].Value, _sma[candleIndex].Value, direction.Value))
                    {
                        point = new DivergencyPoint
                        {
                            Index = i,
                            Atr = atr,
                            Candle = _candles[i],
                            Date = _candles[i].Date,
                            Direction = direction.Value,
                            FmaPrice = _fma[i].Value,
                            SmaPrice = _sma[i].Value
                        };

                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsDivergencyPoint(int i, out DivergencyPoint point)
        {
            point = null;

            var candle = _candles[i];

            if (_isDateInValidRange(candle.Date) == false)
            {
                return false;
            }

            Direction? direction = null;

            if (candle.Close > Math.Max(_fma[i].Value, _sma[i].Value) &&
                candle.Get(PriceTypes.Mean) > Math.Max(_fma[i].Value, _sma[i].Value))
            {
                direction = Direction.Buy;
            }
            else if (candle.Close < Math.Min(_fma[i].Value, _sma[i].Value) &&
                     candle.Get(PriceTypes.Mean) < Math.Min(_fma[i].Value, _sma[i].Value))
            {
                direction = Direction.Sell;
            }

            if ((direction == Direction.Sell && _fma[i].Value > _sma[i].Value) ||
                direction == Direction.Buy && _fma[i].Value < _sma[i].Value)
            {
                return false;
            }

            if (direction.HasValue == false) return false;

            var difference = CalculateDifference(i, direction.Value);

            var atr = _atr.Calculate(i);

            if (IsDifferenceInLegitimateRange(atr, difference) == false && IsDifferenceBetweenMovingsIsBig(i, atr) == false)
            {
                if (IsDifferenceInLegitimateRange(atr, difference) == false)
                {
                    return false;
                }
            }

            if (IsPointExtremelyFarFromMoving(_candles[i], _fma[i].Value, _sma[i].Value, atr, direction.Value))
            {
                return false;
            }

            if (IsPointCloseCorrect(_candles[i], _fma[i].Value, _sma[i].Value, direction.Value) == false)
            {
                return false;
            }

            point = new DivergencyPoint
            {
                Index = i,
                Atr = atr,
                Candle = _candles[i],
                Date = _candles[i].Date,
                Direction = direction.Value,
                FmaPrice = _fma[i].Value,
                SmaPrice = _sma[i].Value
            };

            return true;
        }

        private bool IsDifferenceBetweenMovingsIsBig(int i, double atr)
        {
            var difference = Math.Abs(_fma[i].Value - _sma[i].Value);

            return difference > 0.5 * atr;
        }

        public bool AreTwoLastCandlesAboveMovings(int candleIndex)
        {
            var candle = _candles[candleIndex];
            var previousCandle = _candles[candleIndex - 1];

            var maxMovingsValue = Math.Max(_fma[candleIndex].Value, _sma[candleIndex].Value);
            var fastMovingPlusAtr = _fma[candleIndex].Value + 2 * _atr.Calculate(candleIndex);

            var movingValue = Math.Min(maxMovingsValue, fastMovingPlusAtr);

            var previousMaxMovingsValue = Math.Max(_fma[candleIndex - 1].Value, _sma[candleIndex - 1].Value);
            var previousFastMovingPlusAtr = _fma[candleIndex - 1].Value + 2 * _atr.Calculate(candleIndex - 1);

            var movingPreviousValue = Math.Min(previousMaxMovingsValue, previousFastMovingPlusAtr);

            //var movingValue = Math.Max(_fma[candleIndex].Value, _sma[candleIndex].Value);
            
            //var movingPreviousValue = Math.Max(_fma[candleIndex - 1].Value, _sma[candleIndex - 1].Value);

            return (candle.Close > movingValue) &&
                   (candle.Get(PriceTypes.Mean) > movingValue) && (previousCandle.Close > movingPreviousValue);
        }

        public bool AreTwoLastCandlesBelowMovings(int i)
        {
            var candle = _candles[i];
            var previousCandle = _candles[i - 1];

            var minMovingsValue = Math.Min(_fma[i].Value, _sma[i].Value);
            var fastMovingMinusAtr = _fma[i].Value - 2 * _atr.Calculate(i);

            var movingValue = Math.Max(minMovingsValue, fastMovingMinusAtr);

            var previousMinMovingsValue = Math.Min(_fma[i - 1].Value, _sma[i - 1].Value);
            var previousFastMovingMinusAtr = _fma[i - 1].Value -2 * _atr.Calculate(i - 1);

            var previousMovingValue = Math.Max(previousMinMovingsValue, previousFastMovingMinusAtr);

            //var movingValue = Math.Min(_fma[i].Value, _sma[i].Value);

            //var previousMovingValue = Math.Min(_fma[i - 1].Value, _sma[i - 1].Value);

            return (candle.Close < movingValue) &&
                   (candle.Get(PriceTypes.Mean) < movingValue) && (previousCandle.Close < previousMovingValue);
        }

        private bool IsPointCloseCorrect(Candle candle, double fmaValue, double smaValue, Direction direction)
        {
            if (direction == Direction.Sell)
            {
                return candle.Close < Math.Min(fmaValue, smaValue) && candle.Get(PriceTypes.Mean) < Math.Min(fmaValue, smaValue);
            }
            else if (direction == Direction.Buy)
            {
                return candle.Close > Math.Max(fmaValue, smaValue) && candle.Get(PriceTypes.Mean) > Math.Max(fmaValue, smaValue);
            }

            return false;
        }

        private bool IsPointExtremelyFarFromMoving(Candle candle, double fmaValue, double smaValue, double atr, Direction direction)
        {
            return false;
            if (direction == Direction.Buy)
            {
                if (Math.Max(candle.Close, candle.Open) > (fmaValue + smaValue) / 2 + 2 * atr)
                    return true;
            }

            if (direction == Direction.Sell)
            {
                if (Math.Min(candle.Close, candle.Open) < (fmaValue + smaValue) / 2 - 2 * atr)
                    return true;
            }

            return false;
        }

        private bool IsPointAfterSignificantMovement(Candle candle, int candleIndex, int lookBackPeriod, Direction direction)
        {        
            if (direction == Direction.Buy)
            {
                var lowValue = _candles.IndexRange(candleIndex - lookBackPeriod, candleIndex).Min(x => x.Get(PriceTypes.Mean));
                var currentValue = candle.High;
                /* разница между текущим положением и положением позади превышает 0.5 % */
                //if ((currentValue - lowValue) * 100 / lowValue > 0.75)
                  if ((currentValue - lowValue) > 5*_atr.Calculate(candleIndex))
                    return true;

                  lowValue = _candles.IndexRange(candleIndex - lookBackPeriod / 3, candleIndex).Min(x => x.Get(PriceTypes.Mean));
                  currentValue = candle.High;
                  /* разница между текущим положением и положением позади превышает 0.5 % */
                  //if ((currentValue - lowValue) * 100 / lowValue > 0.75)
                  if ((currentValue - lowValue) > 3 * _atr.Calculate(candleIndex))
                      return true;
            }

            if (direction == Direction.Sell)
            {
                var highValue = _candles.IndexRange(candleIndex - lookBackPeriod, candleIndex).Max(x => x.Get(PriceTypes.Mean));
                var currentValue = candle.Low;
                /* разница между текущим положением и положением позади превышает 0.5 % */
                //if ((highValue - currentValue) * 100 / currentValue > 0.75)
                  if ((highValue - currentValue)  > 5*_atr.Calculate(candleIndex))
                    return true;

                  highValue = _candles.IndexRange(candleIndex - lookBackPeriod/3, candleIndex).Max(x => x.Get(PriceTypes.Mean));
                  currentValue = candle.Low;
                  /* разница между текущим положением и положением позади превышает 0.5 % */
                  //if ((highValue - currentValue) * 100 / currentValue > 0.75)
                  if ((highValue - currentValue) > 3 * _atr.Calculate(candleIndex))
                      return true;
            }

            return false;
        }

        private bool IsDifferenceInLegitimateRange(double atr, double difference)
        {
            /* Ограничивать максимальное отклонение нет большого смысла, оно будет отфильтровано дальше */
            return difference > 0.01 * atr; //&& difference < 0.5 * atr; //нужно подбирать параметры. 
        }

        private double CalculateDifference(int i, Direction direction)
        {
            var differenceNow = _fma[i].Value - _sma[i].Value;
            var differenceBefore5 = _smoothedDifferenceFmaMinusSma[i - 3].Value;
            //var differenceBefore10 = _smoothedDifferenceFmaMinusSma[candleIndex - 6].Value;

            if (direction == Direction.Buy)
            {
                if (differenceNow > differenceBefore5 )
                {
                    return Math.Abs(differenceNow - differenceBefore5);
                }
            }
            else if (direction == Direction.Sell)
            {
                if (differenceNow < differenceBefore5)
                {
                    return Math.Abs(differenceNow - differenceBefore5);
                }
            }

            return 0;
        }

        public bool IsBigCandleCrossingFastMovingDown(int index)
        {
            var candle = _candles[index];
            var atr = _atr.Calculate(index);

            if (Math.Abs(candle.Close - candle.Open) > atr * 0.75)
            {
                if (_fma[index].Value - candle.Close > atr * 0.3)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsBigCandleCrossingFastMovingUp(int index)
        {
            var candle = _candles[index];
            var atr = _atr.Calculate(index);

            if (Math.Abs(candle.Close - candle.Open) > atr * 0.75)
            {
                if (candle.Close - _fma[index].Value > atr * 0.3)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsReversalPoint(int index, out DivergencyPoint point)
        {            
            point = null;
            return false;

            var candle = _candles[index];
            var atr = _atr.Calculate(index);

            if (_fma[index].Value > _sma[index].Value)
            {
                var diff = (candle.Close - candle.Open);
                if (diff > atr * 2 && diff < atr * 4)
                {
                    point = new DivergencyPoint() { Atr = atr, Candle = candle, Direction = Direction.Buy };
                    return true;
                }
            }
            else if (_fma[index].Value < _sma[index].Value)
            {
                var diff = (candle.Open - candle.Close);
                if (diff > atr * 2 && diff < atr * 4)
                {
                    point = new DivergencyPoint() { Atr = atr, Candle = candle, Direction = Direction.Sell };
                    return true;
                }

            }

            return false;
        }

        /// <summary>
        /// Для движения вверх, размер свечи увеличиваем на размер тени внизу и уменьшаем на размер тени вверху
        /// </summary>
        public bool IsOneBigCandleCrossingFastMovingUp(int index)
        {
            var candle = _candles[index];
            var atr = _atr.Calculate(index);

            if (candle.Close> _fma[index].Value)
            {
                var diff = (candle.Close - candle.Open) + (candle.Open - candle.Low) - (candle.High - candle.Close);
                if (diff > atr * 0.25)
                {                    
                    return true;
                }
            }            

            return false;
            
        }

        public bool IsOneBigCandleCrossingFastMovingDown(int index)
        {
            var candle = _candles[index];
            var atr = _atr.Calculate(index);

            if (candle.Close < _fma[index].Value)
            {
                var diff = (candle.Open - candle.Close) + (candle.High - candle.Open) - (candle.Close - candle.Low);
                if (diff > atr * 0.25)
                {
                    return true;
                }
            }

            return false;

        }

        public bool IsParentDirectionMatch(int index, Direction direction)
        {
            if (direction == Direction.Buy)
            {
                return _parentFma[index].Value > _parentSma[index].Value;
            }
            else
            {
                return _parentFma[index].Value < _parentSma[index].Value;
            }
        }

        public bool IsCandleBelowMovingsAndThreeNextBelowSlowMoving(int index)
        {
            Candle candle;

            for (int i = index; i > Math.Max(index - 3, 0); i--)
            {
                candle = _candles[i];
                if (candle.Close >= _fma[i].Value)
                {
                    return false;
                }
            }
            
            candle = _candles[index - 3];

            if (candle.Close < Math.Min(_fma[index - 3].Value, _sma[index - 3].Value) &&
                candle.Open > _sma[index-3].Value)
            {
                return true;
            }

            return false;
            
        }

        public bool IsCandleAboveMovingsAndThreeNextAboveSlowMoving(int index)
        {
            Candle candle;

            for (int i = index; i > Math.Max(index - 3, 0); i--)
            {
                candle = _candles[i];
                if (candle.Close <= _fma[i].Value)
                {
                    return false;
                }
            }

            candle = _candles[index - 3];

            if (candle.Close > Math.Max(_fma[index - 3].Value, _sma[index - 3].Value) &&
                candle.Open < _sma[index - 3].Value)
            {
                return true;
            }

            return false;

        }

        /// <summary>
        /// Большая белая свеча после серии больших черных свечей
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsBigWhiteAfterBigBlacks(int index)
        {           
            var candle = _candles[index];
            var previousCandle = _candles[index - 1];

            var open = new List<double>()
            {
                _candles[index - 2].Open,
                _candles[index - 3].Open,
                _candles[index - 4].Open,
                _candles[index - 5].Open
            }.Max();
            
            var atr = _atr.Calculate(index);

            if (candle.Close - candle.Low > atr * 0.75)
            {
                if (previousCandle.Open - previousCandle.Low > atr * 0.75)
                {
                    if (open - previousCandle.Low > 2 * atr)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsBigWhite(int index, List<IntersectionSegment> segments)
        {
            var candle = _candles[index];

            if (segments.Any() == false) return false;

            var atr = new SimpleAtr(_candles.TakeRange(index - 16, index).ToArray()).Calculate();

            var lastSegment = segments.Last();

            var time = lastSegment.TimeSpan;
            var movement = Math.Abs(lastSegment.Movement);

            if (movement > 10 * atr && movement / time > 9.5)
            {
                if (candle.Close - candle.Open > atr)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
