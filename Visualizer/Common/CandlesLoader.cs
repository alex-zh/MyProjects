﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Common.Classes;
using Common.Classes.General;

namespace Common
{
    public class CandlesLoader
    {
        public string FirstLine = "<DATE>;<TIME>;<OPEN>;<HIGH>;<LOW>;<CLOSE>;<VOL>";

        public IEnumerable<Candle> FromFile(string fileName)
        {
            var result = new List<Candle>();

            var file = new FileInfo(fileName);

            var isFirstLine = true;

            using (StreamReader sr = file.OpenText())
            {
                string line;

                while (String.IsNullOrEmpty(line = sr.ReadLine()) == false)
                {
                    if (isFirstLine)
                    {
                        ValidateFirstLine(line);
                        isFirstLine = false;
                        continue;
                    }

                    var candle = ParseCandleLine(line);

                    result.Add(candle);
                }
            }

            return result;
        }

        public IEnumerable<Candle> FromText(string text)
        {
            var result = new List<Candle>();

            var isFirstLine = true;

            string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                if (isFirstLine)
                {
                    ValidateFirstLine(line);
                    isFirstLine = false;
                    continue;
                }

                var candle = ParseCandleLine(line);

                if (candle.Date.Hour >= 0 && candle.Date.Hour < 10)
                    continue;

                result.Add(ParseCandleLine(line));
            }

            return result;
        }

        private void ValidateFirstLine(string line)
        {
            if (line.Trim() != FirstLine)
            {
                throw new ArgumentException("First line format is not valid: " + line + "\r\n" + "Should be: " + FirstLine);
            }
        }

        private Candle ParseCandleLine(string line)
        {
            if (String.IsNullOrEmpty(line))
            {
                throw new ArgumentNullException("line");
            }

            line = line.Replace(" ", "");
            line = line.Trim();

            var data = line.Split(new[] { ";" }, StringSplitOptions.None);

            DateTime day, time;
                                    
            if (DateTime.TryParseExact(data[0],  "yyyyMMdd",CultureInfo.InvariantCulture, DateTimeStyles.None,  out day) == false)
            {
                throw new ArgumentException(" DateTime format is not valid: " + data[0]);
            }

            if (DateTime.TryParseExact(data[1], "HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out time) == false)
            {
                throw new ArgumentException(" Time format is not valid: " + data[1]);
            }            

            return new Candle()
            {
                Date = day.Date + time.TimeOfDay,
                Open = double.Parse(data[2]),
                High = double.Parse(data[3]),
                Low = double.Parse(data[4]),
                Close = double.Parse(data[5]),
                Volume = double.Parse(data[6])
            };            
        }
    }
}