using System;
using Newtonsoft.Json;

namespace BitMexConnector.Parsers.OrdersTable.DataItems
{
    public class UpdateDataItem
    {
        [JsonProperty("orderID")]
        public Guid OrderId { get; set; }

        [JsonProperty("clOrdID")]
        public string ClientOrderId { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("ordStatus")]
        public OrderStatuses OrderStatus { get; set; } // New -> Canceled

    }
}