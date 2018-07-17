using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes.Robot;
using Common.Robots.Common;

namespace Robot.DataLayer.Classes
{
    public static class DbRobotTradeExtensions
    {
        public static RobotTrade ToRobotTrade(this DbRobotTrade robotTrade)
        {            
            return new RobotTrade
            {
                Id = robotTrade.Id,
                Date= robotTrade.DateTime,
                Price = robotTrade.Price,
                Size = robotTrade.Size, 
                TradeNumber = robotTrade.TradeNumber,
                OrderNumber = robotTrade.OrderNumber
            };
        }        

        public static IEnumerable<RobotTrade> ToRobotTrades(this IEnumerable<DbRobotTrade> dbTrades)
        {
            return dbTrades.Select(dbTrade => dbTrade.ToRobotTrade()).ToList();
        }
    }

    public static class DbOrderExtensions
    {
        public static LimitOrder ToLimitedOrder(this DbOrder order)
        {
            var orderParams = new OrderParams((Direction) order.Direction, order.Size, order.Price);

            return new LimitOrder
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber, 
                Params = orderParams,
                DateTime = order.DateTime,
                Status = (OrderStatuses)order.Status,
                RobotOrderType = (RobotOrderTypes)order.RobotOrderType
            };
        }

        public static DbOrder ToDbOrder(this LimitOrder order, int robotId)
        {
            return new DbOrder()
            {
                Id = order.Id,
                RobotId = robotId,
                Status = (int)order.Status,
                OrderNumber = order.OrderNumber,
                RobotOrderType = (int)order.RobotOrderType,
                DateTime = order.DateTime,
                Size = order.Params.Size,
                Direction = (int)order.Params.Direction,
                Price = order.Params.Price,

            };
        }

        public static Dictionary<RobotOrderTypes, LimitOrder> ToRobotLimitedOrders(this IEnumerable<DbOrder> orders)
        {
            return orders.Select(dbOrder => dbOrder.ToLimitedOrder()).ToDictionary(order => order.RobotOrderType);
        }
    }
}
