using BitMexConnector.Tables.Classes;

namespace BitMexConnector
{
    public class BitMexOrderBookEntry
    {
        public long Id { get; set; }
        public double Size { get; set; }
        public double Price { get; set; }
        public string Symbol { get; set; }
        public OrderBookSides Side { get; set; }
    }
}