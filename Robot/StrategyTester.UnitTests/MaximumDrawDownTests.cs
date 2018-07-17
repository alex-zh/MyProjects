using Common.Classes.StrategyTester;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StrategyTester.UnitTests
{
    [TestClass]
    public  class MaximumDrawDownTests
    {
        [TestMethod]
        public void Test1()
        {
            var data = new double[] {1, 2, 4, -2, -3, 2, 4};

            Assert.AreEqual(-5, TradeDataAnalyzer.CalculateMaximumDrawdown(data));
        }

        [TestMethod]
        public void Test2()
        {
            var data = new double[] { 1, 2, 4, -2, -3, 2, 4, -4, 1, -3, -1, 1 };

            Assert.AreEqual(-7, TradeDataAnalyzer.CalculateMaximumDrawdown(data));
        }

        [TestMethod]
        public void Test3()
        {
            var data = new double[] { -1, -2, 4, -2, -3, 2, 4, -4, 1, -3, -1, 1, 10, -4, 2, -5, 1, -5, 12 };

            Assert.AreEqual(-11, TradeDataAnalyzer.CalculateMaximumDrawdown(data));
        }
    }
}
