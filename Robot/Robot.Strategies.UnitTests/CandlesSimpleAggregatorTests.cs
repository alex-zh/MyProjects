using System.Linq;
using Common.Classes.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Strategies.Agents;

namespace Robot.Strategies.UnitTests
{
    [TestClass]
    public class CandlesSimpleAggregatorTests
    {
        [TestInitialize]
        public void Init()
        {
           
        }

        [TestMethod]
        public void Test1()
        {
            var candlesAggregator = new CandlesSimpleAggregator(2);

            var candles = new[]
            {
                new Candle {Open = 1, Close = 2},
                new Candle() {Open = 2, Close = 3},
                new Candle() {Open = 3, Close = 4},
                new Candle() {Open = 4, Close = 5}
            }.ToList();

            var result = candlesAggregator.Aggregate(candles);
            
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Open);
            Assert.AreEqual(3, result[0].Close);
            Assert.AreEqual(3, result[1].Open);
            Assert.AreEqual(5, result[1].Close);
        }

        [TestMethod]
        public void Test2()
        {
            var candlesAggregator = new CandlesSimpleAggregator(2);

            var candles = new[]
            {
                new Candle {Open = 0, Close = 1},
                new Candle {Open = 1, Close = 2},
                new Candle() {Open = 2, Close = 3},
                new Candle() {Open = 3, Close = 4},
                new Candle() {Open = 4, Close = 5}
            }.ToList();

            var result = candlesAggregator.Aggregate(candles);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Open);
            Assert.AreEqual(3, result[0].Close);
            Assert.AreEqual(3, result[1].Open);
            Assert.AreEqual(5, result[1].Close);
        }

        [TestMethod]
        public void Test3()
        {
            var candlesAggregator = new CandlesSimpleAggregator(3);

            var candles = new[]
            {
                new Candle {Open = -1, Close = 0},
                new Candle {Open = 0, Close = 1},
                new Candle {Open = 1, Close = 2},
                new Candle() {Open = 2, Close = -1},
                new Candle() {Open = -2, Close = 4},
                new Candle() {Open = 4, Close = 6}
            }.ToList();

            var result = candlesAggregator.Aggregate(candles);

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(-1, result[0].Open);
            Assert.AreEqual(2, result[0].Close);

            Assert.AreEqual(2, result[1].Open);
            Assert.AreEqual(6, result[1].Close);
        }
    }
}