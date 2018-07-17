using System.Collections.Generic;

namespace Common.Classes.Robot
{
    /// <summary>
    /// Contains information on what has been changed
    /// </summary>
    public class RobotStateChangesResult
    {
        public List<string> Errors { get; } = new List<string>();
    }
}