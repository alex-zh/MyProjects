using System;

namespace Robot.Core
{
    public interface IAgentOperationTimeScheduler
    {
        bool IsEnoughTimeElapsedToRecalculateMovingsDeviations(DateTime timeOfLastCandle, DateTime timeOfLastUpdate);
        bool IsEnoughTimeElapsedToRecalculateMovingsIntersections(DateTime timeOfLastCandle, DateTime timeOfLastUpdate);
    }
}