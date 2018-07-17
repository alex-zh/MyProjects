using System;
using Common.Classes.Robot;

namespace Robot.Core
{
    public interface IRobotOperationTimeScheduler
    {
        bool IsEnoughTimeElapsedToUpdateLongSeries(DateTime currentTime, DateTime lastUpdateTime);
        bool IsEnoughTimeElapsedToReplaceOrder(LimitedOrder order);        
    }
}