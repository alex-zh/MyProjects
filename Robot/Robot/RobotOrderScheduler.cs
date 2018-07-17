using System;
using System.Linq;
using Common.Classes.General;
using Common.Classes.Robot;
using Common.Robots.Common;

namespace Robot.Core
{
    /// <summary>
    /// Analizes should newOrderParams be send to the market
    /// </summary>
    public class RobotOrderScheduler : IRobotOrderScheduler
    {        
        private const int OffsetInSeconds = 29;
        private RobotStateManager _stateManager;

        public RobotOrderScheduler(RobotStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public bool ShouldUpdateLongSeries(Candle currentCandle, Candle longSeriesLastCandle)
        {
            /* обновляем каждые N минут */

            if ((currentCandle.Date - longSeriesLastCandle.Date).TotalMinutes >= 5)
            {
                return true;
            }

            return false;
        }

        public bool ShouldReplace(RobotOrderTypes orderType, OrderParams newOrderParams)
        {
            var currentOrder = _stateManager.GetOrdersByType(orderType).FirstOrDefault();

            if (currentOrder == null)
                return true;

            if (Math.Abs((currentOrder.Params.Price - newOrderParams.Price)/currentOrder.Params.Price) < 0.04/100)
            {
                return false;
            }

            return true;
        }
    }
   
}