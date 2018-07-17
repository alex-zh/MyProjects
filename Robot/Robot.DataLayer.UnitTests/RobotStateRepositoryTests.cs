using System;
using System.Linq;
using Common;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.DataLayer.Repositories;

namespace Robot.DataLayer.UnitTests
{
    [TestClass]
    public class RobotStateRepositoryTests
    {
        private readonly RobotRepository _repository = new RobotRepository();
        private readonly RobotStateRepository _stateRepository = new RobotStateRepository();

        [TestInitialize]
        public void Init()
        {
            _repository.AddRobot(RobotNames.TrendFollowerSi);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _repository.DeleteRobotBy(RobotNames.TrendFollowerSi);
        }
       
        [TestMethod]
        public void GetByName_Success()
        {
            var robotState = _stateRepository.GetByName(RobotNames.TrendFollowerSi);

            Assert.IsNotNull(robotState);
            Assert.AreEqual(0, robotState.Size);
            Assert.AreEqual(0, robotState.Orders.Count());
        }

        [TestMethod]
        public void UpdateSize_Success()
        {
            var robotState = _stateRepository.GetByName(RobotNames.TrendFollowerSi);

            robotState.AddSize(10);

            _stateRepository.Update(RobotNames.TrendFollowerSi, robotState);

            robotState = _stateRepository.GetByName(RobotNames.TrendFollowerSi);

            Assert.AreEqual(10, robotState.Size);
        }

        [TestMethod]
        public void UpdateSizeSeveralTimes_Success()
        {
            var robotState = _stateRepository.GetByName(RobotNames.TrendFollowerSi);

            robotState.AddSize(Direction.Buy.ToInt() * 10);
            robotState.AddSize(Direction.Sell.ToInt() * 4);
            robotState.AddSize(Direction.Sell.ToInt() * 2);

            _stateRepository.Update(RobotNames.TrendFollowerSi, robotState);

            robotState = _stateRepository.GetByName(RobotNames.TrendFollowerSi);

            Assert.AreEqual(4, robotState.Size);
        }

        [TestMethod]
        public void AddOrders_Success()
        {            
           var robotState = _stateRepository.GetByName(RobotNames.TrendFollowerSi);
            var robotStateManager = new RobotStateManager(robotState);

            var firstOrderDateTime = new DateTime(2015, 6, 24, 12, 20, 0);
            var secondOrderDateTime = new DateTime(2015, 6, 24, 12, 40, 0);

            robotStateManager.UpdateOrder(new LimitOrder { OrderNumber = 3, RobotOrderType = RobotOrderTypes.OpenPosition, DateTime = firstOrderDateTime, Params = new OrderParams(Direction.Buy, 7, 1200)});
            robotStateManager.UpdateOrder(new LimitOrder { OrderNumber = 4, RobotOrderType = RobotOrderTypes.TakeProfit, DateTime = secondOrderDateTime, Params = new OrderParams(Direction.Sell, 3, 600) });

            _stateRepository.Update(RobotNames.TrendFollowerSi, robotState);

            robotState = _stateRepository.GetByName(RobotNames.TrendFollowerSi);

            Assert.AreEqual(2, robotState.Orders.Count());

            var firstOrder = robotState.Orders.First();

            Assert.AreEqual(3, firstOrder.OrderNumber);
            Assert.AreEqual(RobotOrderTypes.OpenPosition, firstOrder.RobotOrderType);
            Assert.AreEqual(Direction.Buy, firstOrder.Params.Direction);
            Assert.AreEqual(7, firstOrder.Params.Size);
            Assert.AreEqual(1200, firstOrder.Params.Price);
            Assert.AreEqual(firstOrderDateTime, firstOrder.DateTime);

            var secondOrder = robotState.Orders.Skip(1).First();

            Assert.AreEqual(4, secondOrder.OrderNumber);
            Assert.AreEqual(RobotOrderTypes.TakeProfit, secondOrder.RobotOrderType);
            Assert.AreEqual(Direction.Sell, secondOrder.Params.Direction);
            Assert.AreEqual(3, secondOrder.Params.Size);
            Assert.AreEqual(600, secondOrder.Params.Price);
            Assert.AreEqual(secondOrderDateTime, secondOrder.DateTime);
        }

        [TestMethod]
        public void AddRemoveOrders_Success()
        {
            var robotState = _stateRepository.GetByName(RobotNames.TrendFollowerSi);
            var robotStateManager = new RobotStateManager(robotState);

            robotStateManager.UpdateOrder(new LimitOrder { OrderNumber = 3, RobotOrderType = RobotOrderTypes.OpenPosition, DateTime = DateTime.Now, Params = new OrderParams(Direction.Buy, 5, 1000) });

            _stateRepository.Update(RobotNames.TrendFollowerSi, robotState);

            robotStateManager.UpdateOrder(new LimitOrder { OrderNumber = 3, RobotOrderType = RobotOrderTypes.OpenPosition, DateTime = DateTime.Now, Params = new OrderParams(Direction.Buy, 3, 900) });

            _stateRepository.Update(RobotNames.TrendFollowerSi, robotState);

            robotStateManager.UpdateOrder(new LimitOrder { OrderNumber = 3, RobotOrderType = RobotOrderTypes.OpenPosition, DateTime = DateTime.Now, Params = new OrderParams(Direction.Sell, 1, 500) });

            _stateRepository.Update(RobotNames.TrendFollowerSi, robotState);

            robotState = _stateRepository.GetByName(RobotNames.TrendFollowerSi);

            var firstOrder = robotState.Orders.First();

            Assert.AreEqual(1, robotState.Orders.Count());
            Assert.AreEqual(3, firstOrder.OrderNumber);
            Assert.AreEqual(RobotOrderTypes.OpenPosition, firstOrder.RobotOrderType);
            Assert.AreEqual(Direction.Sell, firstOrder.Params.Direction);
            Assert.AreEqual(1, firstOrder.Params.Size);
            Assert.AreEqual(500, firstOrder.Params.Price);
        }     
    }
}