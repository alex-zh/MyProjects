using System.Windows;
using System.Windows.Media;
using BitMex.TradeApplication.Classes;
using Robot.Core;

namespace BitMex.TradeApplication.Controls
{
    public partial class ConnectionSummaryControl
    {
        private const string ConnectString = "Connect";
        private const string DisconnectString = "Disconnect";
        private const string ConnectedString = "Connected";
        private const string NotConnectedString = "Not Connected";

        private IConnector _connector;

        public ConnectionSummaryControl()
        {
            InitializeComponent();

            _connector = MarketConnectorKeeper.Get();
            _connector.ConnectionStatusChanged += _connector_ConnectionStatusChanged;
        }

        private void _connector_ConnectionStatusChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                btnConnect.Content = (e.IsWebSocketConnected) ? DisconnectString : ConnectString;

                tbRobotWebSocketConnectionStatus.Text = e.IsWebSocketConnected ? ConnectedString : NotConnectedString;
                tbRobotWebSocketConnectionStatus.Foreground = e.IsWebSocketConnected ? Brushes.Green : Brushes.DarkBlue;
            });

        }

        private void BtnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_connector.IsConnected)
            {
                _connector.Connect();
            }
            else
            {
                _connector.Disconnect();
            }
        }
    }
}