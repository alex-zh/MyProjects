using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Trans2Quik.Core;
using Trans2Quik.Core.Entities.Transaction.Order;
using Trans2Quik.Core.Internals;

namespace Trans2Quik.Tests
{    
    [TestClass]
    public class TransactionBuilderTests
    {
        private TransactionBuilder tb;

        [TestInitialize]
        public void Setup()
        {
            this.tb = new TransactionBuilder("ABC001");
        }

        [TestMethod]
        public void CanCreateNewOrder()
        {
            var o = this.tb.NewOrder(new OrderTradeParams(Mother.SBRF, Direction.Buy, 2, 73.45M));

            Assert.IsFalse(String.IsNullOrEmpty(o.ToString()));
            Assert.IsTrue(o.IsLimitOrder.GetValueOrDefault());
            Assert.IsTrue(this.tb.NextTxnId == 2);
        }

        [TestMethod]
        public void CanCreateNewMarketOrder()
        {
            var o = this.tb.NewOrder(new OrderTradeParams(Mother.SBRF, Direction.Sell, 2));
            Assert.IsFalse(String.IsNullOrEmpty(o.ToString()));
            Assert.IsFalse(o.IsLimitOrder.GetValueOrDefault());
            Assert.IsTrue(this.tb.NextTxnId == 2);
        }

        [TestMethod]
        public void CanCreateStopOrder()
        {
            var q = new OrderTradeParams(Mother.SBRF, Direction.Sell, 1);
            var o = this.tb.NewStopLimitOrder(new StopOrderTradeParams(q, 73.48m));
            Assert.IsFalse(String.IsNullOrEmpty(o.ToString()));
        }

        [TestMethod]
        public void CanCreateTakeProfitOrder()
        {
            var q = new OrderTradeParams(Mother.SBRF, Direction.Sell, 1);
            var sq = new StopOrderTradeParams(q, 76.48m, Mother.STD_ProfitCondition);
            var o = this.tb.NewTakeProfitOrder(sq);
            Assert.IsFalse(String.IsNullOrEmpty(o.ToString()));
        }

        [TestMethod]
        public void CanCreateTakeProfitAndStopLimitOrder()
        {
            var q = new OrderTradeParams(Mother.SBRF, Direction.Sell, 1, 76.48m);
            var sq = new StopOrderTradeParams(q, 76.48m, Mother.STD_ProfitCondition, ExpiryDate.TODAY, 73.12m);
            var o = this.tb.NewTakeProfitAndStopLimitOrder(sq);
            Assert.IsFalse(String.IsNullOrEmpty(o.ToString()));
        }

        [TestMethod]
        public void CanCreateKillOrder()
        {
            var o = this.tb.KillOrder(Mother.SBRF, "2");
            Assert.IsFalse(String.IsNullOrEmpty(o.ToString()));
            Assert.IsTrue(o.OrderKey == "2");
        }

        [TestMethod]
        public void CanCreateKillStopOrder()
        {
            var o = this.tb.KillStopOrder(Mother.SBRF, "22");
            Assert.IsFalse(String.IsNullOrEmpty(o.ToString()));
            Assert.IsTrue(o.StopOrderKey == "22");
        }
    }
}
