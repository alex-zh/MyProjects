using Common.Classes.Robot;
using Common.Robots.Common;

namespace Robot.TradeApplication.Controls
{
    internal class RepresentingLimitOrderOld : LimitOrderOld
    {
        public RepresentingLimitOrderOld(LimitOrderOld orderOld)
        {
            this.Id = orderOld.Id;
            this.DateTime = orderOld.DateTime;
            this.OrderNumber = orderOld.OrderNumber;
            this.Status = orderOld.Status;
            this.Params = orderOld.Params.Clone();
            this.RobotOrderType = orderOld.RobotOrderType;
        }

        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
    }
}