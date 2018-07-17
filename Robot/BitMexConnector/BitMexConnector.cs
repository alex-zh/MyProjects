using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Common.Classes.General;
using Connector.Common;

namespace BitMexConnector
{
    public class Connector : IConnector
    {
        private static string BaseUrl = "https://www.bitmex.com/api/v1";
        private static readonly string CandlesUrl = $"{BaseUrl}/trade/bucketed";
        private static int maxAllowedCandlesPerRequest = 500;

        public IEnumerable<string> GetSymbols()
        {
            return new List<string>()
              {
                  "XBTUSD",
                  "ETHU18",
                  "LTCU18",
                  "BCHU18",
                  "XRPU18"
              };
        }

        public IEnumerable<Candle> GetCandles(string symbol, string timeframe, int limit, DateTime endDate)
        {
            var candlesEndDate = (endDate.Kind == DateTimeKind.Local) ? endDate.ToUniversalTime() : endDate;            

            var result = new List<Candle>();

            var numberOfRequests = (int)(limit-1) / maxAllowedCandlesPerRequest;
            var offsetInMinutes = GetOffsetForTimeframe(timeframe);
            for (int i = 0; i <= numberOfRequests; i++)
            {
                var requestLimit = (limit - (i+1)*maxAllowedCandlesPerRequest) >= 0
                    ? maxAllowedCandlesPerRequest
                    : (limit - i*maxAllowedCandlesPerRequest);

                var end = (result.Any() ? result.First().Date.AddMinutes(-1* offsetInMinutes) : candlesEndDate).ToString("o");
                var candles = RequestCandles(symbol, timeframe, requestLimit, end).OrderBy(x=>x.Date);
                result.InsertRange(0, candles);
            }

            return result;
        }

        private static int GetOffsetForTimeframe(string timeframe)
        {
            switch (timeframe)
            {
                case "1m":
                    return 1;
                case "5m":
                    return 5;
                case "15m":
                    return 15;
                case "1h":
                    return 60;
                default: throw new NotSupportedException(timeframe);
                    
            }
        }

        private static IEnumerable<Candle> RequestCandles(string symbol, string timeframe, int count, string end)
        {
            var url = $"{CandlesUrl}?symbol={symbol}&binSize={timeframe}&count={count}&reverse=true&endTime={end}";

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    {
                        return new BitMexResponseParser().Parse(ResponseTypes.Candles, response);
                    }
                }
            }

            return new Candle[0];
        }

        public IEnumerable<Candle> GetCandles(string symbol, string timeframe, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
    }
}
