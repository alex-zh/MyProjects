using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BitMexConnector.Parsers.OrderBookL2.DataItems;
using BitMexConnector.Tables.Classes;
using Common.Classes.General;
using Common.Robots.Common;

namespace BitMexConnector.Tables
{
    public class OrderBook
    {
        private ConcurrentDictionary<long, OrderBookEntry> _bids = new ConcurrentDictionary<long, OrderBookEntry>();
        private ConcurrentDictionary<long, OrderBookEntry> _asks = new ConcurrentDictionary<long, OrderBookEntry>();

         
        readonly Func<PartialDataItem, KeyValuePair<long, OrderBookEntry>> _convertToKeyValue =
            x => new KeyValuePair<long, OrderBookEntry>(x.Id, new OrderBookEntry(x.Id, x.Size, x.Price));

        public void Init(IEnumerable<PartialDataItem> bids, IEnumerable<PartialDataItem> asks)
        {
            _bids = new ConcurrentDictionary<long, OrderBookEntry>(bids.Select(_convertToKeyValue));
            _asks = new ConcurrentDictionary<long, OrderBookEntry>(asks.Select(_convertToKeyValue));
        }

        public IEnumerable<OrderBookEntry> Bids
        {
            get { return _bids.Values.OrderByDescending(x => x.Price); }
        }

        public IEnumerable<OrderBookEntry> Asks
        {
            get { return _asks.Values.OrderBy(x => x.Price); }
        }

        public List<Quote> GetQuotes(int levelsNumber)
        {
            var result = new List<Quote>();

            var asks = _asks.Values.OrderBy(x => x.Price).ToList();
            var bids = _bids.Values.OrderByDescending(x => x.Price).ToList();

            for (int i = 0; i < Math.Min(levelsNumber, Math.Min(bids.Count, asks.Count)); i++)
            {
                result.Add(new Quote
                {
                    Ask = new QuoteItem { Price = asks[i].Price, Size = asks[i].Size},
                    Bid = new QuoteItem { Price = bids[i].Price, Size = bids[i].Size }
                });
            }

            return result;
        }

        public void Update(IEnumerable<UpdateDataItem> items, OrderSides side)
        {
            foreach (var entry in items)
            {
                if (side == OrderSides.Buy)
                {
                    var updatedEntry = new OrderBookEntry(entry.Id, entry.Size, _bids[entry.Id].Price);

                    _bids.TryUpdate(entry.Id, updatedEntry, _bids[entry.Id]);
                }
                else if (side == OrderSides.Sell)
                {
                    var updatedEntry = new OrderBookEntry(entry.Id, entry.Size, _asks[entry.Id].Price);

                    _asks.TryUpdate(entry.Id, updatedEntry, _asks[entry.Id]);
                }
            }
        }

        public void Insert(IEnumerable<InsertDataItem> items, OrderSides side)
        {
            foreach (var item in items)
            {
                if (side == OrderSides.Buy)
                {
                    _bids.TryAdd(item.Id, new OrderBookEntry(item.Id, item.Size, item.Price));
                }
                else if (side == OrderSides.Sell)
                {
                    _asks.TryAdd(item.Id, new OrderBookEntry(item.Id, item.Size, item.Price));
                }
            }
        }

        public void Delete(IEnumerable<DeleteDataItem> items, OrderSides side)
        {
            foreach (var entry in items)
            {
                OrderBookEntry entryToRemove;

                if (side == OrderSides.Buy)
                {
                    _bids.TryRemove(entry.Id, out entryToRemove);
                }
                else if (side == OrderSides.Sell)
                {
                    _asks.TryRemove(entry.Id, out entryToRemove);
                }
            }
        }


    }
}