using System;
using System.Collections.Generic;
using FinancialIdeasTester;
using FinancialIdeasTester.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class LogScaleDetrendedChangesValuatorTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var valuator = new LogScaleDetrendedChangesValuator();

            var data = new List<Candle>
            {
                new Candle() {Date = DateTime.Now, Open = 1, Close = 2},
                new Candle() {Date = DateTime.Now, Open = 2, Close = 4},
                new Candle() {Date = DateTime.Now, Open = 4, Close = 8}
            }
            ;
            var result = valuator.Valuate(data);

            Assert.AreEqual(0, Math.Round(result[0].Change, 6));//almost zero
            Assert.AreEqual(0, Math.Round(result[1].Change, 6));
            Assert.AreEqual(0, Math.Round(result[2].Change, 6));
        }
    }
}
