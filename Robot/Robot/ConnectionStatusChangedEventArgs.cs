using System;

namespace Robot.Core
{
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public bool IsDllConnected { get; set; }
        public bool IsQuikConnected { get; set; }        
    }

    public class ConnectionStateChangedEventArgs : EventArgs
    {
        public bool IsWebSocketConnected { get; set; }
    }
}