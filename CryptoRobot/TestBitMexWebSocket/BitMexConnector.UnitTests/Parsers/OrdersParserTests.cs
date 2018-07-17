using System.Linq;
using BitMexConnector.DataStoreUpdators;
using BitMexConnector.Parsers.OrdersTable;
using Common.Robots.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace BitMexConnector.UnitTests.Parsers
{
    [TestClass]
    public class OrdersParserTests
    {
        private DataStore _store;
        private TestLogger _logger;
        private OrdersTableUpdator _updator;
        private DataProvider _dataProvider;
        private OrdersTableParser _ordersParser;


        [TestInitialize]
        public void Init()
        {
            _store = new DataStore(new[] { Symbols.BitMex.XBTUSD });
            _logger = new TestLogger();
            _updator = new OrdersTableUpdator(_store, _logger);
            _dataProvider = new DataProvider(_store);
            _ordersParser = new OrdersTableParser(_updator, _logger);
        }

        [TestMethod]
        public void Parse_Insert()
        {
            _ordersParser.Parse(JObject.Parse(TestData.OrdersParserTests.Orders_Insert));

            var orders = _dataProvider.GetOrders().ToList();

            Assert.AreEqual(2, orders.Count);

            var firstOrder = orders.First(x => x.OrderId.ToString() == "client-order-id-1");
            var secondOrder = orders.First(x => x.OrderId.ToString() == "client-order-id-2");

            //var firstOrder = orders.First(x => x.OrderId.ToString() == "74090a64-09cd-fdea-688b-33ead0387941");
            //var secondOrder = orders.First(x => x.OrderId.ToString() == "74090a64-09cd-fdea-688b-33ead0387942");

            Assert.AreEqual(10, firstOrder.Size);
            Assert.AreEqual(295000, secondOrder.Size);

            Assert.AreEqual(10500.5, firstOrder.Price);
            Assert.AreEqual(null, secondOrder.Price);

            Assert.AreEqual(OrderTypes.Limit, firstOrder.OrderType);
            Assert.AreEqual(OrderTypes.Market, secondOrder.OrderType);

            Assert.AreEqual(OrderStatuses.Active, firstOrder.OrderStatus);
            Assert.AreEqual(OrderStatuses.Active, secondOrder.OrderStatus);

        }
    }
}
