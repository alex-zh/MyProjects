using System;
using BitMexConnector;
using Common.Classes.Logger;
using Robot.Core;

namespace BitMex.TradeApplication.Classes
{
    public class MarketConnector : IConnector
    {
        private readonly ILogger _logger;
        private readonly Connector _connector;

        public MarketConnector(ILogger logger)
        {
            _logger = logger;
            _connector = ConnectorKeeper.GetConnector();

            _connector.ConnectionOpened += _connector_ConnectionOpened;
            _connector.ConnectionClosed += _connector_ConnectionClosed;
        }

        private void _connector_ConnectionOpened(object sender, EventArgs e)
        {
          ConnectionStatusChanged?.Invoke(this, new ConnectionStateChangedEventArgs() {IsWebSocketConnected = true});   
        }

        private void _connector_ConnectionClosed(object sender, EventArgs e)
        {
            ConnectionStatusChanged?.Invoke(this, new ConnectionStateChangedEventArgs() { IsWebSocketConnected = false });
        }

        public bool IsConnected => _connector.IsConnected;

        public bool Connect()
        {
            try
            {
                _connector.Connect();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());

                return false;
            }

            return true;
        }

        public void Disconnect()
        {
            try
            {
                _connector.Disconnect();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStatusChanged;
    }
}