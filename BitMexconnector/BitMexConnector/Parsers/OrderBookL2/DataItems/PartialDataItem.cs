using BitMexConnector.Tables.Classes;
using Common.Robots.Common;

namespace BitMexConnector.Parsers.OrderBookL2.DataItems
{
    public class PartialDataItem
    {
        public long Id { get; set; }
        public string Symbol { get; set; }
        public OrderSides Side { get; set; }

        public double Price { get; set; }
        public double Size { get; set; }
    }
}