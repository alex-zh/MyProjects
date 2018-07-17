using System;

namespace Robot.DataLayer.Classes
{
    public class DbLogItem
    {
        public int Id { get; set; }
        public int RobotId { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public int MessageType { get; set; }
    }
}
