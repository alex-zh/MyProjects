using BitMexConnector.Tables.Classes;

namespace BitMexConnector.Parsers.OrderBookL2.DataItems
{
    public class PartialDataItem
    {
        public long Id { get; set; }
        public string Symbol { get; set; }
        public OrderBookSides Side { get; set; }

        public double Price { get; set; }
        public double Size { get; set; }
    }
}