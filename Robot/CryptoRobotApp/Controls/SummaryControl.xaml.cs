using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Robot.Core;
using Robot.Core.Providers;

namespace Robot.TradeApplication.Controls
{
    public partial class SummaryControl
    {
        private const string ConnectString = "Connect";
        private const string DisconnectString = "Disconnect";
        private const string ConnectedString = "Connected";
        private const string NotConnectedString = "Not Connected";

        private IQuikConnector _quikConnector;
        private IQuikDataProvider _dataProvider;

        public SummaryControl()
        {
            InitializeComponent();
        }

        public IQuikConnector QuikConnector
        {
            get { return _quikConnector; }
            set
            {
                _quikConnector = value;
                _quikConnector.ConnectionStatusChanged += _quikConnector_ConnectionStatusChanged;
            }
        }

        public IQuikDataProvider DataProvider
        {
            get { return _dataProvider; }
            set
            {
                _dataProvider = value;
                _dataProvider.TimeChanged += DataProviderTimeChanged;  
            }
        }        

        private void _quikConnector_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                btnConnect.Content = (e.IsDllConnected && e.IsQuikConnected) ? DisconnectString : ConnectString;

                tbRobotToQuikStatus.Text = e.IsDllConnected ? ConnectedString : NotConnectedString;
                tbRobotToQuikStatus.Foreground = e.IsDllConnected ? Brushes.Green : Brushes.DarkBlue;

                tbQuikToServerStatus.Text = e.IsQuikConnected ? ConnectedString : NotConnectedString;
                tbQuikToServerStatus.Foreground = e.IsQuikConnected ? Brushes.Green : Brushes.DarkBlue;
            });
        }

        private void DataProviderTimeChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var quikTime = _dataProvider.GetServerTime();
                tbQuikTime.Text = quikTime.ToString("hh:mm:ss", CultureInfo.InvariantCulture);

                tbRobotTime.Text = DateTime.Now.ToString("hh:mm:ss", CultureInfo.InvariantCulture);

                if ((quikTime - DateTime.Now).Seconds > 1)
                {
                    tbQuikDdeStatus.Text = NotConnectedString;
                    tbQuikDdeStatus.Foreground = Brushes.DarkBlue;
                }
                else
                {
                    tbQuikDdeStatus.Text = ConnectedString;
                    tbQuikDdeStatus.Foreground = Brushes.Green;
                }
            });
        }

        private void BtnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_quikConnector.IsConnected)
            {
                QuikConnector.Connect();
            }
            else
            {
                QuikConnector.Disconnect();
            }
        }
    }
}