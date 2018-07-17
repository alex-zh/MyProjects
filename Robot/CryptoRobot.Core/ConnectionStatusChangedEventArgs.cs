using System;

namespace Robot.Core
{
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public bool IsDllConnected { get; set; }
        public bool IsQuikConnected { get; set; }        
    }
}