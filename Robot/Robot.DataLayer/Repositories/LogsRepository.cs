using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using Robot.DataLayer.Classes;

namespace Robot.DataLayer.Repositories
{
    public class LogsRepository : ILogsRepository
    {
        private static readonly object _lock = new object();
        private RobotRepository _robotRepository  = new RobotRepository();

        public IEnumerable<LogItem> GetAll()
        {
            return GetAll(x => true);
        }

        public IEnumerable<LogItem> GetAll(Func<LogItem, bool> filter)
        {
            using (var db = new RobotContext())
            {
                return db.Logs.ToList().Select(x => x.ToLogItem()).Where(filter).ToList();
            }
        }

        public IEnumerable<LogItem> GetAll(int robotId)
        {
            using (var db = new RobotContext())
            {
                return db.Logs.Where(x => x.RobotId == robotId).ToList().Select(x => x.ToLogItem()).ToList();
            }
        }

        public IEnumerable<LogItem> GetAll(RobotNames robotName)
        {
            var robotId = _robotRepository.GetRobotIdOrDefault(robotName);

            return GetAll(robotId);
        }

        public void Add(LogItem logItem)
        {
            lock (_lock)
            {
                using (var db = new RobotContext())
                {
                    db.Logs.Add(logItem.ToDbLogItem(null));
                    db.SaveChanges();
                }
            }
        }

        public void Add(LogItem item, RobotNames robotName)
        {            
            var robotId = _robotRepository.GetRobotIdOrDefault(robotName) ;
            
            lock (_lock)
            {
                using (var db = new RobotContext())
                {
                    db.Logs.Add(item.ToDbLogItem(robotId));
                    db.SaveChanges();
                }
            }
        }

        public void ClearAll()
        {
            lock (_lock)
            {
                using (var db = new RobotContext())
                {
                    db.Logs.RemoveRange(db.Logs);
                    db.SaveChanges();
                }
            }
        }        
    }
}
