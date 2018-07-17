using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Common.Classes.General;
using Common.Finders;

namespace Viewer
{
    internal class StructureParser
    {
        /// <summary>
        /// Format:
        /// 1-10, 2-15, 6-20        
        /// </summary>
        public static StructureInfo ParseFromText(List<Candle> candles, string text, bool useLogScale)
        {
            text = text.Replace(" ", "").Trim();

            var parts = text.Split(',');

            var result = new StructureInfo();

            foreach (var part in parts)
            {
                var startIndex = int.Parse(part.Split('-')[0]);
                var endIndex = int.Parse(part.Split('-')[1]);

                var startCandle = candles[startIndex];
                var endCandle = candles[endIndex];

               var change =  CalculateChange(startCandle, endCandle, useLogScale);

                var atr = CalculateVolatility(candles, startIndex, endIndex, change);// 

                var item = new StructureInfoItem() {Period = endIndex-startIndex, Change =  change, Atr = atr};
                result.Items.Add(item);
            }

            return result;
        }

        public static StructureInfo Parse(List<Candle> candles, Structure structure, bool useLogScale)
        {            
            var result = new StructureInfo();

            foreach (var part in structure)
            {
                var startIndex = part.StartIndex;
                var endIndex = part.EndIndex;

                var startCandle = candles[startIndex];
                var endCandle = candles[endIndex];

                var change = CalculateChange(startCandle, endCandle, useLogScale);

                var atr = CalculateVolatility(candles, startIndex, endIndex, change);// 

                var item = new StructureInfoItem() { Period = endIndex - startIndex, Change = change, Atr = atr };
                result.Items.Add(item);
            }

            return result;
        }

        private static double CalculateVolatility(List<Candle> candles, int startIndex, int endIndex, double change)
        {
            var range = candles.TakeRange(startIndex, endIndex).ToList();
            var stepChange = change / (endIndex - startIndex + 1);

            var deviations = range.Select((candle, index) =>
            {
                var trendValue = ((index + 1) * stepChange + 1) * range.First().Open;
                return candle.Close - trendValue;
            }).ToList();

           var result = deviations.Select(Math.Abs).Max();

            var atr = new SimpleAtr(candles.TakeRange(Math.Max(0, endIndex - 30), endIndex).ToArray()).Calculate();

            return result;
        }

        private static double CalculateChange(Candle startCandle, Candle endCandle, bool useLogScale)
        {
            if (useLogScale)
            {
                return endCandle.Log().Close - startCandle.Log().Open;
            }

            return (endCandle.Close - startCandle.Open) / startCandle.Open;
        }
    }
}