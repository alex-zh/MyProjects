using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.Robot;
using Common.Robots.Common;
using Robot.DataLayer.Classes;

namespace Robot.DataLayer.Repositories
{
    public class RobotTradeRepository
    {
        private static readonly object _lock = new object();

        private readonly RobotRepository _repository = new RobotRepository();

        public IEnumerable<RobotTrade> GetAllFor(RobotNames robotName)
        {
            var robotId = _repository.GetByName(robotName).RobotId;

            return GetByRobotId(robotId);
        }

        public IEnumerable<RobotTrade> GetAllFor(RobotNames robotName, DateTime date)
        {
            var robotId = _repository.GetByName(robotName).RobotId;

            lock (_lock)
            {                
                var startDate = date.Date;
                var endDate = date.Date.AddHours(24);

                return GetByRobotId(robotId).Where(x => x.Date > startDate && x.Date < endDate).OrderBy(x => x.TradeNumber);
            }
        }

        public IEnumerable<RobotTrade> GetByRobotId(int robotId)
        {
            lock (_lock)
            {
                using (var db = new RobotContext())
                {
                    var dbTrades = db.RobotTrades.Where(x => x.RobotId == robotId);

                    return dbTrades.ToRobotTrades();
                }
            }
        }       

        public void Update(RobotTrade robotTrade)
        {
            lock (_lock)
            {
                using (var db = new RobotContext())
                {
                    var dbRobotTrade = db.RobotTrades.FirstOrDefault(x => x.Id == robotTrade.Id);

                    if (dbRobotTrade == null)
                    {
                        throw new ArgumentException("Trade with Id " + robotTrade.Id + " isn't present in the database");
                    }

                    dbRobotTrade.Price = robotTrade.Price;
                    dbRobotTrade.Size = robotTrade.Size;
                    dbRobotTrade.DateTime = robotTrade.Date;
                    dbRobotTrade.OrderNumber = robotTrade.OrderNumber;

                    db.SaveChanges();
                }
            }
        }

        public void Add(RobotNames robotName, RobotTrade robotTrade)
        {
            Add(robotName, new[] { robotTrade });
        }

        public void Add(RobotNames robotName, IEnumerable<RobotTrade> trades)
        {
            var robotId = _repository.GetByName(robotName).RobotId;

            lock (_lock)
            {             
                using (var db = new RobotContext())
                {
                    foreach (var robotTrade in trades)
                    {
                        db.RobotTrades.Add(new DbRobotTrade
                        {
                            RobotId = robotId,
                            DateTime = robotTrade.Date,
                            Price = robotTrade.Price,
                            Size = robotTrade.Size,
                            TradeNumber = robotTrade.TradeNumber,
                            OrderNumber = robotTrade.OrderNumber
                        });
                    }

                    db.SaveChanges();

                }
            }
        }
       
        public void DeleteAllFor(RobotNames robotName)
        {
            var robotId = _repository.GetByName(robotName).RobotId;

            lock (_lock)
            {                
                using (var db = new RobotContext())
                {
                    var dbTrades = db.RobotTrades.Where(x => x.RobotId == robotId);
                    db.RobotTrades.RemoveRange(dbTrades);

                    db.SaveChanges();
                }
            }
        }
    }   
}
