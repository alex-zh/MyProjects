using Common.Robots.Common;

namespace BitMexConnector.Parsers.OrderBookL2.DataItems
{
    public class DeleteDataItem
    {
        public long Id { get; set; }
        public string Symbol { get; set; }
        public OrderSides Side { get; set; }
    }
}
