using System;
using System.Windows;
using QuikConnector.Classes;
using QuikConnector.Common;
using QuikConnector.Common.Classes;

namespace QuikConnector
{    
    public partial class MainWindow
    {
        private QuikDdeServer _ddeServer;
        private readonly ILogger _logger;
        private QuikProxy _proxy;

        public MainWindow()
        {
            InitializeComponent();
            _logger = new Logger(tbLog);
        }

        private void BtnRegister_OnClick(object sender, RoutedEventArgs e)
        {
            if (_ddeServer == null)
            {
                _ddeServer = new QuikDdeServer("RobotDdeServer", _logger);
                _ddeServer.Register();
            }
        }

        private void BtnUnregister_OnClick(object sender, RoutedEventArgs e)
        {
            if (_ddeServer != null)
            {
                _ddeServer.Unregister();
                _ddeServer = null;
            }
        }

        private void BtnOpenCandles_OnClick(object sender, RoutedEventArgs e)
        {
            if (_ddeServer != null)
            {
                var cw = new CandelsWindow(_ddeServer);
                cw.Show();
            }
        }        

        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
           _proxy = new QuikProxy(_logger);
            _proxy.Start();
        }

        private void BtnStop_OnClick(object sender, RoutedEventArgs e)
        {
            _proxy.Stop();
        }

        private void BtnSendOrder_OnClick(object sender, RoutedEventArgs e)
        {
            _proxy.SetLimitedOrder();
        }

        private void BtnClearLog_OnClick(object sender, RoutedEventArgs e)
        {
            tbLog.Text = "";
        }

        private void BtnKillOrder_OnClick(object sender, RoutedEventArgs e)
        {
            _proxy.KillLimitedOrder();
        }
    }
}
