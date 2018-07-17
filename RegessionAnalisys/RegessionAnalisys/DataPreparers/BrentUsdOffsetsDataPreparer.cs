using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using Scatterplot.Core.Classes;

namespace RegessionAnalisys.DataPreparers
{
    /* 
     Готовит данные для пары USD/RUB и Brent
     Для конкретного момента (текущий момент) рассматриваем период, на котором для каждой 5-минутной свечи ищем офсет по нефти и по доллару. 
     Оффсеты вверх и вниз получают разные категории.
     Данные будут использоватся для нахождения регрессии отдельно для движений вверх и вниз. 
     Сможем, например, понять что в текущий момент курс рубля реагирует сильнее на движения в нефти вниз чем на движения вверх. 
     Таким образом, в частности, сможем реализовать некоторую фильтрацию.      
     */
    public class BrentUsdOffsetDataPreparer
    {
        private string _exportFilePath;
                
        public BrentUsdOffsetDataPreparer()
        {
            _exportFilePath = Directory.GetCurrentDirectory() + "\\..\\..\\AppData\\BrentUsd\\Offsets\\";

            var brentDataFilePath = Directory.GetCurrentDirectory() + "\\AppData\\ICE.BRN_141101_150219.txt";
            var usdDataFilePath = Directory.GetCurrentDirectory() + "\\AppData\\USD000UTSTOM_141101_150219.txt";

            var brentCandles = new FileLoader().Load(brentDataFilePath);
            var usdCandles = new FileLoader().Load(usdDataFilePath);

            var points = usdCandles.Join(brentCandles, x => x.Date, y => y.Date, (x, y) => new BrentUsdDataPoint{ Date = x.Date, UsdCandle = x, BrentCandle = y });

            var pointsByDate = points.GroupBy(x => x.Date.Date);
            
            foreach (var point in pointsByDate)
            {
                var percents = GetPercents(point.ToList());
                if(percents.Any())
                    Export(point.Key.Day.ToString("00") + "_" + point.Key.Month.ToString("00")+"_"+point.Key.Year, percents);
            }
        }
        
        private List<BrentUsdDataPercent> GetPercents(List<BrentUsdDataPoint> points)
        {
            var result = new List<BrentUsdDataPercent>();

            points = points.Where(x => x.Date.Hour >= 11 && x.Date.Hour < 19).OrderBy(x => x.Date).ToList();

            if (points.Any() == false)
            {
                return result;
            }
            
            for(var i=0; i< points.Count(); i++)
            {
                var p = points[i];

                var usdCurrent = p.UsdCandle;
                var brentCurrent = p.BrentCandle;

                result.Add(new BrentUsdDataPercent()
                {
                    Date = p.Date,
                    UsdPercent = Math.Round((usdCurrent.Close - usdCurrent.Open) * 10000 / usdCurrent.Open),
                    BrentPercent = Math.Round((brentCurrent.Close - brentCurrent.Open) * 10000 / brentCurrent.Open),
                });

                for (int j = 2; j < 2; j++)
                {
                    if (i >=j)
                    {
                        var previousPoint = points[i - j];

                        result.Add(new BrentUsdDataPercent()
                        {
                            Date = previousPoint.Date,
                            UsdPercent = Math.Round((p.UsdCandle.Close - previousPoint.UsdCandle.Open) * 10000 / (j * previousPoint.UsdCandle.Open)),
                            BrentPercent = Math.Round((p.BrentCandle.Close - previousPoint.BrentCandle.Open) * 10000 / (j * previousPoint.BrentCandle.Open)),
                            IsGrouped = true
                        });
                    }                    
                }
                
            }            

            return result;
        }


        private void Export(string filePath, IEnumerable<BrentUsdDataPercent> percents)
        {
            using (var streamWriter = new StreamWriter(_exportFilePath + "\\" + filePath + ".txt"))
            {
                streamWriter.WriteLine(@"Br\tUsd\tGroup\tSubGroup");    

                foreach (var p in percents)
                {
                    var group = p.IsGrouped ? 2 : 1;
                    var category = p.BrentPercent > 0 ? 1 :2 ;
                    streamWriter.WriteLine(@p.BrentPercent + "\t" + p.UsdPercent + "\t" + category + "\t" + group);    
                }                
            }
        }
    }
}
