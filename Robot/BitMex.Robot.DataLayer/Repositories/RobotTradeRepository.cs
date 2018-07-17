using System;
using System.Collections.Generic;
using System.Linq;
using BitMex.Robot.DataLayer.Classes;
using Common.DataLayer.Repositories;
using Common.Robots.Common;

namespace BitMex.Robot.DataLayer.Repositories
{
    public class RobotTradeRepository: IRobotTradeRepository
    {
        private static readonly object _lock = new object();

        public IEnumerable<RobotTrade> GetAll(string robotId, DateTime date)
        {
            lock (_lock)
            {                
                var startDate = date.Date;
                var endDate = date.Date.AddHours(24);

                return GetAll(robotId).Where(x => x.DateTime > startDate && x.DateTime < endDate).OrderBy(x => x.DateTime);
            }
        }

        public IEnumerable<RobotTrade> GetAll(string robotId)
        {
            lock (_lock)
            {
                using (var db = new RobotContext())
                {
                    var dbTrades = db.Trades.Where(x => x.RobotId == robotId);

                    return dbTrades.ToRobotTrades();
                }
            }
        }

        public IEnumerable<RobotTrade> GetAll(string robotId, string orderId)
        {
            return GetAll(robotId).Where(x => x.OrderId == orderId);
        }


        public void Update(RobotTrade robotTrade)
        {
            lock (_lock)
            {
                using (var db = new RobotContext())
                {
                    var dbRobotTrade = db.Trades.FirstOrDefault(x => x.TradeId == robotTrade.TradeId);

                    if (dbRobotTrade == null)
                    {
                        throw new ArgumentException("Trade with Id " + robotTrade.TradeId + " isn't present in the database");
                    }

                    dbRobotTrade.Price = robotTrade.Price;
                    dbRobotTrade.Size = robotTrade.Size;
                    dbRobotTrade.DateTime = robotTrade.DateTime;
                    dbRobotTrade.OrderId = robotTrade.OrderId;

                    db.SaveChanges();
                }
            }
        }

        
        public void Add(string robotId, RobotTrade robotTrade)
        {
            Add(robotId, new[] { robotTrade });
        }

        public void Add(string robotId, IEnumerable<RobotTrade> trades)
        {
            lock (_lock)
            {             
                using (var db = new RobotContext())
                {
                    foreach (var robotTrade in trades)
                    {
                        db.Trades.Add(new DbTrade
                        {
                            RobotId = robotId,
                            DateTime = robotTrade.DateTime,
                            Price = robotTrade.Price,
                            Size = robotTrade.Size,
                            TradeId = robotTrade.TradeId,
                            OrderId = robotTrade.OrderId
                        });
                    }

                    db.SaveChanges();

                }
            }
        }
       
        public void RemoveAll(string robotId)
        {
            lock (_lock)
            {                
                using (var db = new RobotContext())
                {
                    var dbTrades = db.Trades.Where(x => x.RobotId == robotId);
                    db.Trades.RemoveRange(dbTrades);

                    db.SaveChanges();
                }
            }
        }

        public void RemoveAll(string robotId, string orderId)
        {
            lock (_lock)
            {
                using (var db = new RobotContext())
                {
                    var dbTrades = db.Trades.Where(x => x.RobotId == robotId && x.OrderId == orderId);
                    db.Trades.RemoveRange(dbTrades);

                    db.SaveChanges();
                }
            }
        }

    }
}
