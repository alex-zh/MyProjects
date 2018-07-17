using BitMex.Robot.DataLayer.Repositories;
using Common.Classes.Robot;
using Common.Robots.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Robot.DataLayer.UnitTests
{
    [TestClass]
    public class RobotRepositoryTests
    {
        private readonly RobotRepository _repository = new RobotRepository();

        [TestInitialize]
        public void Init()
        {
            _repository.AddRobot("robot1");
        }

        [TestCleanup]
        public void Cleanup()
        {
            _repository.DeleteRobotBy("robot1");
        }
       
        [TestMethod]
        public void GetByName_Success()
        {
            var repository = new RobotRepository();

            var robotState = repository.GetById("robot1");

            Assert.IsNotNull(robotState);
            Assert.AreEqual(RobotModes.Logging, robotState.Mode);
            Assert.AreEqual(0, robotState.Size);
            Assert.AreEqual(null, robotState.Symbol);
            Assert.AreEqual(RobotOperationalStatuses.NotStarted, robotState.Status);
        }

        [TestMethod]
        public void UpdateModeAndStatus_Success()
        {
            var repository = new RobotRepository();

            var robot = repository.GetById("robot1");

            robot.Symbol = "XBTUSD";
            robot.Status = RobotOperationalStatuses.Started;
            robot.Mode = RobotModes.Operative;
            robot.Size = 10;

            repository.Update(robot);

            robot = repository.GetById("robot1");

            Assert.AreEqual(RobotOperationalStatuses.Started, robot.Status);
            Assert.AreEqual(RobotModes.Operative, robot.Mode);
            Assert.AreEqual("XBTUSD", robot.Symbol);
            Assert.AreEqual(10, robot.Size);
        }   
    }
}