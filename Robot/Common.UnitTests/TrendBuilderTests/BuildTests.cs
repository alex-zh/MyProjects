using System;
using System.Collections.Generic;
using Common.Classes.Builders;
using Common.Classes.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests.TrendBuilderTests
{
    [TestClass]
    public class BuildTests
    {
        private const int Year = 1900;
        private const int Month = 1;

        [TestMethod]
        public void Test1()
        {
            var candles = new List<Candle>()
            {
                new Candle {Open = 10, Close = 15, Date = new DateTime(Year, Month, 1)},
                new Candle {Open = 15, Close = 20, Date = new DateTime(Year, Month, 2)},

                new Candle {Open = 20, Close = 25, Date = new DateTime(Year, Month, 3)},
                new Candle {Open = 25, Close = 10, Date = new DateTime(Year, Month, 4)},
            };

            var options = new TrendBuilderOptions
            {
                IsForTest = true,
                LevelUpThreshold = 1,
                TestTrendCoefficients = new List<double>() { 1, 0.5 },
                TestLookupWindowCoefficients = new List<double>() { 2, 2 },
                AverageOneDayMove = 10,
            };

            var builder = new TrendBuilderNew(candles, options);
            var result = builder.Build();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(0, result[0].StartIndex);
            Assert.AreEqual(2, result[0].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[0].TrendType);

            Assert.AreEqual(3, result[1].StartIndex);
            Assert.AreEqual(3, result[1].EndIndex);
            Assert.AreEqual(TrendTypes.Down, result[1].TrendType);
        }

        [TestMethod]
        public void Test2()
        {
            var candles = new List<Candle>()
            {
                new Candle {Open = 10, Close = 15, Date = new DateTime(Year, Month, 1)},
                new Candle {Open = 15, Close = 20, Date = new DateTime(Year, Month, 2)},

                new Candle {Open = 20, Close = 25, Date = new DateTime(Year, Month, 3)},
                new Candle {Open = 25, Close = 10, Date = new DateTime(Year, Month, 4)},
            };

            var options = new TrendBuilderOptions
            {
                IsForTest = true,
                TestTrendCoefficients = new List<double>() { 1, 0.5 },
                TestLookupWindowCoefficients = new List<double>() { 2, 2 },
                AverageOneDayMove = 10,
            };

            var builder = new TrendBuilderNew(candles, options);
            var result = builder.Build();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(0, result[0].StartIndex);
            Assert.AreEqual(2, result[0].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[0].TrendType);

            Assert.AreEqual(3, result[1].StartIndex);
            Assert.AreEqual(3, result[1].EndIndex);
            Assert.AreEqual(TrendTypes.Down, result[1].TrendType);
        }
    }
}
