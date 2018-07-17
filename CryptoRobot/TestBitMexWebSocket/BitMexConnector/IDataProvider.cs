using System.Collections.Generic;
using BitMexConnector.Tables;
using BitMexConnector.Tables.Classes;

namespace BitMexConnector
{
    public interface IDataProvider
    {
        OrderBook GetOrderBook(string symbol);
        IEnumerable<LimitOrder> GetOrders();
    }
}