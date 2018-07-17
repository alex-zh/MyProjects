using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;
using Common.Classes.General;
using Connector.Common;

namespace BitfinexConnector
{
    public class Connector : IConnector
    {
        private static string BaseUrl = "https://api.bitfinex.com/v2/";
        private static readonly string CandlesUrl = $"{BaseUrl}/candles/trade";
        private static int maxAllowedCandlesPerRequest = 1000;

        public IEnumerable<string> GetSymbols()
        {
            return new List<string>()
              {
                  "tBTCUSD",
                  "tETHUSD",
                  "tEOSUSD",
                  "tXRPUSD",
                  "tXRPBTC",
                  "tEOSBTC"
              };
        }

        public IEnumerable<Candle> GetCandles(string symbol, string timeframe, int limit, DateTime endDate)
        {
            var result = new List<Candle>();

            var numberOfRequests = (int)(limit - 1) / maxAllowedCandlesPerRequest;
            var offsetInMinutes = GetOffsetForTimeframe(timeframe);
            for (int i = 0; i <= numberOfRequests; i++)
            {
                var requestLimit = (limit - (i + 1) * maxAllowedCandlesPerRequest) >= 0
                    ? maxAllowedCandlesPerRequest
                    : (limit - i * maxAllowedCandlesPerRequest);

                var end = TimeConverter.FromDate(result.Any() ? result.First().Date.AddMinutes(-1 * offsetInMinutes) : DateTime.UtcNow).ToString();
                var candles = RequestCandles(symbol, timeframe, requestLimit, end).OrderBy(x => x.Date);
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

        private static IEnumerable<Candle> RequestCandles(string symbol, string timeframe, int limit, string end)
        {
            var url = $"{CandlesUrl}:{timeframe}:{symbol}/hist?limit={limit}&end={end}";

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    {
                        return new BitfinexResponseParser().Parse(ResponseTypes.Candles, response);
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
