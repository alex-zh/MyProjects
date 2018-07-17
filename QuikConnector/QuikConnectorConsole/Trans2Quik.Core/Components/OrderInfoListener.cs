using System;
using System.Runtime.InteropServices;
using Trans2Quik.Core.Internals;

namespace Trans2Quik.Core.Components
{
    internal class OrderInfoListener
    {
        private readonly EntryPoint.OrderStatusCallback _orderStatusCallback;

        public ReturnValue LastResult { get; private set; }
        public string ClassCode { get; private set; }
        public string SecurityCode { get; private set; }

        public event EventHandler<OrderInfoEventArgs> OrderStatusChanged;

        public OrderInfoListener()
        {
            this._orderStatusCallback = OrderStatusCallback;
            GCHandle.Alloc(_orderStatusCallback);
        }

        public bool Subscribe(string classCode, string securityCode)
        {
            ClassCode = classCode;
            SecurityCode = securityCode;
            this.LastResult = EntryPoint.SubscribeOrders(this.ClassCode, this.SecurityCode);
            return LastResult.IsSuccess;
        }
        public bool Unsubscribe()
        {
            LastResult = EntryPoint.UnsubscribeOrders();
            return LastResult.IsSuccess;
        }
        public bool StartOrders()
        {
            LastResult = EntryPoint.StartOrders(_orderStatusCallback);
            return LastResult.IsSuccess;
        }

        private void OrderStatusCallback(
                Int32 mode,
                Int32 transId,
                Double number,
                [MarshalAs(UnmanagedType.LPStr)] string classCode,
                [MarshalAs(UnmanagedType.LPStr)] string secCode,
                Double price,
                Int32 balance,
                Double value,
                Int32 isSell,
                Int32 status,
                Int32 orderDescriptor)
        {
            var order = new OrderInfo()
            {
                Balance = balance,
                ClassCode = classCode,
                Direction = TypeConverter.GetDirection(isSell),
                Mode = (OrderInfoMode) mode,
                Number = number,
                OrderDescriptor = orderDescriptor,
                Price = price,
                SecCode = secCode,
                Status = TypeConverter.GetStatus(status),
                TransId = transId,
                Value = value
            };

            var orderDetails = OrderInfoDetails.Fetch(order.OrderDescriptor);
            this.OnOrderStatusChanged(new OrderInfoEventArgs(order, orderDetails));
        }

        protected virtual void OnOrderStatusChanged(OrderInfoEventArgs args)
        {
            var tmp = this.OrderStatusChanged;

            if (tmp != null)
            {
                tmp(this, args);
            }
        }
    }
}
