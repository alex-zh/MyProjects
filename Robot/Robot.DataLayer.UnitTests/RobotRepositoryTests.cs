using System.Linq;
using Common;
using Common.Classes.Robot;
using Common.Robots.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.DataLayer.Repositories;

namespace Robot.DataLayer.UnitTests
{
    [TestClass]
    public class RobotRepositoryTests
    {
        private readonly RobotRepository _repository = new RobotRepository();

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
            var repository = new RobotRepository();

            var robotState = repository.GetByName(RobotNames.TrendFollowerSi);

            Assert.IsNotNull(robotState);
            Assert.AreEqual(RobotModes.Logging, robotState.Mode);
            Assert.AreEqual(RobotOperationalStatuses.NotStarted, robotState.Status);
        }

        [TestMethod]
        public void UpdateModeAndStatus_Success()
        {
            var repository = new RobotRepository();

            var robot = repository.GetByName(RobotNames.TrendFollowerSi);

            robot.Status = RobotOperationalStatuses.Started;
            robot.Mode = RobotModes.Operative;
            
            repository.Update(robot);

            robot = repository.GetByName(RobotNames.TrendFollowerSi);

            Assert.AreEqual(RobotOperationalStatuses.Started, robot.Status);
            Assert.AreEqual(RobotModes.Operative, robot.Mode);
        }   
    }
}