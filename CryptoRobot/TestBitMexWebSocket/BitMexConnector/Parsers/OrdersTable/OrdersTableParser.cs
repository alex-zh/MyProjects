﻿using System;
using System.Linq;
using BitMexConnector.DataStoreUpdators;
using BitMexConnector.Parsers.OrdersTable.DataItems;
using Newtonsoft.Json.Linq;

namespace BitMexConnector.Parsers.OrdersTable
{
    public class OrdersTableParser
    {
        private readonly ILogger _logger;
        private readonly OrdersTableUpdator _updator;

        public OrdersTableParser(OrdersTableUpdator updator, ILogger logger)
        {
            _updator = updator;
            _logger = logger;
        }

        public void Parse(JObject data)
        {
            Validate(data);

            var tableData = BitMexTableDataParser.ParseTableData(data);

            if (tableData.HasErrors)
            {
                _logger.Error(tableData.ErrorText);
                return;
            }

            try
            {
                switch (tableData.ActionName)
                {
                    case ActionNames.Partial:
                        HandlePartialData(tableData.Data);
                        break;
                    case ActionNames.Insert:
                        HandleInsertData(tableData.Data);
                        break;
                    case ActionNames.Update:
                        HandleUpdateData(tableData.Data);
                        break;
                    case ActionNames.Delete:
                        HandleDeleteData(tableData.Data);
                        break;

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

        }

        private void Validate(JObject data)
        {
            // TODO: add keys validation
        }

        private void HandlePartialData(JArray data)
        {
            _updator.Update(data.Select(x => x.ToObject<PartialDataItem>()));
        }

        private void HandleInsertData(JArray data)
        {
            _updator.Update(data.Select(x => x.ToObject<InsertDataItem>()));
        }

        private void HandleUpdateData(JArray data)
        {
            //_updator.Update(data.Select(x => x.ToObject<UpdateDataItem>()));
        }

        private void HandleDeleteData(JArray data)
        {
            //_updator.Update(data.Select(x => x.ToObject<DeleteDataItem>()));
        }
    }
}