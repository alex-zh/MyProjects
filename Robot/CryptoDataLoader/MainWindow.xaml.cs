using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Common;
using Common.Classes.General;
using Connector.Common;

namespace CryptoDataLoader
{
    public partial class MainWindow : Window
    {        
        private readonly string _pathPattern = "C:\\CryptoData\\{0}\\{1}";


        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            datePicker.SelectedDate = DateTime.Now;
            comboboxSymbol.SelectionChanged += ComboboxSymbol_SelectionChanged;
            comboboxConnector.SelectionChanged += ComboboxConnector_SelectionChanged;            
            LoadAndBindConnectors();
        }

        private void LoadAndBindConnectors()
        {
            comboboxConnector.ItemsSource = ConnectorsExtensions.ToList()
                .Select(x => new ComboBoxItem { Content = ((Connectors)x.Key).ToText(), DataContext = x.Key });

            comboboxConnector.SelectedIndex = 0;
        }

        private void ComboboxConnector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            datePicker.IsEnabled = SelectedConnector != Connectors.BitFinex;
                
            LoadAndBindSymbols();
        }

        private void LoadAndBindSymbols()
        {
            var connector = ConnectorsFactory.Get(SelectedConnector);

            comboboxSymbol.ItemsSource = connector.GetSymbols()
              .Select(x => new ComboBoxItem { Content = x, DataContext = x });


            comboboxSymbol.SelectedIndex = 0;
        }

        private void ComboboxSymbol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textboxPath.Text = String.Format(_pathPattern, SelectedConnector.ToText(), SelectedSymbol);
        }

        public Connectors SelectedConnector
        {
            get
            {
                return Dispatcher.Invoke(() => (Connectors)((ComboBoxItem)(comboboxConnector.SelectedItem)).DataContext);
            }
        }

        public string SelectedSymbol
        {
            get
            {
                if (comboboxSymbol.SelectedItem != null)
                    return Dispatcher.Invoke(() => (string)((ComboBoxItem)(comboboxSymbol.SelectedItem)).DataContext);

                return String.Empty;
            }
        }


        private async void LoadCandles_OnClick_OnClick(object sender, RoutedEventArgs e)
        {
            var connector = ConnectorsFactory.Get(SelectedConnector);

            LoadCandles.IsEnabled = false;
            var path = textboxPath.Text;
            var symbol = SelectedSymbol;
            var timeFrame = textBoxTimeFrame.Text;
            var limit = int.Parse(textBoxLimit.Text);
            var priceCoefficient = int.Parse((textBoxPriceCorrection.Text));

            var endDate = GetEndDateTime();

            var fileName = GenerateFileName(symbol, timeFrame, limit);
            var filePath = Path.Combine(path, fileName);

            await Task.Run(() =>
            {
                var candles = connector.GetCandles(symbol, timeFrame, limit, endDate);
                if (priceCoefficient != 1)
                {
                    candles = CorrectPrices(priceCoefficient, candles);
                }
                new CandlesWriter(filePath).Write(candles);

            });

            LoadCandles.IsEnabled = true;
        }

        private IEnumerable<Candle> CorrectPrices(int priceCoefficient, IEnumerable<Candle> candles)
        {
            foreach (var candle in candles)
            {
                candle.Open *= priceCoefficient;
                candle.Close *= priceCoefficient;
                candle.High *= priceCoefficient;
                candle.Low *= priceCoefficient;
            }

            return candles;
        }

        private DateTime GetEndDateTime()
        {
            var endDate = datePicker.SelectedDate ?? DateTime.Now;
            endDate = endDate.AddTicks(DateTime.Now.TimeOfDay.Ticks);
            return endDate;
        }

        private string GenerateFileName(string symbol, string timeFrame, int limit)
        {
            var endDate = GetEndDateTime();
            return $"{endDate.ToString("yyyy-MM-dd")}_{symbol}_{timeFrame}_{limit}.txt";
        }
    }
}
