using Common.Classes.Robot;
using Common.Robots.Common;

namespace Robot.DataLayer.Repositories
{
    public interface IRobotRepository
    {
        RobotInfo GetByName(RobotNames name);

        void Update(RobotInfo robot);
    }
}