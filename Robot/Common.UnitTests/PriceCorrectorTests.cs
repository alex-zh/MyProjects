using System;
using Common.Caclulators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests
{
    [TestClass]
    public class PriceCorrectorTests
    {
        [TestMethod]
        public void Test1()
        {
            Assert.AreEqual(63216, PriceCorrector.GetPrice(63214, Direction.Buy));
            Assert.AreEqual(63226, PriceCorrector.GetPrice(63218, Direction.Buy));

            Assert.AreEqual(63236, PriceCorrector.GetPrice(63233, Direction.Buy));
            Assert.AreEqual(63206, PriceCorrector.GetPrice(63203, Direction.Buy));


            Assert.AreEqual(63214, PriceCorrector.GetPrice(63217, Direction.Sell));
            Assert.AreEqual(63204, PriceCorrector.GetPrice(63207, Direction.Sell));

            Assert.AreEqual(63234, PriceCorrector.GetPrice(63236, Direction.Sell));
            Assert.AreEqual(63594, PriceCorrector.GetPrice(63597.5, Direction.Sell));
        }
    }
}
