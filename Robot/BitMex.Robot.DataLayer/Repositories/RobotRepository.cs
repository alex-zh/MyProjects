using System;
using System.Collections.Generic;
using System.Linq;
using BitMex.Robot.DataLayer.Classes;
using Common.Classes.Robot;
using Common.DataLayer.Repositories;
using Common.Robots.Common;

namespace BitMex.Robot.DataLayer.Repositories
{
    public class RobotRepository : IRobotRepository
    {
        private static readonly object _lock = new object();

        public RobotInfo GetById(string robotId)
        {
            using (var db = new RobotContext())
            {
                var dbRobot = db.Robots.FirstOrDefault(x => x.RobotId == robotId);

                if (dbRobot == null)
                {
                    throw new ArgumentException("Robot with id " + robotId + " isn't present in the database");
                }

                return new RobotInfo(dbRobot.RobotId, (RobotModes)dbRobot.Mode, (RobotOperationalStatuses)dbRobot.Status, dbRobot.Size, dbRobot.Symbol);
            }
        }

        public RobotState GetStateByRobotId(string robotId)
        {
            using (var db = new RobotContext())
            {
                var robotInfo = GetById(robotId);

                var dbOrders = db.Orders.Where(x => x.RobotId == robotId);

                var orders = dbOrders.Any() ? dbOrders.ToRobotLimitOrders() : new List<LimitOrder>();

                return new RobotState(robotInfo.Size, orders);
            }
        }

        public void Update(RobotInfo robot)
        {
            lock (_lock)
            {
                using (var db = new RobotContext())
                {
                    var dbRobot = db.Robots.FirstOrDefault(x => x.RobotId == robot.RobotId);

                    if (dbRobot == null)
                    {
                        throw new ArgumentException("Robot with id " + robot.RobotId + " isn't present in the database");
                    }

                    dbRobot.Size = robot.Size;
                    dbRobot.Symbol = robot.Symbol;
                    dbRobot.Mode = (int) robot.Mode;
                    dbRobot.Status = (int) robot.Status;

                    db.SaveChanges();
                }
            }
        }

        public void AddRobot(string robotId)
        {
            using (var db = new RobotContext())
            {
                if (db.Robots.Any(x => x.RobotId == robotId))
                {
                    return;
                }
            }

            using (var db = new RobotContext())
            {
                db.Robots.Add(new DbRobot { RobotId = robotId });
                db.SaveChanges();
            }
        }

        public void DeleteRobotBy(string robotId)
        {
            using (var db = new RobotContext())
            {
                var dbRobots = db.Robots.Where(x => x.RobotId == robotId);
                db.Robots.RemoveRange(dbRobots);

                var ordersToDelete = db.Orders.Where(x => x.RobotId == robotId);
                db.Orders.RemoveRange(ordersToDelete);

                db.SaveChanges();
            }
        }

       
        public void Update(string robotId, RobotState robotState)
        {
            lock (_lock)
            {
                using (var db = new RobotContext())
                {
                    var dbRobot = db.Robots.FirstOrDefault(x => x.RobotId == robotId);

                    if (dbRobot == null)
                    {
                        throw new ArgumentException("Robot with id " + robotId + " isn't present in the database");
                    }

                    dbRobot.Size = robotState.Size;

                    var dbOrders = db.Orders.Where(x => x.RobotId == robotId);
                    db.Orders.RemoveRange(dbOrders);

                    foreach (var order in robotState.GetOrders().ToList())
                    {
                        db.Orders.Add(order.ToDbOrder(robotId));
                    }

                    db.SaveChanges();
                }
            }
        }
    }
}
