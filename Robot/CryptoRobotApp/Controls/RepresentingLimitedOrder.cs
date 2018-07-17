using Common.Classes.Robot;

namespace Robot.TradeApplication.Controls
{
    internal class RepresentingLimitedOrder : LimitedOrder
    {
        public RepresentingLimitedOrder(LimitedOrder order)
        {
            this.Id = order.Id;
            this.DateTime = order.DateTime;
            this.OrderNumber = order.OrderNumber;
            this.Status = order.Status;
            this.Params = order.Params.Clone();
            this.RobotOrderType = order.RobotOrderType;
        }

        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
    }
}