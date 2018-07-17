using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Trans2Quik.Core;

namespace Trans2Quik.Tests
{
    [TestClass]
    public class ExpiryDateTests
    {
        [TestMethod]
        public void CanCreateEmptyExpDate()
        {
            var ex = new ExpiryDate();
            Assert.IsTrue(!ex.Date.HasValue);
            Assert.IsFalse(ex.GoodTillCanceled);
            Assert.IsTrue(ex.ToString() == "TODAY");
        }

        [TestMethod]
        public void CanCreateGtcExpDate()
        {
            var ex = new ExpiryDate(true);
            Assert.IsTrue(!ex.Date.HasValue);
            Assert.IsTrue(ex.GoodTillCanceled);
            Assert.IsTrue(ex.ToString() == "GTC");
        }

        [TestMethod]
        public void CanCreateSpecificExpDate()
        {
            var ex = new ExpiryDate(new DateTime(2014, 9, 15));
            Assert.IsTrue(ex.Date.HasValue);
            Assert.IsFalse(ex.GoodTillCanceled);
            Assert.IsTrue(ex.ToString() == "20140915");
        }

        [TestMethod]
        public void CanUseStaticExpDate()
        {
            Assert.IsTrue(ExpiryDate.GTC.ToString() == "GTC");
            Assert.IsTrue(ExpiryDate.TODAY.ToString() == "TODAY");
        }
    }
}