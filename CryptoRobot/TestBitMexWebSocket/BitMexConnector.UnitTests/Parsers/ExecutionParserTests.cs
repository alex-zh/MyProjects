using System.Linq;
using BitMexConnector.DataStoreUpdators;
using BitMexConnector.Parsers.ExecutionTable;
using Common.Robots.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace BitMexConnector.UnitTests.Parsers
{
    [TestClass]
    public class ExecutionParserTests
    {
        private DataStore _store;
        private TestLogger _logger;
        private ExecutionTableUpdator _updator;
        private DataProvider _dataProvider;
        private ExecutionTableParser _parser;


        [TestInitialize]
        public void Init()
        {
            _store = new DataStore(new[] { Symbols.BitMex.XRPU18 });
            _logger = new TestLogger();
            _updator = new ExecutionTableUpdator(_store, _logger);
            _dataProvider = new DataProvider(_store);
            _parser = new ExecutionTableParser(_updator, _logger);
        }

        [TestMethod]
        public void Parse_Partial()
        {
            
        }

        [TestMethod]
        public void Parse_Insert()
        {
            _parser.Parse(JObject.Parse(TestData.ExecutionParserTests.Execution_Insert));

            var trades = _dataProvider.GetTrades().ToList();

            Assert.AreEqual(1, trades.Count);

            Assert.AreEqual("02042c2a-edf7-7c39-2949-a66be85683a7", trades[0].TradeId);
            Assert.AreEqual("e3ab7ab8-2cfb-4cb0-bc1a-57cccf9f0cb9", trades[0].OrderId);
            Assert.AreEqual("6ab772ce-7279-6ff6-0052-fbf75bc1f670", trades[0].SystemOrderId);
            Assert.AreEqual(7000, trades[0].Price);
            Assert.AreEqual(5, trades[0].Size);
            Assert.AreEqual(0, trades[0].LeftSize);
            Assert.AreEqual("2018-06-26T09:49:32", trades[0].DateTime.ToString("s"));
            
        }
   }
}
