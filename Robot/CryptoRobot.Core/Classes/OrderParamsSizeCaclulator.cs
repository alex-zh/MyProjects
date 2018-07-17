using System;
using Common;
using Common.Classes.Robot;

namespace Robot.Core.Classes
{
    public class OrderParamsSizeCaclulator
    {
        private readonly RobotExecutionOptions _executionOptions;

        public OrderParamsSizeCaclulator(RobotExecutionOptions executionOptions)
        {
            _executionOptions = executionOptions;
        }

        public OrderParams Get(RobotOrderTypes orderType, AgentActionParams actionParams, int currentPositionSize)
        {
            int size = 0;
            Direction? direction = null;

            if (orderType == RobotOrderTypes.OpenPosition)
            {
                direction = actionParams.Direction;

                if (direction == Direction.Up)
                {
                    size = Math.Min(_executionOptions.MaxPositionSize, currentPositionSize + _executionOptions.PositionSize) - currentPositionSize;
                }
                else if (direction == Direction.Down)
                {
                    size = currentPositionSize - Math.Max(currentPositionSize - _executionOptions.PositionSize, _executionOptions.MinPositionSize);
                }
            }
            else if (orderType == RobotOrderTypes.ClosePosition ||
                     orderType == RobotOrderTypes.StopLoss)
            {
                size = Math.Abs(currentPositionSize);
                direction = currentPositionSize > 0 ? Direction.Down : Direction.Up;
            }
            else if ((currentPositionSize > 0 && orderType == RobotOrderTypes.AddToPosition) ||
                     (currentPositionSize < 0 && orderType == RobotOrderTypes.TakeProfit))
            {
                direction = Direction.Up;
                size = _executionOptions.HalfPositionSize;
            }

            else if ((currentPositionSize < 0 && orderType == RobotOrderTypes.AddToPosition) ||
                     (currentPositionSize > 0 && orderType == RobotOrderTypes.TakeProfit))
            {
                direction = Direction.Down;
                size = _executionOptions.HalfPositionSize;
            }

            if (direction.HasValue)
            {
                return new OrderParams(direction.Value, size, actionParams.Price);
            }

            return null;
        }
    }
}