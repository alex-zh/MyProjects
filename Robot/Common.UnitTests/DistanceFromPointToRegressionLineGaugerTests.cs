using System;
using Common.Regression;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests
{
    [TestClass]
    public class DistanceFromPointToRegressionLineGaugerTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var regression = new LinearRegression() {a0 = 0, a1 = 1};

            var gauger = new DistanceFromPointToRegressionLineGauger(regression);

            Assert.AreEqual(0, gauger.GetDistance(12, 12));
            Assert.AreEqual(Math.Round(3 * Math.Sqrt(2) / 2, 5), Math.Round(gauger.GetDistance(12, 15), 5));
        }
    }
}