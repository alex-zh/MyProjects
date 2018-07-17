using System;
using System.Collections.Generic;
using Common.Caclulators;
using Common.Classes.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests
{
    [TestClass]
    public class CandlesStatisticsCalculatorTests
    {
        [TestMethod]
        public void Test1()
        {
            var candles = new List<Candle>()
            {
                new Candle {Open = 10, Close = 20, High = 25, Low = 0},
                new Candle {Open = 10, Close = 20, High = 25, Low = 0},
                new Candle {Open = 10, Close = 20, High = 25, Low = 0},
            };

            var  result = new CandlesStatisticsCalculator(candles).Calculate();

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(10, result.Body.Mean);
            Assert.AreEqual(10, result.Body.Median);

            Assert.AreEqual(5, result.TailsUp.Mean);
            Assert.AreEqual(5, result.TailsUp.Median);

            Assert.AreEqual(10, result.TailsDown.Mean);
            Assert.AreEqual(10, result.TailsDown.Median);

        }
    }
}
