using System.Collections.Generic;
using Common.Classes;
using Common.Classes.General;

namespace Core
{
    /// <summary>
    /// Определяем тренд, как 'CountOfTrendSteps' последовательных понижений.
    /// </summary>
    public class TrendUsingATR
    {
        private const int CountOfTrendSteps = 5;
        private Candle[] _candles;
        private int _atrPeriod;

        public TrendUsingATR(Candle[] candles, int atrPeriod)
        {
            _candles = candles;
            _atrPeriod = atrPeriod;
        }

        public List<int> FoundIndices
        {
            get;
            private set;
        }

        public void Calculate()
        {
            var result = new List<int>();

            for (int i = _atrPeriod; i < _candles.Length; i++)
            {
                var currentCandle = _candles[i];

                double atr = new VolatilityCalculator(_candles).Get(i);
                double atr_0_5 = atr * 0.5;
                double atr_1_5 = atr * 1.5;

                var count = 0;
                List<int> indices = new List<int>();

                for (int j = i + 1; j < _candles.Length; j++)
                {
                    var derivative = GetDerivative(_candles[j]);

                    if (derivative  > GetDerivative(currentCandle) - atr_1_5 &&
                        derivative  < GetDerivative(currentCandle) + atr_1_5)
                    {
                        indices.Add(j);

                        if (derivative < GetDerivative(currentCandle) - atr_0_5)
                        {
                            count++;
                        }
                    }
                    else
                    {
                        if (count > CountOfTrendSteps)
                        {
                            result.AddRange(indices);
                        }
                        break;
                    }
                }
            }

            FoundIndices = result;
        }

        private double GetDerivative(Candle candle)
        {
            return (candle.Open + candle.Close) / 2;
        }

    }
}
