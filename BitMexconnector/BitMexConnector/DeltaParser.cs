using System;
using System.IO;
using BitMexConnector.DataStoreUpdators;
using BitMexConnector.Parsers.ExecutionTable;
using BitMexConnector.Parsers.OrderBookL2;
using BitMexConnector.Parsers.OrdersTable;
using Common.Classes.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitMexConnector
{
    internal class DeltaParser
    {
        private readonly DataStore _store;
        private readonly ILogger _logger;
        private readonly bool _logInFile;

        private readonly OrderBookUpdator _orderBookUpdator;
        private OrdersTableUpdator _ordersTableUpdator;
        private ExecutionTableUpdator _executionTableUpdator;

        public DeltaParser(DataStore store, ILogger logger, bool logInFile = true)
        {
            _store = store;
            _logger = logger;
            _logInFile = logInFile;
            _orderBookUpdator = new OrderBookUpdator(_store);
            _ordersTableUpdator = new OrdersTableUpdator(_store, _logger);
            _executionTableUpdator = new ExecutionTableUpdator(_store, _logger);
        }

        public void Parse(JObject data)
        {
            JToken tableNameJToken;
            if (!data.TryGetValue("table", out tableNameJToken))
            {
                _logger.Warning($"Table attribute is not present in data");
                return;
            }

            var tableName = tableNameJToken.Value<string>();

            _store.LastUpdateTime = DateTime.Now;

            if (tableName == SymbolTables.OrderBookL2)
            {
                new OrderBookL2Parser(_orderBookUpdator, _logger).Parse(data);
            }
            else if (tableName == SystemTables.Order)
            {
                new OrdersTableParser(_ordersTableUpdator, _logger).Parse(data);
            }
            else if (tableName == SystemTables.Execution)
            {
                new ExecutionTableParser(_executionTableUpdator, _logger).Parse(data);
            }
            else
            {
                _logger.Error($"Not supported table {tableName}");
            }

            if (_logInFile)
            {
                var path = $"C:\\BitMex\\logs\\{tableName}";

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                using (var sw = File.CreateText($"{path}\\{Guid.NewGuid()}.json"))
                {
                    using (var writer = new JsonTextWriter(sw))
                        data.WriteTo(writer);
                }
            }
        }

        private void ParseOrderBookL2(JObject data)
        {

        }
    }
}

