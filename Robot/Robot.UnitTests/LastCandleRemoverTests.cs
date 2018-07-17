using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Core.Classes;

namespace Robot.UnitTests
{
    [TestClass]
    public class LastCandleRemoverTests
    {
        [TestMethod]
        public void Test1()
        {
            var date = new DateTime(2015, 7, 27, 20, 4, 50);

            var rawCandles = new List<Candle>()
            {
                new Candle() {Date = date, Close = 10},
                new Candle() {Date = date.AddSeconds(-5), Close = 4},
            };

            var candles = LastCandleRemover.Remove(rawCandles, false);
            Assert.AreEqual(1, candles.Count);
            Assert.AreEqual(10, candles.First().Close);
        }

        [TestMethod]
        public void Test2()
        {
            var date = new DateTime(2015, 7, 27, 20, 4, 50);

            var rawCandles = new List<Candle>()
            {
                new Candle() {Date = date, Close = 10},
                new Candle() {Date = date.AddSeconds(1), Close = 20},
            };

            var candles = LastCandleRemover.Remove(rawCandles, false);
            Assert.AreEqual(1, candles.Count);
            Assert.AreEqual(10, candles.First().Close);
        }
    }
}
