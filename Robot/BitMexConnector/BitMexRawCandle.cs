using System;
using Newtonsoft.Json;

namespace BitMexConnector
{
    public class BitMexRawCandle
    {
        [JsonProperty(PropertyName = "symbol")]
        public string Symbol { get; set; }
        [JsonProperty(PropertyName = "open")]
        public double? Open { get; set; }
        [JsonProperty(PropertyName = "close")]
        public double? Close { get; set; }
        [JsonProperty(PropertyName = "low")]
        public double? Low { get; set; }
        [JsonProperty(PropertyName = "high")]
        public double? High { get; set; }
        [JsonProperty(PropertyName = "volume")]
        public double? Volume{ get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }
    }
}