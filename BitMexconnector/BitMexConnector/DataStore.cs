using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BitMexConnector.Tables;
using BitMexConnector.Tables.Classes;

namespace BitMexConnector
{
    public class DataStore
    {
        private DateTime _lastUpdateTime = DateTime.MinValue;

        public event EventHandler<EventArgs> TradesChanged;
        public event EventHandler<EventArgs> OrdersChanged;

        private Dictionary<string, OrderBook> _instruments = new Dictionary<string, OrderBook>();
        private readonly Dictionary<string, OrderBook> _orderBooks = new Dictionary<string, OrderBook>();
        private readonly Dictionary<string, CandlesStore> _candles = new Dictionary<string, CandlesStore>();

        private ConcurrentDictionary<string, BitMexTrade> _trades = new ConcurrentDictionary<string, BitMexTrade>();
        private ConcurrentDictionary<string, BitMexLimitOrder> _orders = new ConcurrentDictionary<string, BitMexLimitOrder>();


        public DataStore(IEnumerable<string> symbols)
        {
            foreach (var symbol in symbols)
            {
                _orderBooks[symbol] = new OrderBook();
            }
        }

        public DateTime LastUpdateTime
        {
            get { return _lastUpdateTime; }
            set { _lastUpdateTime = value; }
        }

        public Dictionary<string, CandlesStore> Candles => _candles;

        public ConcurrentDictionary<string, BitMexLimitOrder> Orders => _orders;

        public ConcurrentDictionary<string, BitMexTrade> Trades => _trades;

        public Dictionary<string, OrderBook> OrderBooks => _orderBooks;

        public void OnOrdersChanged()
        {
            OrdersChanged?.Invoke(this, EventArgs.Empty);
        }

        public void OnTradesChanged()
        {
            TradesChanged?.Invoke(this, EventArgs.Empty);
        }


    }
}