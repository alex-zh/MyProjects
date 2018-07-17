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
    public class BrentUsdAllDataPreparer
    {
        private string _exportFilePath;
                
        public BrentUsdAllDataPreparer()
        {
            _exportFilePath = "C:\\TestData\\Results";

            var usdDataFilePath = "C:\\TestData\\Input\\SPFB.Si_150901_151113.txt";
            var brentDataFilePath = "C:\\TestData\\Input\\SPFB.BR_150901_151113.txt";
            
            var usdCandles = new FileLoader().Load(usdDataFilePath);
            var brentCandles = new FileLoader().Load(brentDataFilePath);
            
            var points = usdCandles.Join(brentCandles, x => x.Date, y => y.Date, (x, y) => new BrentUsdDataPoint{ Date = x.Date, UsdCandle = x, BrentCandle = y });

            var pairs = BuildPairs(points.ToList());

            if (pairs.Any())
            {
                Export("result", pairs);
            }
          }

      

        private List<BrentUsdPair> BuildPairs(List<BrentUsdDataPoint> points)
        {
            var result = new List<BrentUsdPair>();

          if (points.Any() == false)
            {
                return result;
            }

            for (var i = 0; i < points.Count(); i += 60)
            {
                var p = points[i];

                var currentIndex = i;
                var pointsRange = points.Where((x, index) => index > currentIndex - 1 && index < currentIndex + 1).ToArray();
                var usdAverage = pointsRange.Select(x => x.UsdCandle.GetPrice(PriceTypes.Mean)).Average();
                var brentAverage = pointsRange.Select(x => x.BrentCandle.GetPrice(PriceTypes.Mean)).Average();
                var group = p.Date.Hour < 19 ? 1 : 2;

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
                    streamWriter.WriteLine(p.BrentPrice.ToString("F2") + "\t" + p.UsdPrice.ToString("F2") + "\t" + p.Group + "\t" + 0 + "\t" + p.Date);
                }                
            }
        }
    }
}
