using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Common.Classes.General;

namespace Common
{
    /// <summary>
    /// Находит сильные отклонения текущей цены от средней
    /// ???? Возможно для отклонения вверх и вниз должны быть разные пороговые значение 
    /// </summary>
    public class AbnormalDeviationFinder
    {
        private readonly double _deviationThreshold;

        public AbnormalDeviationFinder(double deviationThreshold)
        {
            _deviationThreshold = deviationThreshold;
        }

        public IEnumerable<AbnormalDeviationPoint> Find(IEnumerable<Candle> candles, IEnumerable<DatePrice> maSeries)
        {
            var candlesList = candles.ToList();
            var maList = maSeries.ToList();

            var result = new List<AbnormalDeviationPoint>();

            for (int i = 0; i <  Math.Min(candlesList.Count, maList.Count); i++)
            {                
                var currentCandle = candlesList[i];
                var movingAveragePoint = maList[i];

                if (IsAbnormallyDeviated(currentCandle, movingAveragePoint))
                {
                    result.Add(new AbnormalDeviationPoint()
                    {
                        Index = i,
                        Date = currentCandle.Date,
                        CandlePrice = candlesList[i].Price,
                        MaPrice = maList[i].Value,
                        Deviation = Math.Abs(candlesList[i].Price - maList[i].Value)
                    });
                }               
            }

            return result;
        }

        private bool IsAbnormallyDeviated(Candle candle, DatePrice maPoint)
        {
            //пока делаем по закрытию, потом нужно будет сделать по текущей цене!!!
            //
            var deviation = Math.Abs(candle.Close - maPoint.Value) * 100 / maPoint.Value;
            if (deviation > _deviationThreshold)
            {
                return true;
            }

            return false;
        }
    }
}
