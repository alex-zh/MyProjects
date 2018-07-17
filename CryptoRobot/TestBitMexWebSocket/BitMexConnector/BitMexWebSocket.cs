using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using WebSocket = WebSocketSharp.WebSocket;

namespace BitMexConnector
{
    class BitMexWebSocket
    {
        private const int StatusCloseNormal = 1000;
        private const int  StatusCloseUnexpected = 1011;
        private const int StatusCloseDowntime = 1012;

        public event EventHandler Opened;
        public event EventHandler Closed;
        public event EventHandler<BitMexDataReceivedEventArgs> DataReceived;

        private int _initialAutoReconnectInterval = 1000;    // ms
        private int _autoReconnectInterval = 1000;
        private WebSocket _webSocket;
        private string _bitMexWebSocketUrl;
        private readonly ILogger _logger;
        private bool _isConnected;
        private readonly object _lock = new object();

        public BitMexWebSocket(string bitMexWebSocketUrl, ILogger logger)
        {
            _bitMexWebSocketUrl = bitMexWebSocketUrl;
            _logger = logger;
        }

        public void Connect()
        {
            lock (_lock)
            {
                if (_isConnected == false)
                {
                    _isConnected = true;

                    _webSocket = new WebSocket(_bitMexWebSocketUrl);

                    _webSocket.OnOpen += Websocket_OnOpen;
                    _webSocket.OnMessage += Websocket_OnMessage;
                    _webSocket.OnClose += _webSocket_OnClose;
                    _webSocket.OnError += _webSocket_OnError;
                    
                    _webSocket.ConnectAsync();
                }
            }
        }
        
        public void Disconnect()
        {
            if (_webSocket != null)
            {                
                _webSocket.OnOpen -= Websocket_OnOpen;
                _webSocket.OnMessage -= Websocket_OnMessage;
                _webSocket.OnClose -= _webSocket_OnClose;
                _webSocket.OnError -= _webSocket_OnError;

                _webSocket.Close();

                _webSocket = null;
                _isConnected = false;
            }
        }

        public void Subscribe(AuthSubscriptionTopic topic)
        {
            var operation = "{\"op\":\"authKeyExpires\",\"args\":[\""+topic.ApiKey+ "\"," + topic.Expires+ ",\"" + topic.Signature+ "\"]}";
            _logger.Info(operation);

            _webSocket.Send(operation);
        }

        public void Subscribe(SubscriptionTopic topic)
        {            
            var operation = "{\"op\":\"subscribe\",\"args\":\"" + topic + "\"}";
            _logger.Info(operation);

            _webSocket.Send(operation);
        }        

        private void Websocket_OnOpen(object sender, EventArgs e)
        {
            Opened?.Invoke(this, EventArgs.Empty);
            
        }

        private void Websocket_OnMessage(object sender, MessageEventArgs e)
        {
            DataReceived?.Invoke(this, new BitMexDataReceivedEventArgs()
            {
                Data = JObject.Parse(e.Data)
            });
        }

        private void _webSocket_OnClose(object sender, CloseEventArgs e)
        {            
            switch (e.Code)
            {
                case StatusCloseNormal:
                    _logger.Info("Socket closed");
                    break;
                case StatusCloseUnexpected:
                    _logger.Error("WebSocket closed unexpectedly.");
                    break;
                default:    // Abnormal closure
                    _logger.Error($"WebSocket closed abnormally with code ${e.Code}.");
                    break;
            }

            OnClose(e.Code);
            
        }

        private void _webSocket_OnError(object sender, ErrorEventArgs e)
        {
            _logger.Error(e.Exception.ToString());
        }


        private void OnClose(int code)
        {
            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }
    }
}
