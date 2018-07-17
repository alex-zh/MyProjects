using System.Collections.Generic;
using System.Linq;
using Common.Classes.StrategyTester;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests
{
    [TestClass]
    public class DealsAnalyzerTests
    {
        [TestMethod]
        public void Test1()
        {
            var deals = new List<Deal>()
            {
               new Deal {Direction = Direction.Buy, TradeItems = new []{ new TestTradeItem(){Price = 10, Size = 1}, new TestTradeItem(){Price = 15, Size = -1}}.ToList<ITradeItem>()},
               new Deal(){Direction = Direction.Buy, TradeItems = new []{ new TestTradeItem(){Price = 10, Size = 1}, new TestTradeItem(){Price = 20, Size = -1}}.ToList<ITradeItem>()},
               new Deal(){Direction = Direction.Buy, TradeItems = new []{ new TestTradeItem(){Price = 10, Size = 1}, new TestTradeItem(){Price = 25, Size = -1}}.ToList<ITradeItem>()},
            };

            var analyzer = new DealsAnalyzer(deals, 1);

            /* Доходность сделок: 50, 100, 150
               Sigma: 50,
               Количество сделок с доходностью больше 0: 3,
               Количество сделок с доходностью больше или равно 50: 3,
                Количество сделок с доходностью больше или равно 100: 2
             */
            Assert.AreEqual(3, analyzer.GetAllProfitableDealsNumber(0));
            Assert.AreEqual(3, analyzer.GetAllProfitableDealsNumber(1));
            Assert.AreEqual(2, analyzer.GetAllProfitableDealsNumber(2));

            Assert.AreEqual(3, analyzer.GetLongProfitableDealsNumber(0));
            Assert.AreEqual(3, analyzer.GetLongProfitableDealsNumber(1));
            Assert.AreEqual(2, analyzer.GetLongProfitableDealsNumber(2));

            Assert.AreEqual(0, analyzer.GetShortProfitableDealsNumber(0));
        }        

        [TestMethod]
        public void Test2()
        {
            var deals = new List<Deal>()
            {               
               new Deal(){Direction = Direction.Sell, TradeItems = new []{ new TestTradeItem(){Price = 10, Size = - 1}, new TestTradeItem(){Price = 5, Size = 1}}.ToList<ITradeItem>()},
               new Deal(){Direction = Direction.Sell, TradeItems = new []{ new TestTradeItem(){Price = 20, Size = -1}, new TestTradeItem(){Price = 10, Size = 1}}.ToList<ITradeItem>()},
               new Deal(){Direction = Direction.Sell, TradeItems = new []{ new TestTradeItem(){Price = 30, Size = -1}, new TestTradeItem(){Price = 15, Size = 1}}.ToList<ITradeItem>()},
            };

            var analyzer = new DealsAnalyzer(deals, 1);
            
            /* Доходность сделок: 50, 50, 50
                Sigma: 50,
                Количество сделок с доходностью больше 0: 3,
                Количество сделок с доходностью больше 50: 2,
                Количество сделок с доходностью больше 100: 1
                */      
            Assert.AreEqual(3, analyzer.GetAllProfitableDealsNumber(0));

            Assert.AreEqual(3, analyzer.GetShortProfitableDealsNumber(0));
            Assert.AreEqual(3, analyzer.GetShortProfitableDealsNumber(1));
            Assert.AreEqual(3, analyzer.GetShortProfitableDealsNumber(2));

            Assert.AreEqual(0, analyzer.GetLongProfitableDealsNumber(0));
        }

        [TestMethod]
        public void Test3()
        {
            var deals = new List<Deal>()
            {               
               new Deal(){Direction = Direction.Sell, TradeItems = new []{ new TestTradeItem(){Price = 25, Size = -1}, new TestTradeItem(){Price = 20, Size = 1}}.ToList<ITradeItem>()},
               new Deal(){Direction = Direction.Sell, TradeItems = new []{ new TestTradeItem(){Price = 30, Size = -1}, new TestTradeItem(){Price = 27, Size = 1}}.ToList<ITradeItem>()},
               new Deal(){Direction = Direction.Sell, TradeItems = new []{ new TestTradeItem(){Price = 60, Size = -1}, new TestTradeItem(){Price = 30, Size = 1}}.ToList<ITradeItem>()},
               new Deal(){Direction = Direction.Sell, TradeItems = new []{ new TestTradeItem(){Price = 30, Size = -1}, new TestTradeItem(){Price = 24, Size = 1}}.ToList<ITradeItem>()},

               new Deal(){Direction = Direction.Buy, TradeItems = new []{ new TestTradeItem(){Price = 10, Size = 1}, new TestTradeItem(){Price = 12, Size = -1}}.ToList<ITradeItem>()},
               new Deal(){Direction = Direction.Buy, TradeItems = new []{ new TestTradeItem(){Price = 10, Size = 1}, new TestTradeItem(){Price = 13, Size = -1}}.ToList<ITradeItem>()},
               new Deal(){Direction = Direction.Buy, TradeItems = new []{ new TestTradeItem(){Price = 12, Size = 1}, new TestTradeItem(){Price = 18, Size = -1}}.ToList<ITradeItem>()},
               new Deal(){Direction = Direction.Buy, TradeItems = new []{ new TestTradeItem(){Price = 10, Size = 1}, new TestTradeItem(){Price = 11, Size = -1}}.ToList<ITradeItem>()},
            };

            /* 
             Доходность сделок вниз: 20, 10, 50, 20, sigma = 17
             Доходность сделок вверх: 20, 30, 50, 10, sigma= 17.3
             Для всех Mean = 35, Sigma = 15
           */
            var analyzer = new DealsAnalyzer(deals, 1);

            Assert.AreEqual(8, analyzer.GetAllProfitableDealsNumber(0));
            Assert.AreEqual(2, analyzer.GetAllProfitableDealsNumber(2));

            Assert.AreEqual(4, analyzer.GetShortProfitableDealsNumber(0));
            Assert.AreEqual(3, analyzer.GetShortProfitableDealsNumber(1));
            Assert.AreEqual(1, analyzer.GetShortProfitableDealsNumber(2));

            Assert.AreEqual(4, analyzer.GetLongProfitableDealsNumber(0));
            Assert.AreEqual(3, analyzer.GetLongProfitableDealsNumber(1));
            Assert.AreEqual(2, analyzer.GetLongProfitableDealsNumber(1.5));
            Assert.AreEqual(1, analyzer.GetLongProfitableDealsNumber(2));
            Assert.AreEqual(0, analyzer.GetLongProfitableDealsNumber(3));

            
        }
    }
}
