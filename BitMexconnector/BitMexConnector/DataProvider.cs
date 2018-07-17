using System;
using System.Collections.Generic;
using BitMexConnector.Tables;
using BitMexConnector.Tables.Classes;
using Common.Classes.General;

namespace BitMexConnector
{
    public class DataProvider: IDataProvider
    {
        public event EventHandler<EventArgs> TradesChanged;
        public event EventHandler<EventArgs> OrdersChanged;

        private readonly DataStore _dataStore;

        public DataProvider(DataStore dataStore)
        {
            _dataStore = dataStore;
            _dataStore.OrdersChanged += _dataStore_OrdersChanged;
            _dataStore.TradesChanged += _dataStore_TradesChanged;
        }        

        public DateTime GetLastServerDataTime()
        {
            return _dataStore.LastUpdateTime;
        }

        public OrderBook GetOrderBook(string symbol)
        {
            if (_dataStore.OrderBooks.ContainsKey(symbol))
                return _dataStore.OrderBooks[symbol];

            throw new NotSupportedException($"Order book for {symbol} is not found");
        }

        public IEnumerable<BitMexLimitOrder> GetOrders()
        {
            return _dataStore.Orders.Values;
        }

        public IEnumerable<BitMexTrade> GetTrades()
        {
            return _dataStore.Trades.Values;
        }

        public List<Quote> GetQuotes(string symbol, int levelsNumber)
        {
            var orderBook = GetOrderBook(symbol);
            return orderBook.GetQuotes(levelsNumber);
        }

        private void _dataStore_TradesChanged(object sender, EventArgs e)
        {
            TradesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void _dataStore_OrdersChanged(object sender, EventArgs e)
        {
            OrdersChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}