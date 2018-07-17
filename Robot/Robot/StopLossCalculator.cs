using Common;

namespace Robot.Core
{
    internal class StopLossCalculator
    {
        public static double GetFor(double openPositionPrice, double stopLimitInPercent, Direction openPositionDirection)
        {
            if (openPositionDirection == Direction.Buy)
            {
                return openPositionPrice*(1 - stopLimitInPercent/(100));
            }
            else
            {
                return openPositionPrice * (1 + stopLimitInPercent / (100));
            }
        }
    }
}