using System.Collections.Generic;
using BitMexConnector.Parsers.OrdersTable;
using BitMexConnector.Parsers.OrdersTable.DataItems;
using BitMexConnector.Tables.Classes;
using Common.Classes.Logger;

namespace BitMexConnector.DataStoreUpdators
{
    public class OrdersTableUpdator
    {
        private readonly DataStore _store;
        private readonly ILogger _logger;

        public OrdersTableUpdator(DataStore store, ILogger logger)
        {
            _store = store;
            _logger = logger;
        }

        public void Update(IEnumerable<PartialDataItem> items)
        {
            foreach (var entry in items)
            {
                if (entry.WorkingIndicator)
                    continue;

                //// not robots orders
                if (string.IsNullOrEmpty(entry.ClientOrderId))
                    continue;

                _store.Orders.TryAdd(entry.ClientOrderId, entry.ToLimitOrder());

            }

            _store.OnOrdersChanged();
        }

        public void Update(IEnumerable<InsertDataItem> items)
        {
            foreach (var entry in items)
            {
                if (entry.WorkingIndicator)
                    continue;

                // not robots orders
                if (string.IsNullOrEmpty(entry.ClientOrderId))
                    continue;

                _store.Orders.TryAdd(entry.ClientOrderId, entry.ToLimitOrder());
            }

            _store.OnOrdersChanged();
        }

        public void Update(IEnumerable<UpdateDataItem> items)
        {
            foreach (var entry in items)
            {
                // some system update, actually no data
                if (entry.WorkingIndicator)
                    continue;

                // not robots orders
                if (string.IsNullOrEmpty(entry.ClientOrderId))
                    continue;

                /* Update can lead to order status change
                   simpleOrderSize will be set to 0 when order has been canceled */
                BitMexLimitOrder order;
                if (_store.Orders.TryGetValue(entry.ClientOrderId, out order))
                {
                    order.OrderStatus = Extensions.ToOrderStatus(entry.OrderStatus);
                    _store.Orders.TryUpdate(order.OrderId, order, order);
                }
                else
                {
                    _logger.Error($" Order with id {entry.ClientOrderId} was not found in store");
                }
            }

            _store.OnOrdersChanged();
        }

    }
}