using BitMexConnector.Tables.Classes;
using Common.Robots.Common;

namespace BitMexConnector
{
    public class BitMexOrderBookEntry
    {
        public long Id { get; set; }
        public double Size { get; set; }
        public double Price { get; set; }
        public string Symbol { get; set; }
        public OrderSides Side { get; set; }
    }
}