using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using Scatterplot.Core.Classes;

namespace RegessionAnalisys.DataPreparers
{
    /* 
       Готовит данные для построения регрессиии Usd - Brent.
       Данные генерятся для одного дня
       10:30 - 18:30 первая категория 
       19:30 - 23:30 вторая категория
       Точки каждые 30 минут, берем среднее значение 5 вперед 5 назад
     */
    public class BrentUsdDataSeveralDaysAggregationPreparer
    {
        private string _exportFilePath;
                
        public BrentUsdDataSeveralDaysAggregationPreparer()
        {
            _exportFilePath = "C:\\TestData\\Results";

            var usdDataFilePath = "C:\\TestData\\Input\\SPFB.Si_150901_151026.txt";
            var brentDataFilePath = "C:\\TestData\\Input\\SPFB.BR_150901_151026.txt";
            
            var usdCandles = new FileLoader().Load(usdDataFilePath);
            var brentCandles = new FileLoader().Load(brentDataFilePath);
            
            var points = usdCandles.Join(brentCandles, x => x.Date, y => y.Date, (x, y) => new BrentUsdDataPoint{ Date = x.Date, UsdCandle = x, BrentCandle = y });

            var pointsByDate = points.GroupBy(x => x.Date.Date).ToList();

            for (int i = 5; i < pointsByDate.Count(); i++)
            {
                var pointsForDays = new List<BrentUsdDataPoint>();

                pointsForDays.AddRange(pointsByDate.ToList()[i - 5]);
                pointsForDays.AddRange(pointsByDate.ToList()[i - 4]);
                pointsForDays.AddRange(pointsByDate.ToList()[i - 3]);
                pointsForDays.AddRange(pointsByDate.ToList()[i - 2]);
                pointsForDays.AddRange(pointsByDate.ToList()[i - 1]);
                pointsForDays.AddRange(pointsByDate.ToList()[i]);

                var pairs = BuildPairs(pointsForDays);

                if (pairs.Any())
                {
                    Export(GenerateFileName(pointsByDate[i].Key), pairs);
                }
            }
        }

        private string GenerateFileName(DateTime dateTime)
        {
            return dateTime.Day.ToString("00") + "_" + dateTime.Month.ToString("00") + "_" + dateTime.Year;
        }

        private List<BrentUsdPair> BuildPairs(List<BrentUsdDataPoint> points)
        {
            var result = new List<BrentUsdPair>();

            points = points.Where(x => (x.Date.Hour >= 10 && x.Date.Hour < 19)).OrderBy(x => x.Date).ToList();

            if (points.Any() == false)
            {
                return result;
            }

            for (var i = 5; i < points.Count() - 5; i += 30)
            {
                var p = points[i];

                var currentIndex = i;
                var pointsRange = points.Where((x, index) => index > currentIndex - 5 && index < currentIndex + 5).ToArray();
                var usdAverage = pointsRange.Select(x => x.UsdCandle.GetPrice(PriceTypes.Mean)).Average();
                var brentAverage = pointsRange.Select(x => x.BrentCandle.GetPrice(PriceTypes.Mean)).Average();
                var group = (i < (points.Count() *3) / 4 ) ? 1 : 2;

                result.Add(new BrentUsdPair
                {
                    Date = p.Date,
                    UsdPrice = usdAverage,
                    BrentPrice = brentAverage,
                    Group = group
                });                               
            }            

            return result;
        }

        private void Export(string filePath, IEnumerable<BrentUsdPair> pairs)
        {
            using (var streamWriter = new StreamWriter(_exportFilePath + "\\" + filePath + ".txt"))
            {
                streamWriter.WriteLine("Br\tSi\tGroup\tSubGroup\tDate");    

                foreach (var p in pairs)
                {
                    streamWriter.WriteLine(p.BrentPrice + "\t" + p.UsdPrice + "\t" + p.Group + "\t" + 0 + "\t" + p.Date);
                }                
            }
        }
    }
}
