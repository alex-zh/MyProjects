using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.UnitTests.Stubs;

namespace Robot.UnitTests
{
    [TestClass]
    public class RobotStateManagerTests
    {
        ILogger logger = new LoggerStub();

        [TestMethod]
        public void Test1()
        {
            var state = new RobotState(0, new List<LimitOrder>());

            var synchronizer = new RobotStateManager(state, logger);

            synchronizer.Synchronize(new List<LimitOrder>());

            Assert.AreEqual(0, state.Size);
            Assert.AreEqual(0, state.GetOrders().Count());
        }

        [TestMethod]
        public void FullyCompletedOrder()
        {
            var orders = new List<LimitOrder>
            {
                    new LimitOrder{ OrderId= "1", Balance = 5, Status = OrderStatuses.Active,OrderSide = OrderSides.Buy, Size = 5, Price = 100},
                    new LimitOrder{ OrderId= "2", Balance = 5, Status = OrderStatuses.Active,OrderSide = OrderSides.Buy, Size = 5, Price = 100},
                    new LimitOrder{ OrderId= "3", Balance = 5, Status = OrderStatuses.Active,OrderSide = OrderSides.Buy, Size = 5, Price = 100},
            };

            var state = new RobotState(0, orders);

            var synchronizer = new RobotStateManager(state, logger);

            Assert.AreEqual(0, state.Size);
            Assert.AreEqual(3, state.GetOrders().Count());

            var marketOrders = new List<LimitOrder>
            {
                new LimitOrder {OrderId = "1", Status = OrderStatuses.Completed, Balance = 0, OrderSide = OrderSides.Buy, Size = 5, Price = 100},
                new LimitOrder {OrderId = "2", Status = OrderStatuses.Withdrawn, Balance = 0},
                new LimitOrder {OrderId = "3", Status = OrderStatuses.Withdrawn, Balance = 0},                
            };

            synchronizer.Synchronize(marketOrders);
            synchronizer.CleanOrders();

            Assert.AreEqual(5, state.Size);
            Assert.AreEqual(0, state.GetOrders().Count());
        }

        [TestMethod]
        public void PartiallyCompletedOrder()
        {

            var orders = new List<LimitOrder>
            {
                    new LimitOrder{ OrderId= "1", Balance = 5, Status = OrderStatuses.Active, OrderSide = OrderSides.Buy, Size = 5, Price = 100},
            };
         

            var state = new RobotState(0, orders);

            var stateManager = new RobotStateManager(state, logger);

            Assert.AreEqual(0, state.Size);
            Assert.AreEqual(1, state.GetOrders().Count());

            var marketOrders = new List<LimitOrder>
            {
                new LimitOrder{ OrderId= "1", Status = OrderStatuses.Active, Balance = 2, OrderSide = OrderSides.Buy, Size = 5, Price = 100},
            };

            stateManager.Synchronize(marketOrders);

            Assert.AreEqual(3, state.Size);
            Assert.AreEqual(1, state.GetOrders().Count());

             marketOrders = new List<LimitOrder>
            {
                new LimitOrder{ OrderId= "1", Status = OrderStatuses.Active, Balance = 1, OrderSide = OrderSides.Buy, Size = 5, Price = 100},
            };
            
            stateManager.Synchronize(marketOrders);

            Assert.AreEqual(4, state.Size);
            Assert.AreEqual(1, state.GetOrders());

            marketOrders = new List<LimitOrder>
            {
                new LimitOrder{ OrderId= "1", Status = OrderStatuses.Active, Balance = 0, OrderSide = OrderSides.Buy, Size = 5, Price = 100},
            };
            

            stateManager.Synchronize(marketOrders);
            stateManager.CleanOrders();

            Assert.AreEqual(5, state.Size);
            Assert.AreEqual(0, state.GetOrders().Count());
        }
    }
}
