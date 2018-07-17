using System;

namespace Robot.Core
{
    //Replace with IConnector
    public interface IQuikConnector
    {
        bool IsConnected { get; }

        bool Connect();
        void Disconnect();
        event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;
    }
}