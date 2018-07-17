using System.Collections.Generic;

namespace Common.Robots.Common
{
    public interface IRobotState: IRobotStateGetter, IRobotStateUpdator
    {
    }

    public interface IRobotStateGetter
    {
        double Size { get; }
        IEnumerable<LimitOrder> GetOrders();       
    }

    public interface IRobotStateUpdator
    {
        void SetSize(double size);
        void AddSize(double addSize);
        void AddOrUpdateOrder(LimitOrder order);
        
        void RemoveOrder(string orderId);
        void RemoveOrders(IEnumerable<string> orderIds);     
    }

    
}