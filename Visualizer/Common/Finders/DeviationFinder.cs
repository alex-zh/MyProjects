using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Common.Classes.General;

namespace Common
{
    public class DeviationFinder
    {
        private readonly List<Candle> _candles;
        private readonly List<DatePrice> _averageLine;

        public DeviationFinder(IEnumerable<Candle> candles)
        {
            _candles = candles.ToList();

            var ema11List = _candles.Ema(11).ToList();
            var ema21List = _candles.Ema(21).ToList();

            _averageLine = ema11List.Select((t, i) => new DatePrice()
            {
                Date = t.Date,
                Value = (t.Value + ema21List[i].Value) / 2
            }).ToList();
        }

        public bool IsDeviated(Candle candle, double threshold)
        {
            var index = _candles.IndexOf(candle);
            return IsDeviated(index, threshold);
        }

        public bool IsDeviated(int index, double threshold)
        {
            var candle = _candles[index];
            var deviation = CalculateDeviation(candle, _averageLine[index].Value);

            return (deviation >= threshold);
        }

        //public IEnumerable<AbnormalDeviationPoint> FindAll()
        //{
        //    var result = new List<AbnormalDeviationPoint>();

        //    var standartDeviations = new List<AbnormalDeviationPoint>();

        //    for (int i = 0; i < _candles.Count; i++)
        //    {
        //        var deviation = CalculateStandartDeviation(i, _candles, _averageLine);

        //        standartDeviations.Add(new AbnormalDeviationPoint()
        //        {
        //            DateTime = _candles[i].DateTime,
        //            Deviation = deviation,
        //            MaPrice = _averageLine[i].Price,
        //            DeviationPercent = (deviation*100/_averageLine[i].Price)
        //        });
        //    }

        //    double minValue = standartDeviations.Where(x => x.DeviationPercent > 0).Min(x => x.DeviationPercent);
        //    double maxValue = standartDeviations.Max(x => x.DeviationPercent);

        //    for (int i = 41; i < candlesList.Count; i++)
        //    {
        //        var currentCandle = candlesList[i];
        //        var standartDeviationPercent = standartDeviations[i].DeviationPercent;

        //        var currentDeviation = CalculateDeviation(currentCandle, _averageLine[i]);
        //        var currentDeviationPercent = currentDeviation * 100 / _averageLine[i].Price;

        //        if (currentDeviationPercent > standartDeviationPercent && standartDeviationPercent < 0.2)
        //        {
        //            double weight = 1;

        //            //if (currentDeviation >= standartDeviation * 1.25)
        //            //    weight = 1.25;
        //            //if (currentDeviation >= standartDeviation*1.5)
        //            //    weight = 1.5;
        //            //if (currentDeviation >= standartDeviation * 1.75)
        //            //    weight = 1.75;
        //            //if (currentDeviation >= standartDeviation * 2)
        //            //    weight = 2;
        //            //if (currentDeviation >= standartDeviation * 2.25)
        //            //    weight = 2.25;
        //            //if (currentDeviation >= standartDeviation * 2.5)
        //            //    weight = 2.5;

        //            result.Add(new AbnormalDeviationPoint()
        //            {
        //                Index = i,
        //                DateTime = currentCandle.DateTime,
        //                CandlePrice = candlesList[i].Price,
        //                MaPrice = _averageLine[i].Price,
        //                Deviation = currentDeviation,
        //                Weight = weight
        //            });
        //        }             
        //    }

        //    return result;
        //}

        //private double CalculateStandartDeviation(int index, List<Candle> candlesList, List<DatePrice> averageLine)
        //{
        //    const int numberOfPoints = 20;
        //    if (index < numberOfPoints) return 0.0;

        //    var allDeviations = new List<DatePrice>();

        //    for (int i = index; i > Math.Max(0, index - numberOfPoints); i--)
        //    {
        //        allDeviations.Add(new DatePrice()
        //        {
        //            DateTime = candlesList[i].DateTime, 
        //            Price = CalculateDeviation(candlesList[i], averageLine[i].Price)
        //        });
        //    }

        //    allDeviations = allDeviations.OrderBy(x => x.Price).ToList();

        //    // var heighest50Percent = allDeviations.Skip(numberOfPoints*1/2);

        //    return allDeviations.Average(x => x.Price);
        //}

        private double CalculateDeviation(Candle candle, double meanPrice)
        {
            //return Math.Max(Math.Abs(meanPrice - candle.Low), Math.Abs(candle.High - meanPrice)) * 100 / meanPrice;
            return  (candle.High - meanPrice) * 100 / meanPrice;
        }
    }
}