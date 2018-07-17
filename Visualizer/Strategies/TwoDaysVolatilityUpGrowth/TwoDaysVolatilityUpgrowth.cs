using System;
using Common;
using Common.Classes;

namespace Strategies.TwoDaysVolatilityUpGrowth
{
    /// <summary>
    /// Если рынок за 2 x N свечей вырос больше чем на Vd x N, интересно посмотреть будет ли статистический сдвиг
    /// т.е например если за 4 свечи рынок вырос больше чем на 2 дневные волатильности
    /// </summary>
    public class TwoDaysVolatilityUpgrowth : IStrategy
    {
        private const int N = 2;
        private int _volatilityCalcRange = N * 4;
        private int _lookingForwardCandlesNumber;
        private int _volatilityToLengthFactor;
        private double _volatilityCount;
        private Candle[] _candles;
        private Atr _atr;
        private StatisticsHolder _statisticsHolder;


        public TwoDaysVolatilityUpgrowth(Candle[] candles, int volatilityToLengthFactor, double volatilityCount)
            : this(candles, N * 4, 10, volatilityToLengthFactor, volatilityCount)
        {

        }

        public TwoDaysVolatilityUpgrowth(Candle[] candles, int volatilityCalcRange, int lookingForwardCandlesNumber, int volatilityToLengthFactor, double volatilityCount)
        {
            _candles = candles;
            _volatilityCalcRange = volatilityCalcRange;
            _atr = new Atr(_candles, _volatilityCalcRange);
            _lookingForwardCandlesNumber = lookingForwardCandlesNumber;
            _volatilityToLengthFactor = volatilityToLengthFactor;
            _volatilityCount = volatilityCount;

            _statisticsHolder = new StatisticsHolder();
        }

        public StatisticsHolder Run()
        {
            for (int i = _volatilityCalcRange - 1; i < _candles.Length; i++)
            {
                var dayVol= _atr.Calculate(i);
                var startCandle = _candles[i];

                for (int j = i + 1; j < Math.Min((i + 1) +  _volatilityToLengthFactor, _candles.Length - 1); j++) // перебираем _volatilityToLengthFactor*N свечей
                {
                    var currentCandle = _candles[j];

                    if (currentCandle.Close >= startCandle.Close + dayVol * _volatilityCount)
                    { 
                        //center condition is true

                        CollectStatistics(i, j);
                        break;
                    }
                }
            }

            return _statisticsHolder;
        }

        private void CollectStatistics(int startCandleIndex, int breakthroughCandleIndex)
        {
            if (breakthroughCandleIndex + _lookingForwardCandlesNumber > _candles.Length) return; // не добавляем для последних свечей

            var startCandle = _candles[startCandleIndex];
            var breakthroughCandle = _candles[breakthroughCandleIndex];

            var strategyRealization = new StrategyRealization();

            strategyRealization.StartCandle = startCandle;
            strategyRealization.CenterConditionCandle = breakthroughCandle;

            for (int i = breakthroughCandleIndex + 1; i < breakthroughCandleIndex + _lookingForwardCandlesNumber; i++)
            {
                var candle = _candles[i];

                strategyRealization.Offsets.Add((candle.Close - breakthroughCandle.Close) / breakthroughCandle.Close);
            }

            _statisticsHolder.Add(strategyRealization);
        }
    }
}
