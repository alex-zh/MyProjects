using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BitMexConnector.Tables;
using BitMexConnector.Tables.Classes;

namespace BitMexConnector
{
    public class DataStore
    {
        private Dictionary<string, OrderBook> _instruments = new Dictionary<string, OrderBook>();
        private ConcurrentDictionary<Guid, LimitOrder> _orders = new ConcurrentDictionary<Guid, LimitOrder>();

        private readonly Dictionary<string, OrderBook> _orderBooks = new Dictionary<string, OrderBook>();

        public DataStore(IEnumerable<string> symbols)
        {
            foreach (var symbol in symbols)
            {
                _orderBooks[symbol] = new OrderBook();
            }
        }

        public ConcurrentDictionary<Guid, LimitOrder> Orders => _orders;

        public Dictionary<string, OrderBook> OrderBooks => _orderBooks;

    }
}