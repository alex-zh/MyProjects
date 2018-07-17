using System.Collections.Generic;
using BitMexConnector.Parsers.OrdersTable.DataItems;
using BitMexConnector.Tables.Classes;

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
                _store.Orders.TryAdd(entry.OrderId, entry.ToLimitOrder());
            }            
        }

        public void Update(IEnumerable<InsertDataItem> items)
        {
            foreach (var entry in items)
            {
                _store.Orders.TryAdd(entry.OrderId, entry.ToLimitOrder());
            }
        }

        public void Update(IEnumerable<UpdateDataItem> items)
        {
            foreach (var entry in items)
            {
                //частичное исполненение, как приходит?                
                LimitOrder order;
                if (_store.Orders.TryGetValue(entry.OrderId, out order))
                {
                    order.OrderStatus = entry.OrderStatus;
                    _store.Orders.TryUpdate(order.OrderId, order, order);
                }
                else
                {
                    _logger.Error($" Order with id {entry.OrderId} was not found in store");
                }
            }
        }    

    }
}