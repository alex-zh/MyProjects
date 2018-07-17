using Common.Classes.Robot;

namespace Common.Robots.Common
{
    public class RobotInfo
    {
        public string RobotId { get; private set; }

        public RobotModes Mode
        {
            get; set;        
        }

        public double Size
        {
            get; set;
        }

        public string Symbol
        {
            get; set;
        }


        public RobotOperationalStatuses Status
        {
            get;set;
        }

        public RobotInfo(string robotId, RobotModes mode, RobotOperationalStatuses status, double size, string symbol)
        {
            RobotId = robotId;

            Mode = mode;            
            Size = size;
            Status = status;
            Symbol = symbol;
        }
    }
}