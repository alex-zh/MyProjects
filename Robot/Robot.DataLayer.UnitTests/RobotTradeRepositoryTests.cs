using System;
using System.Linq;
using Common.Classes.Robot;
using Common.Robots.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.DataLayer.Repositories;

namespace Robot.DataLayer.UnitTests
{
    [TestClass]
    public class RobotTradeRepositoryTests
    {
        private readonly RobotRepository _repository = new RobotRepository();
        private readonly RobotTradeRepository _tradeRepository = new RobotTradeRepository();

        [TestInitialize]
        public void Init()
        {
            _repository.AddRobot(RobotNames.TrendFollowerSi);
            _repository.AddRobot(RobotNames.TrendFollowerXbtUsd);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _repository.DeleteRobotBy(RobotNames.TrendFollowerSi);
            _repository.DeleteRobotBy(RobotNames.TrendFollowerXbtUsd);
        }
       
        [TestMethod]
        public void GetByName_Success()
        {
            var trades = _tradeRepository.GetAllFor(RobotNames.TrendFollowerSi);

            Assert.IsNotNull(trades);
            Assert.AreEqual(0, trades.Count());
        }

        [TestMethod]
        public void AddTrade_Success()
        {
            var tradeTime = DateTime.Now.AddMinutes(-10);

             _tradeRepository.Add(RobotNames.TrendFollowerSi, new RobotTrade(){ Date = tradeTime, Price = 100.0, Size = 10, TradeNumber = 777, OrderNumber = 101010});

             var trades = _tradeRepository.GetAllFor(RobotNames.TrendFollowerSi).ToList();

             Assert.AreEqual(1, trades.Count);
             Assert.AreEqual((int)tradeTime.TimeOfDay.TotalSeconds, (int)trades[0].Date.TimeOfDay.TotalSeconds);
             Assert.AreEqual(100, trades[0].Price);
             Assert.AreEqual(10, trades[0].Size);
             Assert.AreEqual(101010, trades[0].OrderNumber);
             Assert.AreEqual(777, trades[0].TradeNumber);
        }

        [TestMethod]
        public void AddThereTrades_RemoveOne_Success()
        {
            var tradeTime = DateTime.Now.AddMinutes(-10);

            _tradeRepository.Add(RobotNames.TrendFollowerSi, new RobotTrade() { Date = tradeTime, Price = 100.0, Size = 10 });
            _tradeRepository.Add(RobotNames.TrendFollowerXbtUsd, new RobotTrade() { Date = tradeTime, Price = 10.0, Size = -10 });
            _tradeRepository.Add(RobotNames.TrendFollowerSi, new RobotTrade() { Date = tradeTime, Price = 50.0, Size = -5 });

            var trades = _tradeRepository.GetAllFor(RobotNames.TrendFollowerSi).ToList();

            Assert.AreEqual(2, trades.Count);
            Assert.AreEqual(100, trades[0].Price); Assert.AreEqual(10, trades[0].Size);
            Assert.AreEqual(50, trades[1].Price); Assert.AreEqual(-5, trades[1].Size);

            trades = _tradeRepository.GetAllFor(RobotNames.TrendFollowerXbtUsd).ToList();
            Assert.AreEqual(1, trades.Count);
            Assert.AreEqual(10, trades[0].Price); Assert.AreEqual(-10, trades[0].Size);

            _tradeRepository.DeleteAllFor(RobotNames.TrendFollowerXbtUsd);

            trades = _tradeRepository.GetAllFor(RobotNames.TrendFollowerXbtUsd).ToList();
            Assert.AreEqual(0, trades.Count);

            trades = _tradeRepository.GetAllFor(RobotNames.TrendFollowerSi).ToList();
            Assert.AreEqual(2, trades.Count);

            _tradeRepository.DeleteAllFor(RobotNames.TrendFollowerSi);

            trades = _tradeRepository.GetAllFor(RobotNames.TrendFollowerSi).ToList();
            Assert.AreEqual(0, trades.Count);
        }  
    }
}