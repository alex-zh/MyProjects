using System.Collections.Generic;
using System.Linq;
using BitMexConnector.Parsers.OrderBookL2.DataItems;
using Common.Robots.Common;

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

                var bids = group.Where(x => x.Side == OrderSides.Buy);
                var asks = group.Where(x => x.Side == OrderSides.Sell);

                orderBook.Init(bids, asks);
            }
        }

        public void Update(IEnumerable<UpdateDataItem> items)
        {
            var groupsBySymbol = items.GroupBy(x => x.Symbol);

            foreach (var group in groupsBySymbol)
            {
                var orderBook = _store.OrderBooks[group.Key];

                var bids = group.Where(x => x.Side == OrderSides.Buy).ToList();
                var asks = group.Where(x => x.Side == OrderSides.Sell).ToList();

                orderBook.Update(bids, OrderSides.Buy);
                orderBook.Update(asks, OrderSides.Sell);
            }

        }

        public void Update(IEnumerable<InsertDataItem> items)
        {
            var groupsBySymbol = items.GroupBy(x => x.Symbol);

            foreach (var group in groupsBySymbol)
            {
                var orderBook = _store.OrderBooks[group.Key];

                var bids = group.Where(x => x.Side == OrderSides.Buy).ToList();
                var asks = group.Where(x => x.Side == OrderSides.Sell).ToList();

                orderBook.Insert(bids, OrderSides.Buy);
                orderBook.Insert(asks, OrderSides.Sell);
            }

        }

        public void Update(IEnumerable<DeleteDataItem> items)
        {
            var groupsBySymbol = items.GroupBy(x => x.Symbol);

            foreach (var group in groupsBySymbol)
            {
                var orderBook = _store.OrderBooks[group.Key];

                var bids = group.Where(x => x.Side == OrderSides.Buy).ToList();
                var asks = group.Where(x => x.Side == OrderSides.Sell).ToList();

                orderBook.Delete(bids, OrderSides.Buy);
                orderBook.Delete(asks, OrderSides.Sell);
            }

        }
    }
}