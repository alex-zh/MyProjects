using System;
using Common.Classes.Robot;

namespace Robot.Core
{
    /// <summary>
    /// Класс контролирует (балансирует) выставление заявок.
    /// Заявки не должны выставлятся/заменятся слишком часто.
    /// Например, заявку на partial take profit не имеет смысл заменять каждую секунду. 
    /// Это создаст излишнюю нагрузку, которая может привести к самым различным проблемам.
    /// </summary>
    public class RobotOperationTimeScheduler : IRobotOperationTimeScheduler
    {        
        private const int OffsetInSeconds = 29;

        public bool IsEnoughTimeElapsedToUpdateLongSeries(DateTime currentTime, DateTime lastUpdateTime)
        {
            /* обновляем каждые 3 минут */

            if ((currentTime - lastUpdateTime).TotalMinutes > 2)
            {
                return true;
            }

            return false;
        }

        public bool IsEnoughTimeElapsedToReplaceOrder(LimitedOrder order)
        {
            /* считаем что незарегистрированный ордер может быть сразу отправлен в quik */
            if (order.IsActive == false) return true;

            if ((DateTime.Now - order.DateTime).Seconds > OffsetInSeconds)
            {
                return true;
            }

            return false;
        }
    }
   
}