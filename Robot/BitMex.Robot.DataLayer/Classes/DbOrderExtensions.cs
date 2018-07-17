using System.Collections.Generic;
using System.Linq;
using Common.Robots.Common;

namespace BitMex.Robot.DataLayer.Classes
{
    public static class DbRobotTradeExtensions
    {
        public static RobotTrade ToRobotTrade(this DbTrade trade)
        {            
            return new RobotTrade
            {
                TradeId = trade.TradeId,
                DateTime= trade.DateTime,
                Price = trade.Price,
                Size = trade.Size, 
                Side = (OrderSides)trade.Side,
                OrderId = trade.OrderId
            };
        }        

        public static IEnumerable<RobotTrade> ToRobotTrades(this IEnumerable<DbTrade> dbTrades)
        {
            return dbTrades.Select(dbTrade => dbTrade.ToRobotTrade()).ToList();
        }
    }

    public static class DbOrderExtensions
    {
        public static LimitOrder ToLimitOrder(this DbOrder order)
        {
            return new LimitOrder
            {
                SystemOrderId = order.SystemOrderId,
                OrderId = order.OrderId,
                Size = order.Size,
                Price = order.Price,
                Balance = order.Side,
                Symbol = order.Symbol,
                DateTime = order.DateTime,                
                Status = (OrderStatuses)order.Status,
                OrderSide = (OrderSides)order.OrderSide
            };
        }

        public static DbOrder ToDbOrder(this LimitOrder order, string robotId)
        {
            return new DbOrder()
            {
                OrderId = order.OrderId,
                SystemOrderId = order.SystemOrderId,
                RobotId = robotId,
                Status = (int)order.Status,
                DateTime = order.DateTime,
                Size = order.Size,
                OrderSide = (int)order.OrderSide,
                Price = order.Price,
                Symbol = order.Symbol
            };
        }

        public static List<LimitOrder> ToRobotLimitOrders(this IEnumerable<DbOrder> orders)
        {
            return orders.Select(dbOrder => dbOrder.ToLimitOrder()).ToList();
        }
    }
}
