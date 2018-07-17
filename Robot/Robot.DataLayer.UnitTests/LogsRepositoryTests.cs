using System;
using System.Linq;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.DataLayer.Repositories;

namespace Robot.DataLayer.UnitTests
{
    [TestClass]
    public class LogsRepositoryTests
    {
        private readonly RobotNames firstRobotName = RobotNames.TrendFollowerSi;
        private readonly RobotNames secondRobotName = RobotNames.TrendFollowerXbtUsd;
        private readonly ILogsRepository _logsRepository = new LogsRepository();
        private readonly RobotRepository _robotsRepository = new RobotRepository();

        [TestInitialize]
        public void Init()
        {
            _robotsRepository.AddRobot(firstRobotName);
            _robotsRepository.AddRobot(secondRobotName);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _logsRepository.ClearAll();
            _robotsRepository.DeleteRobotBy(firstRobotName);
            _robotsRepository.DeleteRobotBy(secondRobotName);            
        }
       
        [TestMethod]
        public void LogsRepository_GetAll_Success()
        {                        
            Assert.AreEqual(0, _logsRepository.GetAll().Count());
        }

        [TestMethod]
        public void LogsRepository_Add_Success()
        {
            _logsRepository.Add(new LogItem() { DateTime = DateTime.Now, MessageType = LogMessageTypes.Error, Text = "first error" });
            _logsRepository.Add(new LogItem() { DateTime = DateTime.Now, MessageType = LogMessageTypes.Error, Text = "second error" });
            _logsRepository.Add(new LogItem() { DateTime = DateTime.Now, MessageType = LogMessageTypes.Error, Text = "first error for robot 1" }, firstRobotName);
            _logsRepository.Add(new LogItem() { DateTime = DateTime.Now, MessageType = LogMessageTypes.Warning, Text = "second error for robot 1" }, firstRobotName);
            _logsRepository.Add(new LogItem() { DateTime = DateTime.Now, MessageType = LogMessageTypes.Info, Text = "first error for robot 2" }, secondRobotName);

            Assert.AreEqual(5, _logsRepository.GetAll().Count());
            Assert.AreEqual(2, _logsRepository.GetAll(firstRobotName).Count());
            Assert.AreEqual(1, _logsRepository.GetAll(secondRobotName).Count());

            Assert.AreEqual("second error for robot 1", _logsRepository.GetAll(firstRobotName).Skip(1).First().Text);
            Assert.AreEqual(LogMessageTypes.Warning, _logsRepository.GetAll(firstRobotName).Skip(1).First().MessageType);

            Assert.AreEqual("first error for robot 2", _logsRepository.GetAll(secondRobotName).First().Text);
            Assert.AreEqual(LogMessageTypes.Info, _logsRepository.GetAll(secondRobotName).First().MessageType);
        }
    }
}