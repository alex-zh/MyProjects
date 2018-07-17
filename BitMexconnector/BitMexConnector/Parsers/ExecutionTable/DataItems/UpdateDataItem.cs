using System;
using Common.Robots.Common;
using Newtonsoft.Json;

namespace BitMexConnector.Parsers.ExecutionTable.DataItems
{
    public class UpdateDataItem
    {
        [JsonProperty("execID")]
        public string ExecId { get; set; }

        [JsonProperty("orderID")]
        public Guid SystemOrderId { get; set; }

        [JsonProperty("clOrdID")]
        public string ClientOrderId { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("side")]
        public OrderSides Side { get; set; }

        [JsonProperty("lastQty")]
        public double? Size { get; set; }

        [JsonProperty("simpleOrderQty")]
        public double? OrderSize { get; set; }

        [JsonProperty("ordStatus")]
        public string OrderStatus { get; set; } // New -> Canceled/Filled

        [JsonProperty("simpleLeavesQty")]
        public double? LeftSize { get; set; }

        [JsonProperty("execType")]
        public ExecutionTypes ExecType { get; set; }

        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }
   }
}