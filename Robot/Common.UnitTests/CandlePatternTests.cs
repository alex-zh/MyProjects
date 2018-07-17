using System;
using System.Collections.Generic;
using System.Linq;
using Common.CandlePattern;
using Common.Classes.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests
{
    [TestClass]
    public class CandlePatternTests
    {
        [TestMethod]
        public void Test1()
        {
            var pattern = new CandlePattern.CandlePattern("");

            pattern.Add(0, (c) => c.Close > 3);
            pattern.Add(1, (c) => c.Close < 2);

            var candles = new List<Candle>()
            {
                new Candle {Close = 1},
                new Candle {Close = 4}, // first
                new Candle {Close = 1},
                new Candle {Close = 5},
                new Candle {Close = 5}, //second
                new Candle {Close = 1},
                new Candle {Close = 2}
            };

            IEnumerable<int> result = new CandlePatternFinder().FindAll(candles, pattern);

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, result.First());
            Assert.AreEqual(4, result.Skip(1).First());
        }

        [TestMethod]
        public void Test2()
        {
            var pattern = new CandlePattern.CandlePattern("");

            pattern.Add(0, c => c.Close > 3);
            pattern.Add(1, c => c.Close < 2);
            pattern.Add((c1, c2) => c1.Close - c2.Close > 3);

            var candles = new List<Candle>
            {
                new Candle {Close = 1},
                new Candle {Close = 4},
                new Candle {Close = 1},
                new Candle {Close = 5},
                new Candle {Close = 5}, //first
                new Candle {Close = 1},
                new Candle {Close = 2}
            };

            IEnumerable<int> result = new CandlePatternFinder().FindAll(candles, pattern);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(4, result.First());
        }

        [TestMethod]
        public void Test3()
        {
            var pattern = new CandlePattern.CandlePattern("");

            pattern.Add(0, c => c.Close > c.Open + 1);
            pattern.Add(0, c => c.Close < 4);
            pattern.Add(0, c => c.Open > 2);

            pattern.Add(1, c => c.Close < 2);
            pattern.Add(1, c => c.Open > 3);

            var candles = new List<Candle>
            {
                new Candle {Close = 5, Open = 1},
                new Candle {Close = 4, Open = 2},
                new Candle {Close = 3.6, Open = 2.5}, //first
                new Candle {Close = 1, Open = 4},
                new Candle {Close = 3.6, Open = 2.5},
                new Candle {Close = 2, Open = 4},
                new Candle {Close = 3.6, Open = 2.5},
                new Candle {Close = 1, Open = 2},
            };

            IEnumerable<int> result = new CandlePatternFinder().FindAll(candles, pattern);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(2, result.First());
        }
    }
}
