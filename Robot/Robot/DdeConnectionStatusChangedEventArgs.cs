using System;

namespace Robot.Core
{
    public class DdeConnectionStatusChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
    }
}