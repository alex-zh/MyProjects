using BitMexConnector.Tables.Classes;
using Common.Robots.Common;

namespace BitMex.TradeApplication.Classes
{
    public static class Extentions
    {
        public static LimitOrder ToLimitOrder(this BitMexLimitOrder bitMexOrder)
        {
            return new LimitOrder
            {
                OrderId = bitMexOrder.OrderId,
                SystemOrderId = bitMexOrder.SystemOrderId,

                Symbol = bitMexOrder.Symbol,
                Size = bitMexOrder.Size,
                Balance = bitMexOrder.LeftSize, //TODO: Not supported
                DateTime = bitMexOrder.DateTime, //TODO: support
                Price = bitMexOrder.Price ?? 0,
                OrderSide = bitMexOrder.Side,
                Status = bitMexOrder.OrderStatus
            };
        }

        public static RobotTrade ToRobotTrade(this BitMexTrade bitMexTrade)
        {
            return new RobotTrade
            {
                TradeId = bitMexTrade.TradeId,
                OrderId = bitMexTrade.OrderId,

                Symbol = bitMexTrade.Symbol,
                Size = bitMexTrade.Size,
                Side = bitMexTrade.Side,

                DateTime = bitMexTrade.DateTime, 
                Price = bitMexTrade.Price,
            };
        }
    }
}
