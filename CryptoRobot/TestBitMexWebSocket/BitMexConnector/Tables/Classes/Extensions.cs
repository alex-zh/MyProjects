namespace BitMexConnector.Tables.Classes
{
    public static class Extensions
    {
        public static LimitOrder ToLimitOrder(this Parsers.OrdersTable.DataItems.PartialDataItem item)
        {
            return new LimitOrder
            {
                OrderId = item.OrderId,
                ClientOrderId = item.ClientOrderId,
                OrderStatus = item.OrderStatus,
                OrderType = item.OrderType,
                Size = item.Size,
                Price = item.Price,
                Symbol = item.Symbol
            };
        }

        public static LimitOrder ToLimitOrder(this Parsers.OrdersTable.DataItems.InsertDataItem item)
        {
            return new LimitOrder
            {
                OrderId = item.OrderId,
                ClientOrderId = item.ClientOrderId,
                OrderStatus = item.OrderStatus,
                OrderType = item.OrderType,
                Size = item.Size,
                Price = item.Price,
                Symbol = item.Symbol
            };
        }
    }
}
