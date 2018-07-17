using System;
using Newtonsoft.Json;

namespace BitMexConnector.Parsers.OrdersTable.DataItems
{
    public class UpdateDataItem
    {
        [JsonProperty("orderID")]
        public Guid SystemOrderId { get; set; }

        [JsonProperty("clOrdID")]
        public string ClientOrderId { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("ordStatus")]
        public string OrderStatus { get; set; } // New -> Canceled

        [JsonProperty("simpleLeavesQty")]
        public double LeftSize { get; set; }

        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("workingIndicator")]
        public bool WorkingIndicator { get; set; }
    }
}