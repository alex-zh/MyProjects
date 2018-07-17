using System.Collections.Generic;
using System.Linq;
using BitMexConnector.DataStoreUpdators;
using BitMexConnector.Parsers.OrderBookL2;
using BitMexConnector.Parsers.OrderBookL2.DataItems;
using BitMexConnector.Tables.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace BitMexConnector.UnitTests.Parsers
{
    [TestClass]
    public class OrderBookL2ParserTests
    {
        private DataStore _store;
        private TestLogger _logger;
        private OrderBookUpdator _updator;
        private DataProvider _dataProvider;
        private OrderBookL2Parser _l2Parser;
        private OrderBookL2TestDataUpdator _testDataUpdator;


        [TestInitialize]
        public void Init()
        {
            _store = new DataStore(new[] { Symbols.XbtUsd });
            _logger = new TestLogger();
            _updator = new OrderBookUpdator(_store);
            _dataProvider = new DataProvider(_store);
            _l2Parser = new OrderBookL2Parser(_updator, _logger);
            _testDataUpdator = new OrderBookL2TestDataUpdator(SymbolTables.OrderBookL2);
        }

        [TestMethod]
        public void Parse_Partial()
        {
            _l2Parser.Parse(JObject.Parse(TestOrderBookL2Parser.OrderBookL2_Partial));

            var orderBook = _dataProvider.GetOrderBook(Symbols.XbtUsd);
            var asks = orderBook.Asks.ToList();
            var bids = orderBook.Bids.ToList();

            Assert.AreEqual(5, asks.Count);
            Assert.AreEqual(5, bids.Count);

            Assert.AreEqual(1, asks[0].Id); Assert.AreEqual(1200, asks[0].Size); Assert.AreEqual(12000, asks[0].Price);
            Assert.AreEqual(2, asks[1].Id); Assert.AreEqual(1100, asks[1].Size); Assert.AreEqual(11000, asks[1].Price);
            Assert.AreEqual(3, asks[2].Id); Assert.AreEqual(1000, asks[2].Size); Assert.AreEqual(10000, asks[2].Price);
            Assert.AreEqual(4, asks[3].Id); Assert.AreEqual(900, asks[3].Size); Assert.AreEqual(9000, asks[3].Price);
            Assert.AreEqual(5, asks[4].Id); Assert.AreEqual(800, asks[4].Size); Assert.AreEqual(8000, asks[4].Price);

            Assert.AreEqual(1, bids[0].Id); Assert.AreEqual(1200, bids[0].Size); Assert.AreEqual(12000, bids[0].Price);
            Assert.AreEqual(2, bids[1].Id); Assert.AreEqual(1100, bids[1].Size); Assert.AreEqual(11000, bids[1].Price);
            Assert.AreEqual(3, bids[2].Id);
            Assert.AreEqual(4, bids[3].Id);
            Assert.AreEqual(5, bids[4].Id);
        }

        [TestMethod]
        public void Parse_Partial_then_Update()
        {
            _l2Parser.Parse(JObject.Parse(TestOrderBookL2Parser.OrderBookL2_Partial));

            var updateJson = _testDataUpdator.GetJson(Symbols.XbtUsd, new List<UpdateDataItem>()
            {
                new UpdateDataItem {Id = 1, Side = OrderBookSides.Sell, Size = 1400},
                new UpdateDataItem {Id = 2, Side = OrderBookSides.Sell, Size = 1500 },
                new UpdateDataItem {Id = 1, Side = OrderBookSides.Buy, Size = 2800},
                new UpdateDataItem {Id = 2, Side = OrderBookSides.Buy, Size = 3000 },
            });

            _l2Parser.Parse(JObject.Parse(updateJson));

            //delete%
            //{"table":"orderBookL2","action":"delete","data":[{"symbol":"XBTUSD","id":15599147000,"side":"Sell"}]}

            var orderBook = _dataProvider.GetOrderBook(Symbols.XbtUsd);


            var asks = orderBook.Asks.ToList();
            var bids = orderBook.Bids.ToList();

            Assert.AreEqual(5, asks.Count);
            Assert.AreEqual(5, bids.Count);

            Assert.AreEqual(1, asks[0].Id); Assert.AreEqual(2800, asks[0].Size); Assert.AreEqual(12000, asks[0].Price);
            Assert.AreEqual(2, asks[1].Id); Assert.AreEqual(3000, asks[1].Size); Assert.AreEqual(11000, asks[1].Price);

            Assert.AreEqual(1, bids[0].Id); Assert.AreEqual(1400, bids[0].Size); Assert.AreEqual(12000, bids[0].Price);
            Assert.AreEqual(2, bids[1].Id); Assert.AreEqual(1500, bids[1].Size); Assert.AreEqual(11000, bids[1].Price);

        }

        [TestMethod]
        public void Parse_Partial_then_Insert_then_Update()
        {
            _l2Parser.Parse(JObject.Parse(TestOrderBookL2Parser.OrderBookL2_Partial));

            var insertJson = _testDataUpdator.GetJson(Symbols.XbtUsd, new List<InsertDataItem>()
            {
                new InsertDataItem() {Id = 6, Side = OrderBookSides.Sell, Size = 600, Price = 6000 },
                new InsertDataItem() {Id = 7, Side = OrderBookSides.Sell, Size = 700, Price = 7000 }
            });

            _l2Parser.Parse(JObject.Parse(insertJson));

            var updateJson = _testDataUpdator.GetJson(Symbols.XbtUsd, new List<UpdateDataItem>()
            {
                new UpdateDataItem() {Id = 2, Side = OrderBookSides.Sell, Size = 1150},
                new UpdateDataItem() {Id = 6, Side = OrderBookSides.Sell, Size = 650 },
                new UpdateDataItem() {Id = 7, Side = OrderBookSides.Sell, Size = 750}
            });

            _l2Parser.Parse(JObject.Parse(updateJson));

            var orderBook = _dataProvider.GetOrderBook(Symbols.XbtUsd);


            var asks = orderBook.Asks.ToList();
            var bids = orderBook.Bids.ToList();

            Assert.AreEqual(5, asks.Count);
            Assert.AreEqual(7, bids.Count);

            var item = bids.FirstOrDefault(x => x.Id == 2);

            Assert.AreEqual(1150, item.Size);
            Assert.AreEqual(11000, item.Price);

            item = bids.FirstOrDefault(x => x.Id == 6);

            Assert.AreEqual(650, item.Size);
            Assert.AreEqual(6000, item.Price);

            item = bids.FirstOrDefault(x => x.Id == 7);

            Assert.AreEqual(750, item.Size);
            Assert.AreEqual(7000, item.Price);
        }
    }
}
