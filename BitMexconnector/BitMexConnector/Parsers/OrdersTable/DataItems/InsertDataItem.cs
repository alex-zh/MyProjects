using System;
using Common.Robots.Common;
using Newtonsoft.Json;

namespace BitMexConnector.Parsers.OrdersTable.DataItems
{
    public class InsertDataItem
    {
        [JsonProperty("orderID")]
        public Guid SystemOrderId { get; set; }

        [JsonProperty("clOrdID")]
        public string ClientOrderId { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("ordStatus")]
        public string OrderStatus { get; set; }

        [JsonProperty("ordType")]
        public OrderTypes OrderType { get; set; }

        [JsonProperty("side")]
        public OrderSides Side { get; set; }

        [JsonProperty("price")]
        public double? Price { get; set; }

        [JsonProperty("simpleOrderQty")]
        public double Size { get; set; }

        [JsonProperty("simpleLeavesQty")]
        public double LeftSize { get; set; }

        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("workingIndicator")]
        public bool WorkingIndicator { get; set; }
    }
}