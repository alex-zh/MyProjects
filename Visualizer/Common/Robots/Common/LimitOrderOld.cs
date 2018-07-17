using System;
using Common.Classes.Robot;

namespace Common.Robots.Common
{
    public class LimitOrderOld
    {
        public LimitOrderOld()
        {
            RobotOrderType = RobotOrderTypes.None;
            Status = OrderStatuses.NotRegistered;
            
            Params = new OrderParams(OrderSides.Buy, 0, 0.0);
        }

        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public long OrderNumber { get; set; }

        /// <summary>
        /// Shows how many contracts are not completed yet. 
        /// Initially balance equals size, when order is fully completed balance equals zero.
        /// </summary>
        public int Balance { get; set; }

        public bool IsActive {
            get
            {
                return Status == OrderStatuses.Active;
            } 
        }        

        public RobotOrderTypes RobotOrderType
        {
            get;
            set;
        }

        public OrderStatuses Status
        {
            get;
            set;
        }

        public OrderParams Params { get; set; }

        public Securities Security { get; set; }

        public LimitOrderOld Clone()
        {
            return new LimitOrderOld
            {                
                Id = Id,
                Status = Status,
                OrderNumber =  OrderNumber,
                DateTime = DateTime,
                RobotOrderType = RobotOrderType,          
                Security = Security,
                Params = Params != null ? Params.Clone() : null,
                Balance = Balance
            };
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}, {5} ", OrderNumber, DateTime, Status, RobotOrderType, Balance, Params);
        }
    }
}