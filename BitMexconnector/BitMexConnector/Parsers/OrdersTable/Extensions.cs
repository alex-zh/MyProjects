using System;
using BitMexConnector.Tables.Classes;
using Common.Robots.Common;

namespace BitMexConnector.Parsers.OrdersTable
{
    public static class Extensions
    {
        public static BitMexLimitOrder ToLimitOrder(this DataItems.PartialDataItem item)
        {
            return new BitMexLimitOrder
            {
                OrderId = item.ClientOrderId,
                SystemOrderId = item.SystemOrderId.ToString(),                
                OrderStatus =  ToOrderStatus(item.OrderStatus),
                OrderType = item.OrderType,
                Size = item.Size,
                Price = item.Price,
                Symbol = item.Symbol,
                DateTime = item.TimeStamp,
                Side = item.Side,
                LeftSize = item.LeftSize
            };
        }

        public static BitMexLimitOrder ToLimitOrder(this DataItems.InsertDataItem item)
        {
            return new BitMexLimitOrder
            {
                OrderId = item.ClientOrderId,
                SystemOrderId = item.SystemOrderId.ToString(),
                OrderStatus = ToOrderStatus(item.OrderStatus),
                OrderType = item.OrderType,
                Size = item.Size,
                Price = item.Price,
                Symbol = item.Symbol,
                DateTime = item.TimeStamp,
                Side = item.Side,
                LeftSize = item.LeftSize
            };
        }

        public static OrderStatuses ToOrderStatus(string status)
        {
            if(status.ToLower() == "New".ToLower())
                return OrderStatuses.Active;

            if (status.ToLower() == "Canceled".ToLower())
                return OrderStatuses.Withdrawn;

            if (status.ToLower() == "Filled".ToLower())
                return OrderStatuses.Completed;

            if (status.ToLower() == "Rejected".ToLower())
                return OrderStatuses.Rejected;

            throw new NotSupportedException($"Bitmex status {status} is not supported");
        } 

    }
}
