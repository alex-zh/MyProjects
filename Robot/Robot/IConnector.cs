using System;

namespace Robot.Core
{
    public interface IConnector
    {
        bool IsConnected { get; }

        bool Connect();
        void Disconnect();

        event EventHandler<ConnectionStateChangedEventArgs> ConnectionStatusChanged;
    }
}