using System;

namespace Common.Classes.Logger
{
    public class LogItem
    {        
        public int Id { get; set; }

        public string RobotId { get; set; }

        public string Text { get; set; }

        public DateTime DateTime { get; set; }

        public LogMessageTypes MessageType { get; set; }
    }
}