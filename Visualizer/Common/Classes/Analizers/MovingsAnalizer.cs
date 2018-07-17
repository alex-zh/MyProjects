using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Caclulators;
using Common.Classes.General;
using Common.Classes.Logger;

namespace Common.Classes.Analizers
{
    public class MovingsAnalizer
    {
        private readonly int _fmaPeriod;
        private readonly int _smaPeriod;
        private readonly List<Candle> _candles;
        private List<DatePrice> _fma;
        private List<DatePrice> _sma;
        private List<DatePrice> _volume;
        private const int AtrRangeLength = 16; // должно быть достаточно большим, т.к если сильно увеличивается волатильность (на коротком интервале), фильтр резко усиливается
        private readonly Func<DateTime, bool> _isDateInValidRange = x => (x.Hour > 10 || x.Hour == 10 && x.Minute > 30) && (x.Hour < 22 || x.Hour == 22 && x.Minute < 30);
        private Atr _atr;
        private SignificantMovementAtr _significantMovementAtr;

        public MovingsAnalizer(List<Candle> candles, int fmaPeriod, int smaPeriod)
        {
            _candles = candles;
            _fmaPeriod = fmaPeriod;
            _smaPeriod = smaPeriod;

            Parallel.Invoke(
                () => _fma = _candles.Ema(_fmaPeriod, PriceTypes.Mean).ToList(),
                () => _sma = _candles.Ema(_smaPeriod, PriceTypes.Mean).ToList(),
                () => _volume = _candles.Ema(_fmaPeriod, PriceTypes.Volume).ToList(),
                () => _atr = new Atr(_candles.ToArray(), AtrRangeLength, _isDateInValidRange),
                () => _significantMovementAtr = new SignificantMovementAtr(_candles.ToArray(), AtrRangeLength)
            );
        }

        public double GetFma(int index)
        {
            return _fma[index].Value;
        }

        public double GetSma(int index)
        {
            return _sma[index].Value;
        }

        public double GetVolumeFma(int index)
        {
            return _volume[index].Value;
        }

        public bool DoesDirectionMatch(DateTime date, Direction direction, bool checkDifference = false)
        {
            //return true;
            var difference = 0.0;
            var candleIndex = _candles.GetIndexByDate(date);

            if (candleIndex > 0)
            {
                if (direction == Direction.Sell)
                {
                    difference = _fma[candleIndex].Value - _sma[candleIndex].Value;
                }
                else if (direction == Direction.Buy)
                {
                    difference = _sma[candleIndex].Value - _fma[candleIndex].Value;
                }

                if (checkDifference)
                {
                    return IsDifferenceInLegitimateRange(_atr.Calculate(candleIndex), difference);
                }
                else
                {
                    return difference > 0;
                }
            }
            return false;
        }

        private bool IsDifferenceInLegitimateRange(double atr, double difference)
        {
            /* Ограничивать максимальное отклонение нет большого смысла, оно будет отфильтровано дальше */
            return difference > 0.01 * atr; //&& difference < 0.5 * atr; //нужно подбирать параметры. 
        }

        public bool IsFastBelowSlow()
        {
            var index = _candles.Count - 1;
            return _fma[index].Value < _sma[index].Value;
        }

        public bool IsFastAboveSlow()
        {
            var index = _candles.Count - 1;
            return _fma[index].Value > _sma[index].Value;
        }

        public double GetDeviatedPrice(Direction currentDirection, int atrCount)
        {
            var index = _candles.Count - 1;
            var atr = atrCount * _significantMovementAtr.Calculate(index);

            if (currentDirection == Direction.Sell)
            {
                atr *= -1;
            }

            return (_sma[index].Value + _fma[index].Value) / 2 + atr;
        }

        public double GetDeviatedPrice(Direction currentDirection,
                                       DistributionParameters deviationParameters,
                                       int deviationFactor,
                                       int atrFactor = 3)
        {
            var index = _candles.Count - 1;
            var atr = _significantMovementAtr.Calculate(index);

            var deviation = deviationParameters.Median + deviationFactor * deviationParameters.MedianSigma;

            if (deviation < atr * atrFactor)
            {
                deviation = atr * atrFactor;
            }

            if (currentDirection == Direction.Sell)
            {
                deviation *= -1;
            }

            var price = _fma[index].Value + deviation;

            return PriceCorrector.GetPrice(price, currentDirection.GetOpposite());
        }

        private double GetMultiplierValue(double value, int A, int B, int a, int b)
        {
            if (value < a)
                return A;
            if (value > b)
                return B;

            return value * (B - A) / (b - a) + (A * b - B * a) / (b - a);
        }
    }
}