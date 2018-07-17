using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpinionAnalyzer.DataLoader.Classes;

namespace OpinionAnalyzer.DataLoader.UnitTests
{
    [TestClass]
    public class DateTimeExtensions
    {
        [TestMethod]
        public void Test1()
        {
            var date = new DateTime(2014, 09, 19);
            Assert.AreEqual("13097DE", date.GetHexValue());
        }

        [TestMethod]
        public void Test2()
        {
            var date = new DateTime(2001, 12, 31);
            Assert.AreEqual("1F0C7D1", date.GetHexValue());
        }

        [TestMethod]
        public void Test3()
        {
            var date = new DateTime(2020, 01, 01);
            Assert.AreEqual("01017E4", date.GetHexValue());
        }
    }
}
