using System;
using Common.Robots.Common;

namespace BitMexConnector.Parsers.OrderBookL2.DataItems
{
    public class InsertDataItem
    {
        public long Id { get; set; }
        public string Symbol { get; set; }
        public OrderSides Side { get; set; }

        public double Price { get; set; }
        public double Size { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}