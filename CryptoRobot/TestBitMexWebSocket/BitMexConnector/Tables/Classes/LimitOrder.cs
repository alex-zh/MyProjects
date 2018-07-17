﻿using System;
using BitMexConnector.Parsers.OrdersTable;

namespace BitMexConnector.Tables.Classes
{
    public class LimitOrder
    {
        /*  Generated by system  */
        public Guid OrderId { get; set; }

        /* Generated by client and is being passed during communication
         when set up via web site is empty string */

        public string ClientOrderId { get; set; }

        public string Symbol { get; set; }

        public double Size { get; set; }

        /*  Market order has null price */
        public double? Price { get; set; }

        public OrderStatuses OrderStatus { get; set; }

        public OrderTypes OrderType { get; set; }
    }
}