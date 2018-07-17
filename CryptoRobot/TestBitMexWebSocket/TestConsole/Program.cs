using BitMEX;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Test
{
    internal class Program
    {
        //private static string bitmexKey = "API_KEY";
        //private static string bitmexSecret = "API_SECRET";
        public static string bitmexKey => "3uFBfBaA6FItdS8LbNr3_k1C";
        public static string bitmexSecret => "OIMZo3Hj46TygavxKPrVy7ys0hj9Z2YSgU81pusc4ejBs6R-";


        private static void Main(string[] args)
        {
            //Program p = new Program();
            //p.Run(args);
            var json = "{\"orderID\": \"74090a64-09cd-fdea-688b-33ead0387943\", \"clOrdID\": \"123\",\"accounT\": \"limit\"}";
            var jobject = JObject.Parse(json);
            var myclass = jobject.ToObject<MyClass>();
        }

        private void Run(string[] args)
        {
            BitMEXApi bitmex = new BitMEXApi(bitmexKey, bitmexSecret);
            // var orderBook = bitmex.GetOrderBook("XBTUSD", 3);
            var orders = bitmex.PostOrders();
            // var orders = bitmex.DeleteOrders();
            Console.WriteLine(orders);
        }
    }

    public enum OrderTypes
    {
        Market = 10,
        Limit = 11
    }
    internal class MyClass
    {
        [JsonProperty("OrderId")]
        public Guid OrderId { get; set; }

        [JsonProperty("ClOrdID")]
        public String ClientOrderId { get; set; }

        [JsonProperty("Account")]
        public OrderTypes AccountNumber { get; set; }

    }
}
