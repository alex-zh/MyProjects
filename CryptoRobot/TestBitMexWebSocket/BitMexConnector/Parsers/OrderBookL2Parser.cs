using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace BitMexConnector.Parsers
{
    class OrderBookL2Parser
    {
        private readonly ILogger _logger;

        public OrderBookL2Parser(DataStore store, ILogger logger)
        {
            _logger = logger;
        }

        public BitMexWebSocketParseResult<BitMexOrderBookEntry> Parse(JObject data)
        {
            var actionName = ParseActionName(data);

            if (actionName.HasValue == false)
            {
                _logger.Error("Action attribute is not present in OrderBookL2 payload");
                return null;
            }

            if (actionName == BitMexWebSocketActionNames.Partial)
            {
                ParsePartial(data);
            }           
        }

        private List<BitMexOrderBookEntry> ParsePartial(JObject payload)
        {
            var result = new List<BitMexOrderBookEntry>();

            var keys = new[]
           {
                "symbol",
                "id",
                "side"
            };

            JToken keysJToken;
            if (!payload.TryGetValue("keys", out keysJToken))
            {
                _logger.Log("Keys attribute is not preset in OrderBookL2 payload");
                return result;
            }

            JArray array = (JArray)keysJToken;
            var dataKeys = array.Select(x => x.Value<string>());

            foreach (var key in keys)
            {
                if (dataKeys.Contains(key) == false)
                {
                    _logger.Log($"{key} is not present in payload keys. Parsing of OrderBookL2 is impossible");
                    return result;
                }
            }

            JToken dataJToken;
            if (!payload.TryGetValue("data", out dataJToken))
            {
                _logger.Log("Data attribute is not preset in OrderBookL2 payload");
                return result;
            }

            JArray data = (JArray)dataJToken;

            foreach (var item in data)
            {
                var entry = ParseOrderBookEntry(item);
            }
            //var dataKeys = array.Select(x => x.Value<string>());

            return result;
        }

        /* 
            {"symbol":"XBTUSD","id":17999992000,"side":"Sell","size":100,"price":80},
        */
        private BitMexOrderBookEntry ParseOrderBookEntry(JToken item)
        {
            throw new NotImplementedException();
        }

        private BitMexWebSocketActionNames? ParseActionName(JObject data)
        {
            JToken actionJToken;
            if (!data.TryGetValue("action", out actionJToken))
            {                
                return null;
            }

            var actionName = actionJToken.Value<string>();

            return (BitMexWebSocketActionNames)Enum.Parse(typeof(BitMexWebSocketActionNames), actionName, true);
        }
    }
}
