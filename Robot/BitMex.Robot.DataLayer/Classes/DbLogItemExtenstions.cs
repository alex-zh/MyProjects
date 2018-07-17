using Common.Classes.Logger;

namespace BitMex.Robot.DataLayer.Classes
{
    public static class DbLogItemExtenstions
    {
        public static DbLogItem ToDbLogItem(this LogItem item, string robotId)
        {
            return new DbLogItem
            {
                Id = item.Id,
                DateTime = item.DateTime,
                MessageType = (int) item.MessageType,
                Text = item.Text,
                RobotId = robotId                 
            };
        }

        public static LogItem ToLogItem(this DbLogItem item)
        {
            return new LogItem
            {
                Id = item.Id,
                RobotId = item.RobotId,
                DateTime = item.DateTime,
                MessageType = (LogMessageTypes)item.MessageType,
                Text = item.Text,
            };
        }
    }
}
