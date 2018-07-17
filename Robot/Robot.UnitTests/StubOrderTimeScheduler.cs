using System;
using Common.Classes.General;
using Common.Classes.Robot;
using Common.Robots.Common;
using Robot.Core;

namespace Robot.UnitTests
{
    internal class StubRobotOrderScheduler : IRobotOrderScheduler
    {        
        public bool ShouldUpdateLongSeries(Candle currentCandle, Candle longSeriesLastCandle)
        {
            return true;
        }

        public bool ShouldReplace(RobotOrderTypes orderType, OrderParams newOrderParams)
        {
            throw new NotImplementedException();
        }

        public bool ShouldReplace(OrderParams newOrderParams)
        {
            throw new NotImplementedException();
        }

    }
}