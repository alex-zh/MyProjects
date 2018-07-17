using System;
using Common.Classes.General;
using Common.Classes.Robot;
using Common.Robots.Common;

namespace Robot.Core
{
    public interface IRobotOrderScheduler
    {
        bool ShouldUpdateLongSeries(Candle currentCandle, Candle longSeriesLastCandle);
        bool ShouldReplace(RobotOrderTypes orderType, OrderParams newOrderParams);        
    }
}