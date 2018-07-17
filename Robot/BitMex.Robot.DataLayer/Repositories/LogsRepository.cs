using System;
using System.Collections.Generic;
using System.Linq;
using BitMex.Robot.DataLayer.Classes;
using Common.Classes.Logger;

namespace BitMex.Robot.DataLayer.Repositories
{
    public class LogsRepository : ILogsRepository
    {
        private static readonly object _lock = new object();

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

        public IEnumerable<LogItem> GetAll(string robotId)
        {
            using (var db = new RobotContext())
            {
                return db.Logs.Where(x => x.RobotId == robotId).ToList().Select(x => x.ToLogItem()).ToList();
            }
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

        public void Add(LogItem item, string robotId)
        {            
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
