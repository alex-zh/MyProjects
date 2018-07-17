using System;
using System.Collections.Generic;
using System.Linq;
using BitMexConnector.Tables;
using BitMexConnector.Tables.Classes;

namespace BitMexConnector
{
    public class DataProvider: IDataProvider
    {
        private readonly DataStore _dataStore;

        public DataProvider(DataStore dataStore)
        {
            _dataStore = dataStore;
        }


        public OrderBook GetOrderBook(string symbol)
        {
            if (_dataStore.OrderBooks.ContainsKey(symbol))
                return _dataStore.OrderBooks[symbol];

            throw new NotSupportedException($"Order book for {symbol} is not found");
        }

        public IEnumerable<LimitOrder> GetOrders()
        {
            return _dataStore.Orders.Values;
        }
    }
}