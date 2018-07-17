using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace IndicatorsLibrary
{
    /// <summary>
    /// Вычисляет волатильность
    /// 1. Вычисляем весь пройденный путь:
    /// Если текущая свеча была свечей роста (закрытие>открытия), то для этой свечи путь определяется как разница
    ///  а) закрытия 
    ///  b) минимальное из значений  [текущее открытие, закрытие предыдущей свечи].
    /// Если текущая свеча была свечей снижения (закрытие < открытия), то путь определяется как разница
    ///  а) максимальное из-значений [текущее открытие, закрытие предыдущей свечи]
    ///  b)цена закрытия
    /// 2. Вычисляем
    /// 
    ///  пройденное растояние, значение close для
    ///                                                       й свечи, минус значение open для свечи period свечей назад
    /// 
    /// Формула EMA
    /// EMA (t) = EMA(t-1) + alpha * (P(t)-EMA(t-1)), alpha - коэфициент, задающий степень убывания весов.>
    ///         <текуще></текуще>
    /// </summary>
    public class VolatilityCalculator
    {
        private Candle[] _candles;
        private int _period;
        private Dictionary<int, double> volatilityList = new Dictionary<int, double>();
        private Dictionary<int, double> emaList = new Dictionary<int, double>();
        private double emaAlpha = 0.5;

        public VolatilityCalculator(Candle[] candles, int period)
        {
            _candles = candles;
            _period = period;
            Init();
        }

        private void Init()
        {
            for (int i = 0; i < _candles.Length; i++)
            {
                volatilityList.Add(i, CalculateVolatility(i));
            }

            var minValue = volatilityList.Values.Min();
            var maxValue = volatilityList.Values.Max();

            for (int i = 0; i < volatilityList.Keys.Count; i++)
            {
                volatilityList[i] = Math.Round ((volatilityList[i] / (maxValue - minValue)) * 100);
            }

            for (int i = 0; i < volatilityList.Keys.Count; i++)
            {
                if (i == 0)
                {
                    emaList.Add(i, volatilityList[i]);
                    continue;
                }

                emaList[i] = emaList[i - 1] + emaAlpha * (volatilityList[i] - emaList[i - 1]);                
            }

            var emaMinValue = emaList.Values.Min();
            var emaMaxValue = emaList.Values.Max();

            for (int i = 0; i < emaList.Keys.Count; i++)
            {
                emaList[i] = Math.Round((emaList[i] / (emaMaxValue - emaMinValue)) * 100);
            }
        }

        public double this[int index]
        {
            get
            {
                return volatilityList[index];
            }
        }

        public double GetEma(int index)
        {            
           return emaList[index];            
        }

        private double CalculateVolatility(int candleIndex)
        {
            if (candleIndex - _period - 1 < 0 || candleIndex >= _candles.Length)
                return 0;

            //реальное перемещение
            var distance = CalculateDistance(candleIndex);

            //пройденный путь
            var travel = CalculateTravel(candleIndex);

            return 1 - (distance / travel);
        }

        private double CalculateTravel(int candleIndex)
        {
            var totalTravel = 0.0;

            for (int i = candleIndex - _period; i <= candleIndex; i++)
            {
                var startValue = GetStartValue(i);
                var endValue = _candles[i].Close;

                totalTravel += Math.Abs(endValue - startValue);
            }

            return totalTravel;
        }

        private double GetStartValue(int candleIndex)
        {
            var candle = _candles[candleIndex];            
            var previousCandle = _candles[candleIndex  - 1];

            var startValue = 0.0;

            startValue = candle.IsUp() ? Math.Min(candle.Open, previousCandle.Close) :
                                         Math.Max(candle.Open, previousCandle.Close);

            return startValue;
        }

        private double CalculateDistance(int candleIndex)
        {            
            var startValue = GetStartValue(candleIndex - _period);
            var endValue = _candles[candleIndex].Close;

            return Math.Abs(endValue - startValue);
        }
    }
}
