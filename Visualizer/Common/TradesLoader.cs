using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Common.Classes.StrategyTester;

namespace Common
{
    // each line has the following format
    // 2015-10-01 19:01:00.000;SiZ5;-2;67249.00000
    public class TradesLoader
    {
        private readonly string _tickerName;

        public TradesLoader(string tickerName)
        {
            _tickerName = tickerName;
        }

        public IEnumerable<ITradeItem> FromFile(string fileName)
        {
            var result = new List<ITradeItem>();

            var file = new FileInfo(fileName);

            using (StreamReader sr = file.OpenText())
            {
                string line, ticker;

                while (String.IsNullOrEmpty(line = sr.ReadLine()) == false)
                {
                   var trade = ParseTradeLine(line, out ticker);

                    if (ticker == _tickerName)
                    {
                        result.Add(trade);
                    }
                }
            }

           result =  RestoreDeals(result);

            return result;
        }

        /// <summary>
        /// User can buy 5 contracts, after that sell 7 in one trade
        /// Two trades must be return: buy 5, sell 5, sell 2
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private List<ITradeItem> RestoreDeals(List<ITradeItem> rawResult)
        {
            var result = new List<ITradeItem>();

            double size=0;

            for (int i = 0; i < rawResult.Count; i++)
            {
                var rawTrade = rawResult[i];
                var tradeSize = rawTrade.Size;

                if (size > 0 && (tradeSize + size) < 0)
                {
                    result.Add(new TestTradeItem() { DateTime = rawTrade.DateTime, Price = rawTrade.Price, Size = size * -1 });
                    result.Add(new TestTradeItem() { DateTime = rawTrade.DateTime, Price = rawTrade.Price, Size = tradeSize + size });
                }
                else if (size < 0 && (tradeSize + size) > 0)
                {
                    result.Add(new TestTradeItem() { DateTime = rawTrade.DateTime, Price = rawTrade.Price, Size = size * -1 });
                    result.Add(new TestTradeItem() { DateTime = rawTrade.DateTime, Price = rawTrade.Price, Size = tradeSize + size });
                }
                else
                {
                    result.Add(rawResult[i]);
                }

                size += tradeSize;
            }

            return result;
        }

        private ITradeItem ParseTradeLine(string line, out string ticker)
        {
            if (String.IsNullOrEmpty(line))
            {
                throw new ArgumentNullException("line");
            }

            var data = line.Split(new[] { ";" }, StringSplitOptions.None);

            var dateTime = new DateTime();

            if (DateTime.TryParseExact(data[0], "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime) == false)
            {
                throw new ArgumentException(" DateTime format is not valid: " + data[0]);
            }

            //dateTime = dateTime.AddMinutes(-1);// new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute - 1, dateTime.Second, 0);
            if (dateTime.Hour == 14 && dateTime.Minute == 0 && dateTime.Second == 0)
            {
                dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 13, 59, 0);
            }

            if (dateTime.Hour == 18 && dateTime.Minute == 45 && dateTime.Second == 0)
            {
                dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 18, 44, 0);
            }

            if (dateTime.Hour == 23 && dateTime.Minute == 50 && dateTime.Second == 0)
            {
                dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 49, 0);
            }

            ticker = data[1];

            return new TestTradeItem()
            {
                DateTime = dateTime,
                Size = int.Parse(data[2]),
                Price = double.Parse(data[3]),
            };
        }
    }
}