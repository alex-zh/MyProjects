using System;
using System.Linq;
using BitMex.Robot.DataLayer.Repositories;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Robot.DataLayer.UnitTests
{
    [TestClass]
    public class LogsRepositoryTests
    {
        private readonly string firstRobotId = "robot1";
        private readonly string secondRobotId = "robot2";

        private readonly ILogsRepository _logsRepository = new LogsRepository();
        private readonly RobotRepository _robotsRepository = new RobotRepository();

        [TestInitialize]
        public void Init()
        {
            _robotsRepository.AddRobot(firstRobotId);
            _robotsRepository.AddRobot(secondRobotId);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _logsRepository.ClearAll();
            _robotsRepository.DeleteRobotBy(firstRobotId);
            _robotsRepository.DeleteRobotBy(secondRobotId);            
        }
       
        [TestMethod]
        public void LogsRepository_GetAll_Success()
        {                        
            Assert.AreEqual(0, _logsRepository.GetAll().Count());
        }

        [TestMethod]
        public void LogsRepository_Add_Success()
        {
            _logsRepository.Add(new LogItem { DateTime = DateTime.Now, MessageType = LogMessageTypes.Error, Text = "first error" });
            _logsRepository.Add(new LogItem { DateTime = DateTime.Now, MessageType = LogMessageTypes.Error, Text = "second error" });
            _logsRepository.Add(new LogItem() { DateTime = DateTime.Now, MessageType = LogMessageTypes.Error, Text = "first error for robot 1" }, firstRobotId);
            _logsRepository.Add(new LogItem() { DateTime = DateTime.Now, MessageType = LogMessageTypes.Warning, Text = "second error for robot 1" }, firstRobotId);
            _logsRepository.Add(new LogItem() { DateTime = DateTime.Now, MessageType = LogMessageTypes.Info, Text = "first error for robot 2" }, secondRobotId);

            Assert.AreEqual(5, _logsRepository.GetAll().Count());
            Assert.AreEqual(2, _logsRepository.GetAll(firstRobotId).Count());
            Assert.AreEqual(1, _logsRepository.GetAll(secondRobotId).Count());

            Assert.AreEqual("second error for robot 1", _logsRepository.GetAll(firstRobotId).Skip(1).First().Text);
            Assert.AreEqual(LogMessageTypes.Warning, _logsRepository.GetAll(firstRobotId).Skip(1).First().MessageType);

            Assert.AreEqual("first error for robot 2", _logsRepository.GetAll(secondRobotId).First().Text);
            Assert.AreEqual(LogMessageTypes.Info, _logsRepository.GetAll(secondRobotId).First().MessageType);
        }
    }
}