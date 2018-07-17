using System.Collections.Generic;
using Common.Classes.StrategyTester;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StrategyTester.UnitTests
{
    [TestClass]
    public class TradeDataAnalyzerTests
    {
        [TestMethod]
        public void TwoTradesOneDeal_Success()
        {
            var data = new List<TestTradeItem>
            {
                new TestTradeItem { Size = 1, Price = 1000},
                new TestTradeItem { Size = -1, Price = 1200}
            };

            var result = new TradeDataAnalyzer(data, 2).GetStatistics();

            Assert.AreEqual(1, result.NumberOfDealsAll);
            Assert.AreEqual(1, result.NumberOfDealsLong);
            Assert.AreEqual(0, result.NumberOfDealsShort);

            Assert.AreEqual(200, result.MoneyProfitAll);
            Assert.AreEqual(20, result.PercentAggregatedProfitAll);

            Assert.AreEqual(200, result.MoneyProfitLong);
            Assert.AreEqual(20, result.PercentAggregatedProfitLong);

            Assert.AreEqual(0, result.MoneyProfitShort);
            Assert.AreEqual(0, result.PercentAggregatedProfitShort);
        }

        [TestMethod]
        public void SixTradesTwoDeals_BothProfitable_Success()
        {
            var data = new List<TestTradeItem>
            {
                new TestTradeItem { Size = 2, Price = 1000},
                new TestTradeItem { Size = 1, Price = 1300},
                new TestTradeItem { Size = -1, Price = 1100},
                new TestTradeItem { Size = -2, Price = 1400},
                new TestTradeItem { Size = -2, Price = 1200},
                new TestTradeItem { Size = 2, Price = 800},
            };

            const double firstDealProfitInMoney = (2 * 1400 + 1100 - 2 * 1000 - 1300);
            const double secondDealProfitInMoney = (2 * 1200 - 2 * 800);

            const double firstDealStartPrice = (2 * 1000 + 1300) / 3;
            const double firstDealEndPrice = (1100 + 1400 * 2) / 3;
            const double firstDealProfitInPercent = (firstDealEndPrice - firstDealStartPrice) * 100 / firstDealStartPrice;

            const double secondDealStartPrice = 1200;
            const double secondDealEndPrice = 800;
            const double secondDealProfitInPercent = (secondDealStartPrice - secondDealEndPrice) * 100 / secondDealStartPrice;

            var result = new TradeDataAnalyzer(data, 3).GetStatistics();

            Assert.AreEqual(2, result.NumberOfDealsAll);
            Assert.AreEqual(1, result.NumberOfDealsLong);
            Assert.AreEqual(1, result.NumberOfDealsShort);


            Assert.AreEqual(firstDealProfitInMoney + secondDealProfitInMoney, result.MoneyProfitAll);
            Assert.AreEqual((int)(firstDealProfitInPercent + secondDealProfitInPercent), (int)result.PercentAggregatedProfitAll);

            Assert.AreEqual(firstDealProfitInMoney, result.MoneyProfitLong);
            Assert.AreEqual((int)firstDealProfitInPercent, (int)result.PercentAggregatedProfitLong);

            Assert.AreEqual(secondDealProfitInMoney, result.MoneyProfitShort);
            Assert.AreEqual((int)secondDealProfitInPercent, (int)result.PercentAggregatedProfitShort);

        }

        [TestMethod]
        public void ThreeDeals_OneProfitableTwoNot_Success()
        {
            var data = new List<TestTradeItem>
            {
                new TestTradeItem { Size = 3, Price = 1000},
                new TestTradeItem { Size = 1, Price = 1300},
                new TestTradeItem { Size = -2, Price = 1100},
                new TestTradeItem { Size = -1, Price = 1000},
                new TestTradeItem { Size = -1, Price = 800},

                new TestTradeItem { Size = -2, Price = 1400},
                new TestTradeItem { Size = -2, Price = 1600},
                new TestTradeItem { Size = 2, Price = 1800},
                new TestTradeItem { Size = 1, Price = 2000},
                new TestTradeItem { Size = 1, Price = 2200},

                new TestTradeItem { Size = -2, Price = 2200},
                new TestTradeItem { Size = 1, Price = 1800},
                new TestTradeItem { Size = 1, Price = 1600}
            };

            const double firstDealProfitInMoney = (2 * 1100 + 1000 + 800 - 3 * 1000 - 1300);
            const double secondDealProfitInMoney = (2 * 1400 + 2 * 1600 - 2 * 1800 - 2000 - 2200);
            const double thirdDealProfitInMoney = (2 * 2200 - 1800 - 1600);
            const double overallProfitInMoney = firstDealProfitInMoney + secondDealProfitInMoney + thirdDealProfitInMoney;

            const double firstDealStartPrice = (3 * 1000 + 1300) / 4;
            const double firstDealEndPrice = (2 * 1100 + 1000 + 800) / 4;
            const double firstDealProfitInPercent = (firstDealEndPrice - firstDealStartPrice) * 100 / firstDealStartPrice;

            const double secondDealStartPrice = (2 * 1400 + 2 * 1600) / 4;
            const double secondDealEndPrice = (2 * 1800 + 2000 + 2200) / 4;
            const double secondDealProfitInPercent = (secondDealStartPrice - secondDealEndPrice) * 100 / secondDealStartPrice;

            const double thirdDealStartPrice = (2 * 2200 + 2) / 2;
            const double thirdDealEndPrice = (1800 + 1600) / 2;
            const double thirdDealProfitInPercent = (thirdDealStartPrice - thirdDealEndPrice) * 100 / thirdDealStartPrice;

            const double overallProfitInPercent = firstDealProfitInPercent + secondDealProfitInPercent + thirdDealProfitInPercent;

            var result = new TradeDataAnalyzer(data, 4).GetStatistics();

            Assert.AreEqual(3, result.NumberOfDealsAll);
            Assert.AreEqual(1, result.NumberOfDealsLong);
            Assert.AreEqual(2, result.NumberOfDealsShort);

            Assert.AreEqual(overallProfitInMoney, result.MoneyProfitAll);
            Assert.AreEqual((int)(overallProfitInPercent), (int)result.PercentAggregatedProfitAll);

            Assert.AreEqual(firstDealProfitInMoney, result.MoneyProfitLong);
            Assert.AreEqual((int)firstDealProfitInPercent, (int)result.PercentAggregatedProfitLong);

            Assert.AreEqual(secondDealProfitInMoney + thirdDealProfitInMoney, result.MoneyProfitShort);
            Assert.AreEqual((int)(secondDealProfitInPercent + thirdDealProfitInPercent), (int)result.PercentAggregatedProfitShort);

        }

        [TestMethod]
        public void OverallDrawdown_Success()
        {
            var data = new List<TestTradeItem>
            {
                new TestTradeItem { Size = 2, Price = 1000},
                new TestTradeItem { Size = -1, Price = 1300},
                new TestTradeItem { Size = -1, Price = 1100},

                new TestTradeItem { Size = -1, Price = 1400},
                new TestTradeItem { Size = 1, Price = 1600},

                new TestTradeItem { Size = -1, Price = 1400},
                new TestTradeItem { Size = 1, Price = 1600},

                new TestTradeItem { Size = -1, Price = 1400},
                new TestTradeItem { Size = 1, Price = 1600},

                new TestTradeItem { Size = 1, Price = 1000},
                new TestTradeItem { Size = -1, Price = 2000}
            };

            var result = new TradeDataAnalyzer(data,2).GetStatistics();

            Assert.AreEqual(-600, result.MoneyDrawdownAll);
            Assert.AreEqual((int)(((double)-200 / 1400) * 3 * 100), (int)result.PercentDrawdownAll);
        }

        [TestMethod]
        public void ThereAreSeveralOverallDrawdowns_PickMaximum_Success()
        {
            var data = new List<TestTradeItem>
            {
                new TestTradeItem { Size = 2, Price = 1000},
                new TestTradeItem { Size = -1, Price = 1300},
                new TestTradeItem { Size = -1, Price = 1100},

                new TestTradeItem { Size = -1, Price = 1400},
                new TestTradeItem { Size = 1, Price = 1600},

                new TestTradeItem { Size = -1, Price = 1400},
                new TestTradeItem { Size = 1, Price = 1600},

                new TestTradeItem { Size = -1, Price = 1400},
                new TestTradeItem { Size = 1, Price = 1600},

                new TestTradeItem { Size = 1, Price = 1000},
                new TestTradeItem { Size = -1, Price = 2000},

                new TestTradeItem { Size = 1, Price = 1000},
                new TestTradeItem { Size = -1, Price = 300}
            };

            var result = new TradeDataAnalyzer(data,2).GetStatistics();

            Assert.AreEqual(-700, result.MoneyDrawdownAll);
            Assert.AreEqual((int)(((double)-700 / 1000) * 100), (int)result.PercentDrawdownAll);
        }
    }
}
