using System.Linq;
using BitMexConnector.DataStoreUpdators;
using BitMexConnector.Parsers.OrderBookL2.DataItems;
using Common.Classes.Logger;
using Newtonsoft.Json.Linq;

namespace BitMexConnector.Parsers.OrderBookL2
{
    public class OrderBookL2Parser
    {
        private readonly ILogger _logger;
        private readonly OrderBookUpdator _updator;

        public OrderBookL2Parser(OrderBookUpdator updator, ILogger logger)
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
            _updator.Update(data.Select(x => x.ToObject<UpdateDataItem>()));
        }

        private void HandleDeleteData(JArray data)
        {
            _updator.Update(data.Select(x => x.ToObject<DeleteDataItem>()));
        }
    }
}