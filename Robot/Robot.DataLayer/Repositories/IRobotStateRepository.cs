using Common.Classes.Robot;
using Common.Robots.TrendFollower;

namespace Robot.DataLayer.Repositories
{
    public interface IRobotStateRepository
    {
        RobotState GetById(int robotId);

        void Update(int robotId, RobotState robot);
    }
}