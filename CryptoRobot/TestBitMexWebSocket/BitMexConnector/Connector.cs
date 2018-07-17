using System.Collections.Generic;
using System.Diagnostics;

namespace BitMexConnector
{
    public class Connector
    {
        private IDataProvider _dataProvider;
        private readonly BitMexWebSocket _socket;
        private readonly DeltaParser _deltaParser;
        private readonly ILogger _logger = new TextFileLogger("connector", "C:\\BitMex\\Logs");

        private readonly DataStore _dataStore = new DataStore(Symbols.GetAll());

        private List<string> _systemTables = new List<string> { SystemTables.Order };

        private readonly Dictionary<string, List<string>> _symbolTables = new Dictionary<string, List<string>>
        {
             //{ Symbols.XbtUsd, new List<string>{ SymbolTables.Instrument, SymbolTables.OrderBookL2 } },
        };


        public IDataProvider DataProvider => _dataProvider;

        public Connector()
        {
            _deltaParser = new DeltaParser(_dataStore, _logger);
            _socket = new BitMexWebSocket(BitMexConfig.WebSocketUrl, _logger);
            _dataProvider = new DataProvider(_dataStore);

            _socket.Opened += _socket_Opened;
            _socket.DataReceived += _socket_DataReceived;

        }

        private void _socket_DataReceived(object sender, BitMexDataReceivedEventArgs e)
        {
            if (e.Data.ContainsKey("status") && e.Data["status"].ToString() == "400")
            {
                var requestMessage = (string)e.Data["error"] ?? "";
                var errorMessage = $"Bad request: {requestMessage}";

                _logger.Error(errorMessage);
                Debug.WriteLine(errorMessage);

                return;
            }

            _deltaParser.Parse(e.Data);
        }

        public void Connect()
        {
            _socket.Connect();
        }

        public void Disconnect()
        {
            _socket.Disconnect();
        }

        private void _socket_Opened(object sender, System.EventArgs e)
        {
            SetAuthentication();
            SubscribeSystemTopics();
            SubscribeSymbolTopics();
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
