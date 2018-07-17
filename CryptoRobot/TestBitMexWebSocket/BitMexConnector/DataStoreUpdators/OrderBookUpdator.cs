using System.Collections.Generic;
using System.Linq;
using BitMexConnector.Parsers.OrderBookL2.DataItems;
using BitMexConnector.Tables.Classes;

namespace BitMexConnector.DataStoreUpdators
{
    public class OrderBookUpdator
    {
        private readonly DataStore _store;

        public OrderBookUpdator(DataStore store)
        {
            _store = store;
        }

        public void Update(IEnumerable<PartialDataItem> items)
        {
            var groupsBySymbol = items.GroupBy(x => x.Symbol);

            foreach (var group in groupsBySymbol)
            {
                var orderBook = _store.OrderBooks[group.Key];

                var bids = group.Where(x => x.Side == OrderBookSides.Buy);
                var asks = group.Where(x => x.Side == OrderBookSides.Sell);

                orderBook.Init(bids, asks);
            }
        }

        public void Update(IEnumerable<UpdateDataItem> items)
        {
            var groupsBySymbol = items.GroupBy(x => x.Symbol);

            foreach (var group in groupsBySymbol)
            {
                var orderBook = _store.OrderBooks[group.Key];

                var bids = group.Where(x => x.Side == OrderBookSides.Buy).ToList();
                var asks = group.Where(x => x.Side == OrderBookSides.Sell).ToList();

                orderBook.Update(bids, OrderBookSides.Buy);
                orderBook.Update(asks, OrderBookSides.Sell);
            }

        }

        public void Update(IEnumerable<InsertDataItem> items)
        {
            var groupsBySymbol = items.GroupBy(x => x.Symbol);

            foreach (var group in groupsBySymbol)
            {
                var orderBook = _store.OrderBooks[group.Key];

                var bids = group.Where(x => x.Side == OrderBookSides.Buy).ToList();
                var asks = group.Where(x => x.Side == OrderBookSides.Sell).ToList();

                orderBook.Insert(bids, OrderBookSides.Buy);
                orderBook.Insert(asks, OrderBookSides.Sell);
            }

        }

        public void Update(IEnumerable<DeleteDataItem> items)
        {
            var groupsBySymbol = items.GroupBy(x => x.Symbol);

            foreach (var group in groupsBySymbol)
            {
                var orderBook = _store.OrderBooks[group.Key];

                var bids = group.Where(x => x.Side == OrderBookSides.Buy).ToList();
                var asks = group.Where(x => x.Side == OrderBookSides.Sell).ToList();

                orderBook.Delete(bids, OrderBookSides.Buy);
                orderBook.Delete(asks, OrderBookSides.Sell);
            }

        }
    }
}