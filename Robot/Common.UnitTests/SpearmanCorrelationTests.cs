using System;
using Common.Classes.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests
{
    [TestClass]
    public class SpearmanCorrelationTests
    {
        [TestMethod]
        public void PositiveTest1()
        {
            var first = new[] {1.0, 2, 3};
            var second = new[] {1.0, 2, 3};

            var calculator = new CorrelationCalculator(first, second);
            Assert.AreEqual(1, calculator.Calculate(CorrelationTypes.Spearman));
        }

        [TestMethod]
        public void PositiveTest2()
        {
            var first = new[] { 1.0, 2, 3 };
            var second = new[] { 2.0, 3, 4 };

            var calculator = new CorrelationCalculator(first, second);
            Assert.AreEqual(1, calculator.Calculate(CorrelationTypes.Spearman));
        }

        [TestMethod]
        public void PositiveTest3()
        {
            var first = new[] { 1.0, 3,  5, 8, 10 };
            var second = new[] { 10.0, 20, 30, 40, 41  };

            var calculator = new CorrelationCalculator(first, second);
            Assert.AreEqual(1, calculator.Calculate(CorrelationTypes.Spearman));
        }

        [TestMethod]
        public void PositiveTest4()
        {
            var first = new[] { 6.8, 9.1, 9.6, 10.1, 10.5, 13, 17.1, 19.1, 22.7, 27.4, 29.6, 32.9 };
            var second = new[] { 0.52, 1.09, 0.67, 2.83, 1.37, 1.95, 4.1, 3.82, 1.59, 1.64, 2.09, 2.96 };

            var calculator = new CorrelationCalculator(first, second);
            Assert.AreEqual(0.62, Math.Round(calculator.Calculate(CorrelationTypes.Spearman), 2));
        }


        [TestMethod]
        public void NegativeTest1()
        {
            var first = new[] { 1.0, 2, 3 };
            var second = new[] { 3.0, 2, 1 };

            var calculator = new CorrelationCalculator(first, second);
            Assert.AreEqual(-1, calculator.Calculate(CorrelationTypes.Spearman));
        }

        [TestMethod]
        public void NegativeTest2()
        {
            var first = new[] { 1.0, 2, 3,5 };
            var second = new[] { 30.0, 20, 10, -2 };

            var calculator = new CorrelationCalculator(first, second);
            Assert.AreEqual(-1, calculator.Calculate(CorrelationTypes.Spearman));
        }
    }
}
