using System;
using Common;
using Common.Classes.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Robot.UnitTests
{
    [TestClass]
    public class AtrTests
    {
        [TestMethod]
        public void CalculateExponentialAverageTest1()
        {
            var result = Atr.CalculateExponentionalAverage(new[] {3.0, 2.0, 1.0}, 2);
            Assert.AreEqual(1.75, result);
        }

        [TestMethod]
        public void CalculateExponentialAverageTest2()
        {
            var result = Atr.CalculateExponentionalAverage(new[] { 1.0, 2.0, 3.0 }, 2);
            Assert.AreEqual(2.25, result);
        }
    }
}
