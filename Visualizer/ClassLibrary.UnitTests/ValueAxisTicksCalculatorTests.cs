using Microsoft.VisualStudio.TestTools.UnitTesting;
using Visualizer.Core.Classes;


namespace ClassLibrary.UnitTests
{
    [TestClass]
    public class ValueAxisTicksCalculatorTests
    {
        [TestMethod]
        public void Test1()
        {
            double max = 0.1335;
            double min = -0.071;
            var calc = new ValueAxisTicksCalculator(min, max);

            Assert.AreEqual(0.14, calc.GetMaxValue());
            Assert.AreEqual(-0.08, calc.GetMinValue());
        }

        [TestMethod]
        public void Test2()
        {
            double max = 133.5;
            double min = -71;
            var calc = new ValueAxisTicksCalculator(min, max);

            Assert.AreEqual(140, calc.GetMaxValue());
            Assert.AreEqual(-80, calc.GetMinValue());
        }

        [TestMethod]
        public void Test3()
        {
            double max = 1334.5;
            double min = -71;
            var calc = new ValueAxisTicksCalculator(min, max);

            Assert.AreEqual(1400, calc.GetMaxValue());
            Assert.AreEqual(-100, calc.GetMinValue());
        }

        [TestMethod]
        public void Test4()
        {
            double max = 32.17;
            double min = 31.67;
            var calc = new ValueAxisTicksCalculator(min, max);

            Assert.AreEqual(32.2, calc.GetMaxValue());
            Assert.AreEqual(31.65, calc.GetMinValue());
        }

        [TestMethod]
        public void Test5()
        {
            double min = 29.23;
            double max = 29.54;
            var calc = new ValueAxisTicksCalculator(min, max);

            //Assert.AreEqual(29.2, calc.GetMinValue());
            Assert.AreEqual(29.55, calc.GetMaxValue());            
        }
    }
}
