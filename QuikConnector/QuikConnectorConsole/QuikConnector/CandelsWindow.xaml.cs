using System;
using System.Windows;
using QuikConnector.Common;

namespace QuikConnector
{
    /// <summary>
    /// Interaction logic for CandelsWindow.xaml
    /// </summary>
    public partial class CandelsWindow : Window
    {
        private readonly QuikDdeServer _server;

        public CandelsWindow(QuikDdeServer server)
        {
            _server = server;
            InitializeComponent();
            this.Loaded += CandelsWindow_Loaded;           
            _server.DataChanged += DataChanged;
        }

        void CandelsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public void DataChanged(object sender, EventArgs args)
        {
            lvCandles.Dispatcher.Invoke(()=>
            {
                lvCandles.ItemsSource = _server.Candles;    
            });            
        }
    }
}
