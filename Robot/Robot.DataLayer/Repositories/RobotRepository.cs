using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.Robot;
using Common.Robots.Common;
using Robot.DataLayer.Classes;

namespace Robot.DataLayer.Repositories
{
    public class RobotRepository: IRobotRepository
    {        
        private  static readonly Dictionary<RobotNames, int> RobotsNameToIdDictionary = new Dictionary<RobotNames, int>(); 

        public RobotRepository()
        {
            FillCache();
        }

        private void FillCache()
        {
            lock (RobotsNameToIdDictionary)
            {
                RobotsNameToIdDictionary.Clear();

                using (var db = new RobotContext())
                {
                    foreach (var robot in db.Robots)
                    {
                        RobotsNameToIdDictionary.Add((RobotNames) Enum.Parse(typeof (RobotNames), robot.Name), robot.Id);
                    }
                }
            }
        }

        public int GetRobotIdOrDefault(RobotNames robotName)        
        {
            lock (RobotsNameToIdDictionary)
            {
                if (RobotsNameToIdDictionary.ContainsKey(robotName))
                {
                    return RobotsNameToIdDictionary[robotName];
                }

                return 0;
            }
        }

        public RobotInfo GetByName(RobotNames robotName)
        {
            var id = GetRobotIdOrDefault(robotName);

            if (id > 0)
            {
                return GetById(id);
            }

            throw new ArgumentException("Robot with name " + robotName + " isn't present in the database");
        }

        public RobotInfo GetById(int id)
        {
            using (var db = new RobotContext())
            {
                var dbRobot = db.Robots.FirstOrDefault(x => x.Id == id);

                if (dbRobot == null)
                {
                    throw new ArgumentException("Robot with id " + id + " isn't present in the database");
                }

                return new RobotInfo(dbRobot.Id, (RobotNames)Enum.Parse(typeof(RobotNames), dbRobot.Name), (RobotModes)dbRobot.Mode, (RobotOperationalStatuses)dbRobot.Status);
            }
        }

        public void Update(RobotInfo robot)
        {
            using (var db = new RobotContext())
            {
                var dbRobot = db.Robots.FirstOrDefault(x => x.Id == robot.RobotId);

                if (dbRobot == null)
                {
                    throw new ArgumentException("Robot with id " + robot.RobotId + " isn't present in the database");
                }

                dbRobot.Mode = (int)robot.Mode;
                dbRobot.Status = (int)robot.Status;                

                db.SaveChanges();
            }
        }

        public void AddRobot(RobotNames robotName)
        {
            var name = robotName.ToString();

            using (var db = new RobotContext())
            {
                if (db.Robots.Any(x => x.Name == name))
                {
                    return;
                }
            }
            
            using (var db = new RobotContext())
            {
                db.Robots.Add(new DbRobot() { Name = name});
                db.SaveChanges();
            }

            FillCache();
        }
       
        public void DeleteRobotBy(RobotNames name)
        {
            using (var db = new RobotContext())
            {
                var dbRobots = db.Robots.Where(x => x.Name == name.ToString());
                db.Robots.RemoveRange(dbRobots);

                foreach (var dbRobot in dbRobots)
                {
                    var robotId = dbRobot.Id;
                    var statesToDelete = db.RobotStates.Where(x => x.RobotId == robotId);
                    db.RobotStates.RemoveRange(statesToDelete);

                    var ordersToDelete = db.Orders.Where(x => x.RobotId == robotId);
                    db.Orders.RemoveRange(ordersToDelete);

                    var tradesToDelete = db.RobotTrades.Where(x => x.RobotId == robotId);
                    db.RobotTrades.RemoveRange(tradesToDelete);    
                }

                db.SaveChanges();
            }
        }
    }
}
