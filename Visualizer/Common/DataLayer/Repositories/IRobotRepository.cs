using Common.Robots.Common;

namespace Common.DataLayer.Repositories
{
    public interface IRobotRepository
    {
        RobotInfo GetById(string robotId);

        RobotState GetStateByRobotId(string robotId);

        void Update(RobotInfo robot);
    }
}