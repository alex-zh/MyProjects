using System;
using System.Collections.Generic;
using Common.Caclulators;
using Common.Classes.General;
using Common.Finders;
using Common.Regression;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests
{
    [TestClass]
    public class TrendnessCalculatorTests
    {
        [TestMethod]
        public void Test1()
        {
            var candles = new List<Candle>()
            {
                new Candle {Close = 1}, new Candle {Close = 2}, new Candle { Close = 4},
                new Candle {Close = 3}, new Candle {Close = 2}, new Candle { Close = 1}
            };

            var calculator = new TrendnessCalculator(candles, x => x.Get(PriceTypes.Close));
            Assert.AreEqual(57, calculator.Calculate());
        }

        [TestMethod]
        public void FullTrend()
        {
            var candles = new List<Candle>()
            {
                new Candle {Close = 1}, new Candle {Close = 2}, new Candle { Close = 3},
                new Candle {Close = 4}, new Candle {Close = 5}, new Candle { Close = 6}
            };

            var calculator = new TrendnessCalculator(candles, x => x.Get(PriceTypes.Close));
            Assert.AreEqual(100, calculator.Calculate());
        }

        [TestMethod]
        public void ZeroTrend()
        {
            var candles = new List<Candle>()
            {
                new Candle {Close = 1}, new Candle {Close = 2}, new Candle { Close = 1},
                new Candle {Close = 2}, new Candle {Close = 1}, new Candle { Close = 2}
            };

            var calculator = new TrendnessCalculator(candles, x => x.Get(PriceTypes.Close));
            Assert.AreEqual(8, calculator.Calculate());
        }

        [TestMethod]
        public void ZeroTrend2()
        {
            var candles = new List<Candle>()
            {
                new Candle {Close = 1}, new Candle {Close = 2}, new Candle { Close = 1},
                new Candle {Close = 2}, new Candle {Close = 1}, new Candle { Close = 2},
                new Candle {Close = 1}, new Candle {Close = 2}, new Candle { Close = 1}
            };

            var calculator = new TrendnessCalculator(candles, x => x.Get(PriceTypes.Close));
            Assert.AreEqual(4, calculator.Calculate());
        }

        [TestMethod]
        public void ZeroTrend3()
        {
            var candles = new List<Candle>()
            {
                new Candle {Close = 1}, new Candle {Close = 2}, new Candle { Close = 1},
                new Candle {Close = 2}, new Candle {Close = 1}, new Candle { Close = 2},
                new Candle {Close = 1}, new Candle {Close = 2}, new Candle { Close = 1},
                new Candle {Close = 2}, new Candle {Close = 1}, new Candle { Close = 2}
            };

            var calculator = new TrendnessCalculator(candles, x => x.Get(PriceTypes.Close));
            Assert.AreEqual(2, calculator.Calculate());
        }


        [TestMethod]
        public void Test3()
        {
            var candles = new List<Candle>()
            {
                new Candle {Close = 1}, new Candle {Close = 2}, new Candle { Close = 3}, new Candle { Close = 4}, new Candle { Close = 5},
                new Candle {Close = 4}
            };

            var calculator = new TrendnessCalculator(candles, x => x.Get(PriceTypes.Close));
            Assert.AreEqual(64, calculator.Calculate());
        }

        [TestMethod]
        public void Test4()
        {
            var candles = new List<Candle>()
            {
                new Candle {Close = 1}, new Candle {Close = 2}, new Candle { Close = 3}, new Candle { Close = 4}, new Candle { Close = 5},
                new Candle {Close = 4}, new Candle {Close = 3}, new Candle {Close = 2}, new Candle {Close = 1}
            };

            var calculator = new TrendnessCalculator(candles, x => x.Get(PriceTypes.Close));
            Assert.AreEqual(61, calculator.Calculate());
        }

        [TestMethod]
        public void Test5()
        {
            var candles = new List<Candle>()
            {
                new Candle {Close = 1}, new Candle {Close = 2}, new Candle { Close = 3}, new Candle { Close = 4}, new Candle { Close = 5},
                new Candle {Close = 4}, new Candle {Close = 3}, new Candle {Close = 2}, new Candle {Close = 1}, new Candle {Close = 0},
                new Candle {Close = 1}, new Candle {Close = 2}, new Candle { Close = 3}, new Candle { Close = 4}, new Candle { Close = 5},
            };

            var calculator = new TrendnessCalculator(candles, x => x.Get(PriceTypes.Close));
            Assert.AreEqual(49, calculator.Calculate());
        }

    }
}
