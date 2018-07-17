using System;
using Common.Classes;
using Common.Classes.Logger;
using Robot.Core.Keepers;
using Robot.Trans2Quik;

namespace Robot.Core
{
    public class QuikConnector: IQuikConnector
    {
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

        private readonly ILogger _logger;
        private readonly Gateway _gateway;

        public QuikConnector(ILogger logger)
        {
            _logger = logger;
            _gateway = QuikGatewayKeeper.Get();
            _gateway.ConnectionStatusChanged += _gateway_ConnectionStatusChanged;
        }

        public bool IsConnected
        {
            get { return _gateway.IsConnected; }
        }

        public bool Connect()
        {           
            if (_gateway.Connect() == false)
            {
                _logger.Log("Could not connect to Quik via Trans2Quik.", LogMessageTypes.Error);
                return false;
            }

            return true;
        }
      
        public void Disconnect()
        {
            _gateway.Disconnect();
        }               

        void _gateway_ConnectionStatusChanged(object sender, EventArgs e)
        {
            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this,
                    new ConnectionStatusChangedEventArgs()
                    {
                        IsQuikConnected = _gateway.IsQuikConnected,
                        IsDllConnected = _gateway.IsDllConnected
                    });
        }
    }
}