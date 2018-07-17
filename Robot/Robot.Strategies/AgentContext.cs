using Common;
using Common.Classes.Robot;
using Common.Robots.Common;

namespace Robot.Strategies
{
    public class AgentContext
    {
        public Direction PositionDirection { get; set; }

        public LimitOrderOld LastCompletedOrderOld { get; set; }
    }
}