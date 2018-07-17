using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BitMexConnector;
using BitMexConnector.Tables.Classes;
using Common.Classes.Logger;
using Common.Robots.Common;
using TextFileLogger = BitMexConnector.TextFileLogger;

namespace TestApplication
{
    public partial class MainWindow
    {
        private ILogger _logger = new TextFileLogger("application", "C:\\BitMex\\Logs");

        private Connector _connector = new Connector(new TextFileLogger("connector", "C:\\BitMex\\Logs"));
        private CancellationTokenSource _cts;
        

        public ObservableCollection<OrderBookEntry> Orders { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ClOrderId.Text = Guid.NewGuid().ToString();
        }


        private async void Connect_OnClick(object sender, RoutedEventArgs e)
        {
            _connector.Connect();

            _cts = new CancellationTokenSource();

            try
            {
                await UpdateOrderBook(_cts.Token);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("Canceled by user");
            }
        }

        private void Disconnect_OnClick(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();

            _connector.Disconnect();
        }

        private async Task UpdateOrderBook(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var orderBook = _connector.DataProvider.GetOrderBook(Symbols.BitMex.XBTUSD);
                var orders = _connector.DataProvider.GetOrders();
                var trades = _connector.DataProvider.GetTrades();

                Dispatcher.Invoke(() =>
                {
                    Bids.ItemsSource = orderBook.Bids.OrderByDescending(x => x.Price).Take(10).ToList();
                    Asks.ItemsSource = orderBook.Asks.OrderBy(x => x.Price).Take(10).ToList();
                    OrdersList.ItemsSource= orders.ToList();
                    TradesList.ItemsSource = trades.ToList();
                });

                await Task.Delay(1000, cancellationToken);
            }
        }

        private void SetOrderHandler(object sender, RoutedEventArgs e)
        {
            var size = int.Parse(Size.Text);
            var price = double.Parse(Price.Text);
            var symbol = Symbol.Text;
            var clientOrderId = ClOrderId.Text;

            var operationExecutor = new BitMexOperationExecutor(_logger);
            //operationExecutor.SetOrder(new BitMexLimitOrder(){ OrderId = clientOrderId, Price = price, Size = size, Symbol = symbol });
            operationExecutor.SetOrders(new List<BitMexLimitOrder>()
            {
                new BitMexLimitOrder() {OrderId = clientOrderId, Price = price, Size = size, Symbol = symbol}
            });

        }

        private void DeleteOrderHandler(object sender, RoutedEventArgs e)
        {
            var clientOrderIdToDelete = ClOrderIdToDelete.Text;

            var operationExecutor = new BitMexOperationExecutor(_logger);
            operationExecutor.DeleteOrder(  clientOrderIdToDelete );

        }

        private void DeleteOrdersHandler(object sender, RoutedEventArgs e)
        {
            var clientOrderIdToDelete = ClOrderIdToDelete.Text;

            var ids = clientOrderIdToDelete.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var orders = ids.Select(id => new BitMexLimitOrder { OrderId = id }).ToList();

            var operationExecutor = new BitMexOperationExecutor(_logger);
            operationExecutor.DeleteOrders(ids.ToList());
        }
    }
}
