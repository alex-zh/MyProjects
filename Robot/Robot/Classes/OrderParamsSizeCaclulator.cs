using System;
using Common;
using Common.Classes.Robot;
using Common.Robots.Common;
using Common.StrategiesOptions;

namespace Robot.Core.Classes
{
    public class OrderParamsSizeCaclulator
    {
        private readonly StrategyOptions _options;

        public OrderParamsSizeCaclulator(StrategyOptions options)
        {
            _options = options;
        }

        public OrderParams Get(RobotOrderTypes orderType, AgentActionParams actionParams, int currentPositionSize)
        {
            int size = 0;
            Direction? direction = null;

            if (orderType == RobotOrderTypes.OpenPosition)
            {
                direction = actionParams.Direction;

                if (direction == Direction.Buy)
                {
                    size = Math.Min(_options.MaxPositionSize, currentPositionSize + _options.PositionSize) - currentPositionSize;
                }
                else if (direction == Direction.Sell)
                {
                    size = currentPositionSize - Math.Max(currentPositionSize - _options.PositionSize, _options.MinPositionSize);
                }
            }
            else if (orderType == RobotOrderTypes.ClosePosition ||
                     orderType == RobotOrderTypes.StopLoss)
            {
                size = Math.Abs(currentPositionSize);
                direction = currentPositionSize > 0 ? Direction.Sell : Direction.Buy;
            }
            else if ((currentPositionSize > 0 && orderType == RobotOrderTypes.AddToPosition) ||
                     (currentPositionSize < 0 && orderType == RobotOrderTypes.TakeProfit))
            {
                direction = Direction.Buy;
                size = _options.HalfPositionSize;
            }

            else if ((currentPositionSize < 0 && orderType == RobotOrderTypes.AddToPosition) ||
                     (currentPositionSize > 0 && orderType == RobotOrderTypes.TakeProfit))
            {
                direction = Direction.Sell;
                size = _options.HalfPositionSize;
            }

            if (direction.HasValue)
            {
                return new OrderParams(direction.Value, size, actionParams.Price);
            }

            return null;
        }
    }
}