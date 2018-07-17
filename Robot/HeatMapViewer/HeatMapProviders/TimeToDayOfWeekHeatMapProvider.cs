using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Classes;
using Common.Classes.General;

namespace DistributionViewer.HeatMapProviders
{
   public  class TimeToDayOfWeekHeatMapProvider
    {
       private List<Candle> _candles;

       public TimeToDayOfWeekHeatMapProvider(List<Candle> candles)
       {
           _candles = candles;
       }

       public HeatMap Get()
       {
           var map = new Dictionary<int, Dictionary<int, List<double>>>();

           for (int dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++)
           {
               for (int candleIndex = 0; candleIndex < _candles.Count - 6; candleIndex++)
               {
                   var candle = _candles[candleIndex];
                   var candleNext = _candles[candleIndex + 6];
                   if (candle.Date.DayOfWeek != (DayOfWeek)dayOfWeek) continue;

                   var timeKey = (int)candle.Date.TimeOfDay.TotalMinutes - 10 * 60;
                   if (timeKey % 30 != 0) continue;

                   if (map.ContainsKey(dayOfWeek) == false)
                   {
                       map.Add(dayOfWeek, new Dictionary<int, List<double>>());
                   }

                   var timeValues = map[dayOfWeek];

                   if (timeValues.ContainsKey(timeKey) == false)
                   {
                       timeValues.Add(timeKey, new List<double>());
                   }

                   var values = timeValues[timeKey];

                   values.Add((candleNext.Close - candle.Close) * 100 / candle.Close);
               }
           }


           var heatMapList = new List<HeatMapChartElement>();

           var keys = new List<int>();

           for (int dayIndex = 0; dayIndex < map.Keys.Count; dayIndex++)
           {
               keys.AddRange(map[map.Keys.ElementAt(dayIndex)].Keys);
           }

           var dayKeys = keys.Distinct().OrderBy(x => x).ToList();

           for (int dayIndex = 0; dayIndex < map.Keys.Count; dayIndex++)
           {
               var dayValues = map[map.Keys.ElementAt(dayIndex)];

               for (int timeIndex = 0; timeIndex < dayKeys.Count; timeIndex++)
               {
                   var key = dayKeys.ElementAt(timeIndex);
                   if (dayValues.ContainsKey(key))
                   {
                       var values = dayValues[key];
                       var result = values.Average();
                       heatMapList.Add(new HeatMapChartElement { X = dayIndex, Y = timeIndex, Value = result });
                   }

               }
           }

           var maxValue = heatMapList.Max(x => x.Value);
           var minValue = heatMapList.Min(x => x.Value);

           foreach (var element in heatMapList)
           {
               // element.Value = (element.Value - minValue)/(maxValue - minValue);
           }

           var xTitles = new HeatMapTitles();

           for (int dayIndex = 0; dayIndex < map.Keys.Count; dayIndex++)
           {
               xTitles[dayIndex] = ((DayOfWeek)(map.Keys.ElementAt(dayIndex))).ToString();
           }

           var yTitles = new HeatMapTitles();

           for (int timeIndex = 0; timeIndex < dayKeys.Count; timeIndex++)
           {
               var totalMinutes = 10 * 60 + dayKeys[timeIndex];
               yTitles[timeIndex] = (totalMinutes / 60).ToString("00") + ":" + (totalMinutes % 60).ToString("00");
           }

           var heatMap = new HeatMap(xTitles, yTitles, heatMapList);

           return heatMap;
       }
    }
}
