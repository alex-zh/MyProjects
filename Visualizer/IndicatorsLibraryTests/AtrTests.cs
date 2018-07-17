using Common;
using Common.Classes;
using Common.Classes.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsLibraryTests
{
    [TestClass]
    public class AtrTests
    {
        [TestMethod]
        public void Test1()
        {
            var candles = new[]{ new Candle{ Low = 1, High= 3}, 
                                        new Candle{ Low = 1, High = 3, Close=1}, 
                                        new Candle{ Low = 1, High = 4, Close=1},
                                        new Candle{ Low = 1, High = 4, Close=1}};

            var result = Atr.Calculate(candles);

            Assert.AreEqual(3, result);            
        }

        [TestMethod]
        public void Test2()
        {
            var candles = new Candle[]{ new Candle{ Low = 1, High= 4, Close=4}, // 3
                                        new Candle{ Low = 5, Open = 5, High = 7, Close=6}, // 7-4 = 3 
                                        new Candle{ Low = 6, High = 12, Close=7}, // 6
                                        new Candle{ Low = 4, High = 6, Close=4}}; //2

            var result = Atr.Calculate(candles);

            Assert.AreEqual(4.5, result);
        }

        [TestMethod]
        public void Test3()
        {
            var candles = new[]{ new Candle{ Low = 1, High= 4, Close=4, Open=1}, // 3
                                        new Candle{ Low = 5, Open = 5, High = 7, Close=6}, // 7-4 = 3 
                                        new Candle{ Low = 6, High = 12, Close=7}, // 6
                                        new Candle{ Low = 6, High = 12, Close=7}, // 6
                                        new Candle{ Low = 4, High = 6, Close=4}}; //2

            var result = Atr.Calculate(candles);

            Assert.AreEqual(5, result);
        }
    }
}
