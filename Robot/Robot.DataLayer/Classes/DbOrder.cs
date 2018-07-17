using System;

namespace Robot.DataLayer.Classes
{
    public class DbOrder
    {
        public int Id { get; set; }

        public int RobotId { get; set; }

        public int Status { get; set; }

        public int Size { get; set; }

        public double Price { get; set; }

        public DateTime DateTime { get; set; }

        public int Direction { get; set; }

        public long OrderNumber { get; set; }

        public int RobotOrderType { get; set; }
    }
}