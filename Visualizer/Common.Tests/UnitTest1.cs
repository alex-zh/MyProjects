using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Common.Classes.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Tests
{
    [TestClass]
    public class ExtentionsTests
    {
        [TestMethod]
        public void TakeLastTwo()
        {
            var ints = new List<int>(){1, 2, 3, 4, 5};

            Assert.AreEqual(2, ints.TakeLast(2).Count());
            Assert.AreEqual(4, ints.TakeLast(2).ToList()[0]);
            Assert.AreEqual(5, ints.TakeLast(2).ToList()[1]);
        }

        [TestMethod]
        public void TakeSinceTest1()
        {
            var ints = new List<int>() { 1, 2, 3, 4, 5 };

            var result = ints.TakeRange(1, 3).ToList();

            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(2, result.ToList()[0]);
            Assert.AreEqual(3, result.ToList()[1]);
            Assert.AreEqual(4, result.ToList()[2]);

            result = ints.TakeRange(3, 4).ToList();

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(4, result.ToList()[0]);
            Assert.AreEqual(5, result.ToList()[1]);
        }

        [TestMethod]
        public void TakeSinceTest2()
        {
            var ints = new List<Candle>() { new Candle() { Close = 3 }, 
                                            new Candle() { Close = 2 },
                                            new Candle() { Close = 1 },
                                            new Candle() { Close = 4 }};

            var result = ints.TakeRange(0, 0).ToList();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(3, result.ToList()[0].Close);

            result = ints.TakeRange(3, 3).ToList();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(4, result.ToList()[0].Close);


            result = ints.TakeRange(0, 1).ToList();

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(3, result.ToList()[0].Close);
            Assert.AreEqual(2, result.ToList()[1].Close);
        }

        [TestMethod]
        public void TakeSinceTest3()
        {
            var ints = new List<Candle>() { new Candle() { Close = 3 }, 
                                            new Candle() { Close = 2 },
                                            new Candle() { Close = 1 },
                                            new Candle() { Close = 4 }};

            var result = ints.TakeRange(3, 3).ToList();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(4, result.ToList()[0].Close);
        }

        //[TestMethod]
        //public void TestMethod2()
        //{
        //    var values = new[] {2, 3, 8, 6, 13, 6, 5, 10, 18, 24, 16, 14, 18};

        //    var series = values.Select(x => new Candle() { Close = x });            

        //    var result = series.Ema(4).ToList();
        //    result.ForEach(x=>Debug.WriteLine(x.Price));
           
        //}

        [TestMethod]
        public void TestMethod2()
        {
        //    var a1 = new[] { 2, 3, 4, 5, 6, 6, 6 };
        //    var a2 = new[] { 1, 2, 3, 7, 7, 5 };

        //    var s1 = a1.Select(x => new DatePrice { Price= x });
        //    var s2 = a2.Select(x => new DatePrice { Price = x });

        //    var result = new MovingCrossingFinder().Find(s1, s2).ToList();

        //    Assert.AreEqual(2, result.Count());

        //    Assert.AreEqual(5, result[0].FirstPrice);
        //    Assert.AreEqual(7, result[0].SecondPrice);
        //    Assert.AreEqual(6, result[1].FirstPrice);
        //    Assert.AreEqual(5, result[1].SecondPrice);
        }
    }
}
