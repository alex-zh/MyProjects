using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FinancialIdeasTester.Classes;

namespace FinancialIdeasTester
{
    class DataLoader
    {
        internal static void LoadData(string dataFilePath, DataHolder dataHolder)
        {
            string line;
            var isFirstLine = true;

            using (var sr = new StreamReader(dataFilePath))
            {                
                while ((line = sr.ReadLine()) != null)
                {
                    if (isFirstLine) { isFirstLine = false; continue; }

                    var candle = ProcessLine(line);

                    dataHolder.Candles.Add(candle);
                    
                }
            }
        }

        //<DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>
        //20110503,100000,2026.4300000,2035.4100000,2024.3100000,2032.9000000,0
        private static Candle ProcessLine(string line)
        {
            var array = line.Split(',');

            if (array.Length != 7)
                Logger.Warn("Incorrect line format: " + line);

            var date = ParseDate(array[0], array[1]);
            var open = double.Parse(array[2], NumberFormatInfo.InvariantInfo);
            var close = double.Parse(array[5], NumberFormatInfo.InvariantInfo);

            return new Candle() { Date = date, Open = open, Close = close };
        }

        private static DateTime ParseDate(string dateString, string timeString)
        {
            var year = int.Parse(dateString.Substring(0,4));
            var month = int.Parse(dateString.Substring(4,2));
            var day = int.Parse(dateString.Substring(6,2));
            var hour = int.Parse(timeString.Substring(0, 2));

            return new DateTime(year, month, day, hour, 0, 0);
        }
        
    }
}
