using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;

namespace Common.Caclulators
{
    /// <summary>
    /// 'Whipsaw'
    /// A condition where a security's price heads in one direction, 
    /// but then is followed quickly by a movement in the opposite direction. 
    /// http://www.investopedia.com/terms/w/whipsaw.asp#ixzz3iyZ145ux 
    /// </summary>
    /// <remarks>
    /// windowLength is length of window inside which we count 'whipsaw' movements, for example 60 candles
    /// whipsawWindowLength is length to find 'whipsaw' movement, for example 5 candles
    /// </remarks>
    public class WhipsawCalculator
    {
        private readonly List<Candle> _candles;
        private readonly int _windowLength;
        private List<DatePrice> _fma;
        private List<DatePrice> _sma;
        private Atr _atr;


        public WhipsawCalculator(IEnumerable<Candle> candles, int fmaPeriod, int smaPeriod, int windowLength)
        {
            _candles = candles.ToList();
            _windowLength = windowLength;
            _fma = _candles.Ema(fmaPeriod, PriceTypes.Mean).ToList();
            _sma = _candles.Ema(smaPeriod, PriceTypes.Mean).ToList();
            _atr = new Atr(_candles.ToArray(), smaPeriod);
        }

        public double Calculate()
        {
            var whipsawCount = 0;
            var startIndex = _candles.Count() - 1;

            for (var j = startIndex; j > Math.Max(1, startIndex - _windowLength); j--)
            {
                if (IsWhipsaw(j))
                {
                    whipsawCount++;
                }
            }

            return whipsawCount;
        }

        private bool IsWhipsaw(int candleIndex)
        {
            var candle = _candles[candleIndex];
            var previousCandle = _candles[candleIndex - 1];
            var previousCandle2 = _candles[candleIndex - 2];

            var fmaValue = _fma[candleIndex].Value;
            var smaValue = _sma[candleIndex].Value;

            var minValue = Math.Min(fmaValue, smaValue);
            var maxValue = Math.Max(fmaValue, smaValue);

            var previousMinValue = Math.Min(_fma[candleIndex - 1].Value, _sma[candleIndex - 1].Value);
            var previousMaxValue = Math.Max(_fma[candleIndex - 1].Value, _sma[candleIndex - 1].Value);

            var previousMinValue2 = Math.Min(_fma[candleIndex - 2].Value, _sma[candleIndex - 2].Value);
            var previousMaxValue2 = Math.Max(_fma[candleIndex - 2].Value, _sma[candleIndex - 2].Value);

            var theshould = 0; GetThreshold(candleIndex);

            if (candle.Close > maxValue + theshould &&
               Math.Min( previousCandle.Close, previousCandle.Open) < previousMinValue - theshould &&
               Math.Min(previousCandle.Close, previousCandle2.Open) < previousMinValue2 - theshould)
            {
                return true;
            }

            if (candle.Close < minValue - theshould &&
                Math.Max(previousCandle.Close, previousCandle.Open) > previousMaxValue + theshould &&
                 Math.Max(previousCandle2.Close, previousCandle2.Open) > previousMaxValue2 + theshould)
            {
                return true;
            }

            return false;
        }

        private double GetThreshold(int i)
        {
            return _atr.Calculate(i) * 0.05;
        }
    }
}
