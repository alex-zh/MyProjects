using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using BitMexConnector.DataStoreUpdators;
using BitMexConnector.Parsers;
using BitMexConnector.Parsers.OrderBookL2;
using BitMexConnector.Parsers.OrdersTable;
using Newtonsoft.Json.Linq;

namespace BitMexConnector
{
    public class DeltaParser
    {
        private readonly DataStore _store;
        private readonly ILogger _logger;
        
        private readonly OrderBookUpdator _orderBookUpdator;
        private OrdersTableUpdator _ordersTableUpdator;

        public DeltaParser(DataStore store, ILogger logger)
        {
            _store = store;
            _logger = logger;
            _orderBookUpdator = new OrderBookUpdator(_store);
            _ordersTableUpdator = new OrdersTableUpdator(_store, _logger);
        }

        public void Parse(JObject data)
        {
            JToken tableNameJToken;
            if (!data.TryGetValue("table", out tableNameJToken))
            {
                _logger.Error("Table attribute is not present in data");
                return;
            }

            var tableName = tableNameJToken.Value<string>();
                    
            if (tableName == SymbolTables.OrderBookL2)
            {
                new OrderBookL2Parser(_orderBookUpdator, _logger).Parse(data);
            }
            else if (tableName == SystemTables.Order)
            {
                new OrdersTableParser(_ordersTableUpdator, _logger).Parse(data);
            }
            else
            {
                _logger.Error($"Not supported table {tableName}");
            }

        }

        private void ParseOrderBookL2(JObject data)
        {

        }
    }
}

