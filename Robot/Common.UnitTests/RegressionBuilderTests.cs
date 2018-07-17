using System;
using Common.Regression;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests
{
    [TestClass]
    public class RegressionBuilderTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var x = new double[] {1, 2, 3, 4, 2, 4, 12, 6};
            var y = new double[] {1, 3, 3, 5, 10, 8, 24, -2};
            var regressionBuilder = new LinearRegressionBuilder(x,y);

            var regression = regressionBuilder.Build();

           Assert.AreEqual(0.729, Math.Round( regression.RegressionCoefficient,3));
        }
    }
}
