using Common.Classes.Robot;

namespace Robot.Core
{
    public interface IQuikTradeExecutor
    {
        LimitedOrder CreateOrder(Securities security, OrderParams orderTradeParams);
        bool KillOrder(Securities security, long orderNumber);
        LimitedOrder ReplaceOrder(Securities security, long orderNumber, OrderParams orderTradeParams);
    }
}