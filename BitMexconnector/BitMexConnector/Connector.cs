using System;
using System.Collections.Generic;
using Common.Classes.Logger;
using Common.Robots.Common;

namespace BitMexConnector
{
    public class Connector
    {
        public event EventHandler<EventArgs> ConnectionOpened;  
        public event EventHandler<EventArgs> ConnectionClosed;

        private readonly ILogger _logger;
        private IDataProvider _dataProvider;
        private readonly BitMexWebSocket _socket;
        private readonly DeltaParser _deltaParser;       

        private readonly DataStore _dataStore = new DataStore(Symbols.BitMex.GetAll());

        private List<string> _systemTables = new List<string> { SystemTables.Order, SystemTables.Execution };

        private readonly Dictionary<string, List<string>> _symbolTables = new Dictionary<string, List<string>>
        {
             { Symbols.BitMex.XBTUSD, new List<string>{ SymbolTables.OrderBookL2} },
             { Symbols.BitMex.XRPU18, new List<string>{ SymbolTables.OrderBookL2} },
        };

        public IDataProvider DataProvider => _dataProvider;

        public Connector(ILogger logger)
        {
            _logger = logger;

            _deltaParser = new DeltaParser(_dataStore, _logger);
            _socket = new BitMexWebSocket(BitMexConfig.WebSocketUrl, _logger);
            _dataProvider = new DataProvider(_dataStore);

            _socket.Opened += _socket_Opened;
            _socket.Closed += _socket_Closed;
            _socket.DataReceived += _socket_DataReceived;            
        }

        
        private void _socket_DataReceived(object sender, BitMexDataReceivedEventArgs e)
        {
            try
            {
                if (e.Data.ContainsKey("status") && e.Data["status"].ToString() == "400")
                {
                    var requestMessage = (string) e.Data["error"] ?? "";
                    var errorMessage = $"Bad request: {requestMessage}";

                    _logger.Error(errorMessage);

                    return;
                }

                _deltaParser.Parse(e.Data);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }

        public bool IsConnected => (_socket != null) && _socket.IsConnected;

        public void Connect()
        {
            _socket.Connect();
        }

        public void Disconnect()
        {
            _socket.Disconnect();
        }

        private void _socket_Opened(object sender, EventArgs e)
        {
            SetAuthentication();
            SubscribeSystemTopics();
            SubscribeSymbolTopics();

            ConnectionOpened?.Invoke(this, e);
        }

        private void _socket_Closed(object sender, EventArgs e)
        {
            ConnectionClosed?.Invoke(this, e);
        }


        private void SetAuthentication()
        {
            var topic = new AuthSubscriptionTopic(BitMexConfig.PublicBitmexKey, BitMexConfig.PrivateBitmexKey);
            _socket.Subscribe(topic);
        }

        private void SubscribeSystemTopics()
        {
            foreach (var tableName in _systemTables)
            {
                var topic = new SubscriptionTopic { Table = tableName };

                _socket.Subscribe(topic);
            }
        }

        private void SubscribeSymbolTopics()
        {
            foreach (var symbolName in _symbolTables.Keys)
            {
                foreach (var tableName in _symbolTables[symbolName])
                {
                    var topic = new SubscriptionTopic { Table = tableName, Symbol = symbolName };
                    _socket.Subscribe(topic);
                }
            }
        }
    }
}
