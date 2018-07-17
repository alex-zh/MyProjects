using System;
using BitMexConnector.Tables.Classes;
using Newtonsoft.Json;

namespace BitMexConnector.Parsers.OrdersTable.DataItems
{
    public class PartialDataItem
    {
        [JsonProperty("orderID")]
        public Guid OrderId { get; set; }

        [JsonProperty("clOrdID")]
        public string ClientOrderId { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("ordType")]
        public OrderTypes OrderType { get; set; }

        [JsonProperty("ordStatus")]
        public OrderStatuses OrderStatus { get; set; }

        [JsonProperty("side")]
        public OrderBookSides Side { get; set; }

        [JsonProperty("price")]
        public double? Price { get; set; }

        [JsonProperty("orderQty")]
        public double Size { get; set; }
    }
    
}