using System;

namespace Robot.Core
{
    public interface IQuikConnector
    {
        bool IsConnected { get; }

        bool Connect();
        void Disconnect();
        event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;
    }
}