using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Common.Classes.General;

namespace Common
{
    /// <summary>
    /// Находит точки пересечения двух средних    
    /// Алгоритм:
    /// Берем разницу, между первой и второй средней. 
    /// Точки после, которых происходит смена знака считаем пересечением.
    ///  Алгоритм определения точки пересечения:
    ///  1. Если разница нулевая, но дальше не происходит смены знака, значит это еще не пересечение
    ///  2. Нулевая разница не может быть определена как абсолютный ноль, должен быть некоторый диапазон
    ///  3. Пересечение может быть резким, т.е происходит смена знака, при это разница не попадает в диапазон из п.2
    ///  4. Для разных активов, разных таймфреймов спред должен быть разный. Например, визуально для MoexSi часовика - это 0.06%, для пятиминут 0.03%, для RTSI часовика это 0.3%. 
    ///   Объясняется, тем что чем больший диапазон умещается на экране, тем визуально мувинги ближе. Оптимальная величина должна быть найдена с помощью оптимизации
    ///  5. Для определения разницы предлагается использовать процент от ATR, например, 0.15*ATR
    /// ! в стратегии с мувингами нужно предусмотреть, что если отрыв слишком велик, то входить в позицию нельзя. Нужно наоборот фиксировать прибыль!!!
    /// </summary>
    public class MovingsIntersectionFinder
    {
        private readonly Atr _atr;
        private readonly List<Candle> _candles;        
        private const int AtrRangeLength = 21;

        public MovingsIntersectionFinder(IEnumerable<Candle> candles)
        {
            _candles = candles.ToList();
            _atr = new Atr(candles.ToArray(), AtrRangeLength);
        }

        public IEnumerable<CrossingPoint> Find(int fmaPeriod, int smaPeriod)
        {
            var result = new List<CrossingPoint>();

            if (fmaPeriod >= smaPeriod)
            {
                throw new ArgumentException("Fma period is bigger than Sma period");
            }

            if (_candles.Count() <= smaPeriod)
            {
                return result;
            }

            var differences = new List<DatePrice>();

            var fma = _candles.Ema(fmaPeriod).ToList();
            var sma = _candles.Ema(smaPeriod).ToList();

            for (int i =  smaPeriod; i < _candles.Count(); i++)
            {                                
                differences.Add(new DatePrice
                {
                    Index = i,
                    Date = _candles[i].Date,
                    Value = (fma[i].Value - sma[i].Value),                    
                });                    
            }

            CrossingPoint crossingPointCandidate = null;

            for (int i = 1; i < differences.Count(); i++)
            {
                var diff = differences[i];
                var previousDiff = differences[i - 1];                

                if ((diff.Value < 0 && previousDiff.Value > 0) ||
                    (diff.Value > 0 && previousDiff.Value < 0))
                {
                    var direction = (diff.Value < 0 && previousDiff.Value > 0) ? Direction.Sell : Direction.Buy;

                    if (crossingPointCandidate != null )
                    {
                        if (direction != crossingPointCandidate.Direction)
                        {
                            crossingPointCandidate = null;
                        }
                        else
                        {
                            throw new Exception("Second crossing point is detected with the same direction");
                        }
                    }
                    else
                    {
                        crossingPointCandidate = new CrossingPoint() { Index = diff.Index, Direction = direction, Date = diff.Date};    
                    }                    
                }

                if (crossingPointCandidate != null)
                {
                    if (Math.Abs(diff.Value) > GetDifferenceThreshold(i))
                    {
                        //точкой пересечения будет точка подтверждения
                        crossingPointCandidate.Index = diff.Index;
                        crossingPointCandidate.Date = diff.Date;
                        
                        result.Add(crossingPointCandidate);

                        crossingPointCandidate = null;
                    }
                }
            }
          
            return result;
        }

        private double GetDifferenceThreshold(int i)
        {
            return _atr.Calculate(i) * 0.05;
        }    
    }
}
