using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Common.Classes.General;
using Newtonsoft.Json;
using Connector.Common;

namespace BitMexConnector
{
    //[{"timestamp":"2018-01-21T13:56:00.000Z","symbol":"XBTUSD","open":12000,"high":12000,"low":11999,"close":12000,"trades":252,"volume":1209979,"vwap":12000.48,"lastSize":285,"turnover":10083092117,"homeNotional":100.83092117,"foreignNotional":1209979},{"timestamp":"2018-01-21T13:55:

    public class BitMexResponseParser
    {

        public IEnumerable<Candle> Parse(ResponseTypes responseType, HttpResponseMessage response)
        {
            if (responseType != ResponseTypes.Candles)
                throw new NotSupportedException(responseType.ToString());

            var jsonText = response.Content.ReadAsStringAsync().Result;

            var candles = JsonConvert.DeserializeObject<List<BitMexRawCandle>>(jsonText);

            return candles.Select(ToCandle).ToList();
        }

        private Candle ToCandle(BitMexRawCandle rawCandle)
        {
            return new Candle()
            {
                Date = rawCandle.Timestamp,
                Open = rawCandle.Open ?? 0,
                Close = rawCandle.Close ?? 0,
                Low = rawCandle.Low ?? 0,
                High = rawCandle.High ?? 0,
                Volume = rawCandle.Volume ?? 0

            };
        }
    }
}