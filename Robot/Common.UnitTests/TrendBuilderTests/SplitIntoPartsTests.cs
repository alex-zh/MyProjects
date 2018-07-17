using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.Builders;
using Common.Classes.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests.TrendBuilderTests
{
    [TestClass]
    public class TrendBuilderTests
    {
        private const int Year = 1900;
        private const int Month = 1;

        [TestMethod]
        public void Test1()
        {
            var candles = new List<Candle>()
            {
                new Candle {Open = 10, Close = 20, Date = new DateTime(Year, Month, 1)},
                new Candle {Open = 20, Close = 30, Date = new DateTime(Year, Month, 2)},
                new Candle {Open = 30, Close = 40, Date = new DateTime(Year, Month, 3)},
            };

            var options = new TrendBuilderOptions
            {
                IsForTest = true,
                AverageOneDayMove = 10,
                LevelUpThreshold = 1.0,
                TestTrendCoefficients = new[] { 1.0 }.ToList(),
                TestLookupWindowCoefficients = new[] {1.0}.ToList()                
            };

            var result = new TrendBuilderNew(candles, options).Build();

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(0, result[0].StartIndex);
            Assert.AreEqual(0, result[0].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[0].TrendType);

            Assert.AreEqual(1, result[1].StartIndex);
            Assert.AreEqual(1, result[1].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[1].TrendType);

            Assert.AreEqual(2, result[2].StartIndex);
            Assert.AreEqual(2, result[2].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[2].TrendType);     
        }

        [TestMethod]
        public void Test2()
        {
            var candles = new List<Candle>()
            {
                new Candle {Open = 10, Close = 20, Date = new DateTime(Year, Month, 1)},
                new Candle {Open = 20, Close = 30, Date = new DateTime(Year, Month, 2)},
                new Candle {Open = 30, Close = 20, Date = new DateTime(Year, Month, 3)},
            };

            var options = new TrendBuilderOptions
            {
                IsForTest = true,
                AverageOneDayMove = 10,
                LevelUpThreshold = 1.0,
                TestTrendCoefficients = new[] { 1.0 }.ToList(),
                TestLookupWindowCoefficients = new[] { 1.0 }.ToList()
            };

            var result = new TrendBuilderNew(candles, options).Build();            

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(0, result[0].StartIndex);
            Assert.AreEqual(0, result[0].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[0].TrendType);

            Assert.AreEqual(1, result[1].StartIndex);
            Assert.AreEqual(1, result[1].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[1].TrendType);

            Assert.AreEqual(2, result[2].StartIndex);
            Assert.AreEqual(2, result[2].EndIndex);
            Assert.AreEqual(TrendTypes.Down, result[2].TrendType);
        }

        [TestMethod]
        public void Test3()
        {
            var candles = new List<Candle>()
            {
                new Candle {Open = 10, Close = 20, Date = new DateTime(Year, Month, 1)},
                new Candle {Open = 10, Close = 20, Date = new DateTime(Year, Month, 2)},
                new Candle {Open = 20, Close = 30, Date = new DateTime(Year, Month, 3)},
            };

            var options = new TrendBuilderOptions
            {
                IsForTest = true,
                AverageOneDayMove = 20,
                LevelUpThreshold  = 1,
                TestTrendCoefficients = new[] { 1.0 }.ToList(),
                TestLookupWindowCoefficients = new[] { 2.0 }.ToList()
            };

            var result = new TrendBuilderNew(candles, options).Build();  

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(0, result[0].StartIndex);
            Assert.AreEqual(0, result[0].EndIndex);
            Assert.AreEqual(TrendTypes.Sideward, result[0].TrendType);

            Assert.AreEqual(1, result[1].StartIndex);
            Assert.AreEqual(2, result[1].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[1].TrendType);
        }

        [TestMethod]
        public void Test4()
        {
            var candles = new List<Candle>()
            {
                new Candle {Open = 10, Close = 20, Date = new DateTime(Year, Month, 1)},
                new Candle {Open = 20, Close = 30, Date = new DateTime(Year, Month, 2)},
                new Candle {Open = 30, Close = 20, Date = new DateTime(Year, Month, 3)},
            };

            var options = new TrendBuilderOptions
            {
                IsForTest = true,
                LevelUpThreshold = 1,
                AverageOneDayMove = 20,
                TestTrendCoefficients = new[] { 1.0 }.ToList(),
                TestLookupWindowCoefficients = new[] { 2.0 }.ToList()
            };

            var result = new TrendBuilderNew(candles, options).Build();  

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(0, result[0].StartIndex);
            Assert.AreEqual(1, result[0].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[0].TrendType);

            Assert.AreEqual(2, result[1].StartIndex);
            Assert.AreEqual(2, result[1].EndIndex);
            Assert.AreEqual(TrendTypes.Sideward, result[1].TrendType);
        }

        [TestMethod]
        public void Test5()
        {
            var candles = new List<Candle>()
            {
                new Candle {Open = 10, Close = 20}, new Candle {Open = 20, Close = 30},
                new Candle {Open = 30, Close = 20}, new Candle {Open = 20, Close = 12}, new Candle {Open = 11, Close = 15}, new Candle {Open = 16, Close = 8},
                new Candle {Open = 8, Close = 30}
            };

            var options = new TrendBuilderOptions
            {
                IsForTest = true,
                LevelUpThreshold = 1,
                AverageOneDayMove = 20,
                TestTrendCoefficients = new[] { 1.0 }.ToList(),
                TestLookupWindowCoefficients = new[] { 4.0 }.ToList()
            };

            var result = new TrendBuilderNew(candles, options).Build();  
           
            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(0, result[0].StartIndex);
            Assert.AreEqual(1, result[0].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[0].TrendType);

            Assert.AreEqual(2, result[1].StartIndex);
            Assert.AreEqual(5, result[1].EndIndex);
            Assert.AreEqual(TrendTypes.Down, result[1].TrendType);

            Assert.AreEqual(6, result[2].StartIndex);
            Assert.AreEqual(6, result[2].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[2].TrendType);
        }

        [TestMethod]
        public void Test6()
        {
            var candles = new List<Candle>()
            {
                new Candle {Open = 10, Close = 20}, new Candle {Open = 20, Close = 30},
                new Candle {Open = 30, Close = 20}, new Candle {Open = 20, Close = 12}, new Candle {Open = 11, Close = 15}, new Candle {Open = 16, Close = 8},
                new Candle {Open = 8, Close = 30}
            };

            var options = new TrendBuilderOptions
            {
                IsForTest = true,
                LevelUpThreshold = 1,
                AverageOneDayMove = 20,
                TestTrendCoefficients = new[] { 1.0 }.ToList(),
                TestLookupWindowCoefficients = new[] { 2.0 }.ToList()
            };

            var result = new TrendBuilderNew(candles, options).Build();  

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(0, result[0].StartIndex);
            Assert.AreEqual(1, result[0].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[0].TrendType);

            Assert.AreEqual(2, result[1].StartIndex);
            Assert.AreEqual(5, result[1].EndIndex);
            Assert.AreEqual(TrendTypes.Sideward, result[1].TrendType);

            Assert.AreEqual(6, result[2].StartIndex);
            Assert.AreEqual(6, result[2].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[2].TrendType);
        }

        [TestMethod]
        public void OneMajorPartDown_OneStandardPartUp()
        {
            var candles = new List<Candle>()
            {
                new Candle {Open = 20, Close = 25}, new Candle {Open = 25, Close = 30},
                new Candle {Open = 30, Close = 25}, new Candle {Open = 25, Close = 18},
                new Candle {Open = 18, Close = 20},
                new Candle {Open = 20, Close = 15}, new Candle {Open = 15, Close = 8},
            };

            var options = new TrendBuilderOptions
            {
                IsForTest = true,
                LevelUpThreshold = 1,
                AverageOneDayMove = 10,
                TestTrendCoefficients = new[] { 1.0 }.ToList(),
                TestLookupWindowCoefficients = new[] { 2.0 }.ToList()
            };

            var result = new TrendBuilderNew(candles, options).Build();

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual(0, result[0].StartIndex);
            Assert.AreEqual(1, result[0].EndIndex);
            Assert.AreEqual(TrendTypes.Up, result[0].TrendType);

            Assert.AreEqual(2, result[1].StartIndex);
            Assert.AreEqual(3, result[1].EndIndex);
            Assert.AreEqual(TrendTypes.Down, result[1].TrendType);

            Assert.AreEqual(4, result[2].StartIndex);
            Assert.AreEqual(4, result[2].EndIndex);
            Assert.AreEqual(TrendTypes.Sideward, result[2].TrendType);

            Assert.AreEqual(5, result[3].StartIndex);
            Assert.AreEqual(6, result[3].EndIndex);
            Assert.AreEqual(TrendTypes.Down, result[3].TrendType);

        }

        }
}
