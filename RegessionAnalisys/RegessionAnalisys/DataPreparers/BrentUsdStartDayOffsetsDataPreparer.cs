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
     Для каждого дня, начиная с 11 часов до 23 часов, для каждой 5-минутной свечи ищем доходность (отступ) от начала дня для USD и Brent.
     Таким образом получается набор пар.
     */
    public class BrentUsdStartDayOffsetFromStartsOfTheDayDataPreparer
    {
        private string _exportFilePath;

        public BrentUsdStartDayOffsetFromStartsOfTheDayDataPreparer()
        {
            _exportFilePath = Directory.GetCurrentDirectory() + "\\..\\..\\AppData\\BrentUsd\\StartDayOffsets\\";

            var brentDataFilePath = Directory.GetCurrentDirectory() + "\\AppData\\ICE.BRN_141101_150219.txt";
            var usdDataFilePath = Directory.GetCurrentDirectory() + "\\AppData\\USD000UTSTOM_141101_150219.txt";

            var brentCandles = new FileLoader().Load(brentDataFilePath);
            var usdCandles = new FileLoader().Load(usdDataFilePath);

            var points = usdCandles.Join(brentCandles, x => x.Date, y => y.Date, (x, y) => new BrentUsdDataPoint{ Date = x.Date, UsdCandle = x, BrentCandle= y});

            var pointsByDate = points.GroupBy(x => x.Date.Date);
            //ищем точки с 11 до 23, соответсвенно нас интересуют дни у которых эти часы присудствуют
            //pointsByDate = pointsByDate.Where(x => x.Count() >= 12);

            foreach (var point in pointsByDate)
            {
                var percents = GetPercents(point.ToList());
                if(percents.Any())
                    Export(point.Key.Day.ToString("00") + "_" + point.Key.Month.ToString("00")+"_"+point.Key.Year, percents);
            }
        }

        //возвращает отступы от 11 часов
        private List<BrentUsdDataPercent> GetPercents(List<BrentUsdDataPoint> points)
        {
            var result = new List<BrentUsdDataPercent>();

            points = points.Where(x => x.Date.Hour >= 11 && x.Date.Hour < 23).OrderBy(x => x.Date).ToList();

            if (points.Any() == false)
                return result;
            
            var start = points.First();

            var usdStart = start.UsdCandle.Close;
            var brentStart = start.BrentCandle.Close;

            foreach (var p in points)
            {                
                var usdCurrent = p.UsdCandle.Close;
                var brentCurrent = p.BrentCandle.Close;

                result.Add(new BrentUsdDataPercent()
                {
                    Date = p.Date,
                    UsdPercent = Math.Round((usdCurrent-usdStart)*10000/usdStart),
                    BrentPercent = Math.Round((brentCurrent - brentStart) * 10000 / brentStart),
                });

            }
            
            return result;
        }


        private void Export(string filePath, IEnumerable<BrentUsdDataPercent> percents)
        {
            using (var streamWriter = new StreamWriter(_exportFilePath + "\\" + filePath))
            {
                streamWriter.WriteLine("Br" + " " + "Usd");    

                foreach (var p in percents)
                {
                    streamWriter.WriteLine(p.BrentPercent + " " + p.UsdPercent);    
                }
                
            }
        }
    }
}
