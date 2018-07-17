using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.Robot;
using Common.Robots.Common;
using Common.Robots.TrendFollower;
using Robot.DataLayer.Classes;

namespace Robot.DataLayer.Repositories
{
    public class RobotStateRepository : IRobotStateRepository
    {
        private readonly RobotRepository _repository = new RobotRepository();

        public RobotState GetByName(RobotNames name)
        {
            var robotId = _repository.GetByName(name).RobotId;

            return GetById(robotId);
        }

        public RobotState GetById(int robotId)
        {
            using (var db = new RobotContext())
            {
                var dbRobotState = db.RobotStates.FirstOrDefault(x => x.RobotId == robotId) ?? new DbRobotState();

                var dbOrders = db.Orders.Where(x => x.RobotId == robotId);

                var orders = dbOrders.Any()? dbOrders.ToRobotLimitedOrders(): new Dictionary<RobotOrderTypes, LimitOrder>();
                
                return new RobotState(dbRobotState.PositionSize, orders);
            }
        }

        public void Update(RobotNames name, RobotState robotState)
        {
            var robotId = _repository.GetByName(name).RobotId;

            Update(robotId, robotState);
        }

        public void Update(int robotId, RobotState robotState)
        {
            using (var db = new RobotContext())
            {                
                var dbRobotState = robotState.ToDbRobotState(robotId);

                var states = db.RobotStates.Where(x => x.RobotId == robotId);
                db.RobotStates.RemoveRange(states);

                db.RobotStates.Add(dbRobotState);

                var dbOrders = db.Orders.Where(x => x.RobotId == robotId);
                db.Orders.RemoveRange(dbOrders);

                foreach (var order in robotState.Orders.ToList())
                {
                    db.Orders.Add(order.ToDbOrder(robotId));
                }

                db.SaveChanges();
            }
        }     
    }
}