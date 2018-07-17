using System;
using Common.Classes.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests
{
    [TestClass]
    public class PearsonCorrelationTests
    {
        [TestMethod]
        public void PositiveTest1()
        {
            var first = new[] {1.0, 2, 3};
            var second = new[] {1.0, 2, 3};

            var calculator = new CorrelationCalculator(first, second);
            Assert.AreEqual(1, calculator.Calculate(CorrelationTypes.Pearson));
        }

        [TestMethod]
        public void PositiveTest2()
        {
            var first = new[] { 1.0, 2, 3, 1 };
            var second = new[] { 2.0, 3, 4, 3 };

            var calculator = new CorrelationCalculator(first, second);
            Assert.AreEqual(0.85, Math.Round(calculator.Calculate(CorrelationTypes.Pearson),2));
        }

        [TestMethod]
        public void NegativeTest1()
        {
            var first = new[] { 1.0, 2, 3 };
            var second = new[] { 3.0, 2, 1 };

            var calculator = new CorrelationCalculator(first, second);
            Assert.AreEqual(-1, calculator.Calculate(CorrelationTypes.Pearson));
        }

        [TestMethod]
        public void NegativeTest2()
        {
            var first = new[] { 1.0, 2, 3, 8 };
            var second = new[] { 8.0, 2, 1, 2 };

            var calculator = new CorrelationCalculator(first, second);
            Assert.AreEqual(-0.486, Math.Round(calculator.Calculate(CorrelationTypes.Pearson), 3));
        }


    }
}
