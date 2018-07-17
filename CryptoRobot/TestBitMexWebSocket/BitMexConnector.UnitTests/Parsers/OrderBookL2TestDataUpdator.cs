using System.Collections.Generic;
using System.Text;
using BitMexConnector.Parsers.OrderBookL2.DataItems;

namespace BitMexConnector.UnitTests.Parsers
{
    public class OrderBookL2TestDataUpdator
    {
        private readonly string _table;

        public OrderBookL2TestDataUpdator(string table)
        {
            _table = table;
        }

        public string GetJson(string symbol, List<InsertDataItem> items)
        {
            var builder = new StringBuilder();
            builder.Append("{");
            builder.Append($"\"table\":\"{_table}\",");
            builder.Append("\"action\":\"insert\",");

            builder.Append("\"data\":[");

            foreach (var item in items)
            {
                builder.Append("{");
                builder.Append($"\"symbol\":\"{symbol}\",\"id\":{item.Id},\"side\":\"{item.Side}\", \"size\": {item.Size},\"price\":{item.Price}");
                builder.Append("},");
            }

            builder.Append("]");
            builder.Append("}");

            return builder.ToString();
        }

        public string GetJson(string symbol, List<UpdateDataItem> items)
        {
            var builder = new StringBuilder();
            builder.Append("{");
            builder.Append($"\"table\":\"{_table}\",");
            builder.Append("\"action\":\"update\",");

            builder.Append("\"data\":[");

            foreach (var item in items)
            {
                builder.Append("{");
                builder.Append($"\"symbol\":\"{symbol}\",\"id\":{item.Id},\"side\":\"{item.Side}\", \"size\": {item.Size}");
                builder.Append("},");
            }

            builder.Append("]");
            builder.Append("}");

            return builder.ToString();
        }

        public void GetJson(string symbol, List<DeleteDataItem> items)
        {
            throw new System.NotImplementedException();
        }
    }
}