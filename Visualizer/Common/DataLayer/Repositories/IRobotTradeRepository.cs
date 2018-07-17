using System;
using System.Collections.Generic;
using Common.Robots.Common;

namespace Common.DataLayer.Repositories
{
    public interface IRobotTradeRepository
    {
        IEnumerable<RobotTrade> GetAll(string robotId);

        IEnumerable<RobotTrade> GetAll(string robotId, string orderId);

        IEnumerable<RobotTrade> GetAll(string robotId, DateTime startDate);

        void Add(string robotId, RobotTrade robotTrade);

        void Add(string robotId, IEnumerable<RobotTrade> trades);

        void Update(RobotTrade robotTrade);

        void RemoveAll(string robotId);

        void RemoveAll(string robotId, string orderId);
    }
}