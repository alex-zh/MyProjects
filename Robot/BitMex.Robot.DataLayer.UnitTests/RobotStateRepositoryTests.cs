using System;
using System.Linq;
using BitMex.Robot.DataLayer.Repositories;
using Common;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Robot.DataLayer.UnitTests
{
    [TestClass]
    public class RobotStateRepositoryTests
    {
        ILogger logger = new LoggerStub();

        private string robotId = "robot1";
        private readonly RobotRepository _repository = new RobotRepository();


        [TestInitialize]
        public void Init()
        {
            _repository.AddRobot(robotId);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _repository.DeleteRobotBy(robotId);
        }
       
        [TestMethod]
        public void GetByName_Success()
        {
            var robotState = _repository.GetStateByRobotId(robotId);

            Assert.IsNotNull(robotState);
            Assert.AreEqual(0, robotState.Size);
            Assert.AreEqual(0, robotState.GetOrders().Count());
        }

        [TestMethod]
        public void UpdateSize_Success()
        {
            var robotState = _repository.GetStateByRobotId(robotId);

            robotState.AddSize(10);

            _repository.Update(robotId, robotState);

            robotState = _repository.GetStateByRobotId(robotId);

            Assert.AreEqual(10, robotState.Size);
        }

        [TestMethod]
        public void UpdateSizeSeveralTimes_Success()
        {
            var robotState = _repository.GetStateByRobotId(robotId);

            robotState.AddSize(Direction.Buy.ToInt() * 10);
            robotState.AddSize(Direction.Sell.ToInt() * 4);
            robotState.AddSize(Direction.Sell.ToInt() * 2);

            _repository.Update(robotId, robotState);

            robotState = _repository.GetStateByRobotId(robotId);

            Assert.AreEqual(4, robotState.Size);
        }

        [TestMethod]
        public void AddOrders_Success()
        {            
           var robotState = _repository.GetStateByRobotId(robotId);
            var robotStateManager = new RobotStateManager(robotState, logger);

            var firstOrderDateTime = new DateTime(2015, 6, 24, 12, 20, 0);
            var secondOrderDateTime = new DateTime(2015, 6, 24, 12, 40, 0);

            robotStateManager.UpdateOrder(new LimitOrder{ OrderId = "3", DateTime = firstOrderDateTime, OrderSide = OrderSides.Buy, Size = 7, Price = 1200});
            robotStateManager.UpdateOrder(new LimitOrder{ OrderId = "4", DateTime = secondOrderDateTime, OrderSide = OrderSides.Sell, Size = 3, Price = 600});

            _repository.Update(robotId, robotState);

            robotState = _repository.GetStateByRobotId(robotId);

            Assert.AreEqual(2, robotState.GetOrders().Count());

            var firstOrder = robotState.GetOrders().First();

            Assert.AreEqual("3", firstOrder.OrderId);
            Assert.AreEqual(OrderSides.Buy, firstOrder.OrderSide);
            Assert.AreEqual(7, firstOrder.Size);
            Assert.AreEqual(1200, firstOrder.Price);
            Assert.AreEqual(firstOrderDateTime, firstOrder.DateTime);

            var secondOrder = robotState.GetOrders().Skip(1).First();

            Assert.AreEqual("4", secondOrder.OrderId);
            Assert.AreEqual(OrderSides.Sell, secondOrder.OrderSide);
            Assert.AreEqual(3, secondOrder.Size);
            Assert.AreEqual(600, secondOrder.Price);
            Assert.AreEqual(secondOrderDateTime, secondOrder.DateTime);
        }

        [TestMethod]
        public void AddRemoveOrders_Success()
        {
            var robotState = _repository.GetStateByRobotId(robotId);
            var robotStateManager = new RobotStateManager(robotState, logger);

            robotStateManager.UpdateOrder(new LimitOrder { OrderId = "3", DateTime = DateTime.Now, OrderSide = OrderSides.Buy, Size =  5, Price = 100});

            _repository.Update(robotId, robotState);

            robotStateManager.UpdateOrder(new LimitOrder { OrderId = "3", DateTime = DateTime.Now, OrderSide = OrderSides.Buy, Size = 3, Price = 900});

            _repository.Update(robotId, robotState);

            robotStateManager.UpdateOrder(new LimitOrder { OrderId = "3", DateTime = DateTime.Now, OrderSide = OrderSides.Sell, Size = 1, Price = 500 });

            _repository.Update(robotId, robotState);

            robotState = _repository.GetStateByRobotId(robotId);

            var firstOrder = robotState.GetOrders().First();

            Assert.AreEqual(1, robotState.GetOrders().Count());
            Assert.AreEqual("3", firstOrder.OrderId);
            Assert.AreEqual(OrderSides.Sell, firstOrder.OrderSide);
            Assert.AreEqual(1, firstOrder.Size);
            Assert.AreEqual(500, firstOrder.Price);
        }     
    }
}