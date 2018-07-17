using System.Collections.Generic;
using Common.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Strategies.TwoDaysVolatilityUpGrowth;

namespace Strategies.UnitTests
{
    [TestClass]
    public class TwoDaysVolatilityUpgrowthTests
    {
        [TestMethod]
        public void Test1()
        {
            var candles = new Candle[]{ new Candle{ Open = 1, Close= 3, Low=1, High=3}, 
                                        new Candle{ Open = 1, Close= 3, Low=1, High=3}, //Start candle
                                        new Candle{ Open = 3, Close= 4, Low=3, High=4},
                                        new Candle{ Open = 4, Close= 7, Low=4, High=7}}; //волатильность 2, двойная 4, т.е Close >= 3+2*2

            var strategy = new TwoDaysVolatilityUpgrowth(candles, 2, 0, 2, 2);

            var statistics = strategy.Run();

            Assert.AreEqual(1, statistics.Count);
        }

        [TestMethod]
        public void Test2()
        {
            var candles1 = new Candle[]{ new Candle{ Open = 1, Close= 3, Low=1, High=3}, 
                                        new Candle{ Open = 1, Close= 3, Low=1, High=3}, //Start candle
                                        new Candle{ Open = 3, Close= 4, Low=3, High=4},
                                        new Candle{ Open = 4, Close= 7, Low=4, High=7},}; //волатильность 2, двойная 4, т.е Close >= 3+2*2

            var candles2 = new Candle[]{ new Candle{ Open = 3, Close= 4, Low=3, High=4}, 
                                         new Candle{ Open = 3, Close= 4, Low=3, High=4},//Start candle
                                         new Candle{ Open = 4, Close= 5, Low=4, High=5},
                                         new Candle{ Open = 4, Close= 5, Low=4, High=5},
                                         new Candle{ Open = 5, Close= 6, Low=5, High=6},//волатильность 1, двойная 2, т.е Close >=(4+2)
                                       }; 

            var candles = new List<Candle>();

            candles.AddRange(candles1);
            candles.AddRange(candles2);

            var strategy = new TwoDaysVolatilityUpgrowth(candles.ToArray(), 2, 0, 2, 2);

            var statistics = strategy.Run();

            Assert.AreEqual(2, statistics.Count);

            Assert.AreEqual(3, statistics.Realizations[0].StartCandle.Close);
            Assert.AreEqual(7, statistics.Realizations[0].CenterConditionCandle.Close);

            Assert.AreEqual(4, statistics.Realizations[1].StartCandle.Close);
            Assert.AreEqual(6, statistics.Realizations[1].CenterConditionCandle.Close);
        }
    }
}
