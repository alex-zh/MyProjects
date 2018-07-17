using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Common.Classes.General;
using Connector.Common;
using Newtonsoft.Json;

namespace BitfinexConnector
{
    public class BitfinexResponseParser
    {
        public IEnumerable<Candle> Parse(ResponseTypes responseType, HttpResponseMessage response)
        {
            if(responseType != ResponseTypes.Candles)
                throw new NotSupportedException(responseType.ToString());

            var jsonText = response.Content.ReadAsStringAsync().Result;

            var candles = JsonConvert.DeserializeObject<List<List<double>>>(jsonText);

            return candles.Select(ParseRawCandle).ToList();
        }

        /*
        [MTS, OPEN, CLOSE, HIGH, LOW, VOLUME] */
        private Candle ParseRawCandle(List<double> rawCandle)
        {
            var date = TimeConverter.FromMilliseconds(rawCandle[0]);

            return new Candle
            {
                Date = date,
                Open = rawCandle[1],
                Close = rawCandle[2],
                High = rawCandle[3],
                Low = rawCandle[4],
                Volume =  rawCandle[5]
            };
        }
    }
}