using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Common.Classes.StrategyTester;
using Common.Robots.Common;
using Common.StrategiesOptions;
using Robot.Core.Classes;
using Robot.Strategies.Agents;

namespace BitMex.TradeApplication.Controls
{
    public partial class RobotViewControl
    {
        private readonly RobotController _robotController;
        private const string ConnectedString = "Connected";
        private const string NotConnectedString = "Not Connected";


        public RobotViewControl()
        {
            InitializeComponent();

            _robotController = new RobotController(this);

            BindRobotModes();
            BindStrategyConfigCombobox();

            BindTradeStatistics(new TradeDataStatistics(), 0);
        }

        public void BindRobotModes()
        {
            foreach (RobotModes robotModeValue in Enum.GetValues(typeof(RobotModes)))
            {
                cbRobotMode.Items.Add(new KeyValuePair<string, RobotModes>(Enum.GetName(typeof(RobotModes), robotModeValue), robotModeValue));
                cbRobotMode.SelectedValuePath = "Value";
                cbRobotMode.DisplayMemberPath = "Key";
            }

            cbRobotMode.SelectedIndex = 0;
        }

        private void BindStrategyConfigCombobox()
        {
            comboboxStrategyConfig.ItemsSource = StrategyConfigsExtensions.ToList()
                .Select(x => new ComboBoxItem { Content = ((StrategyConfigs)x.Key).ToText(), DataContext = x.Key });

            comboboxStrategyConfig.SelectedIndex = 0;
        }


        public void BindTradeStatistics(TradeDataStatistics tradeStatistics, int maxPositionSize)
        {
            var statisticsToGridConverter = new TradeDataStatisticsToGridConverter(tradeStatistics, maxPositionSize);

            ProfitLossGrid.Build(statisticsToGridConverter.ProfitLossDictionary,
                                 statisticsToGridConverter.FormatValue,
                                 statisticsToGridConverter.FormatElement,
                                 statisticsToGridConverter.FormatRowHeader);
        }

        public void Init(string robotId, string symbol)
        {
            _robotController.Init(robotId, symbol);
        }

        public RobotModes RobotMode
        {
            get
            {
                return ((KeyValuePair<string, RobotModes>)cbRobotMode.SelectedItem).Value;
            }

            set
            {
                cbRobotMode.SelectedValue = value;
            }
        }

        public IStrategyOptions SelectedStrategyOptions
        {
            get
            {
                var selectedConfigName = Dispatcher.Invoke(() => (StrategyConfigs)((ComboBoxItem)(comboboxStrategyConfig.SelectedItem)).DataContext);
                return StrategyOptionsFactory.GetByName(selectedConfigName, StrategyConfigTypes.Work);
            }
        }

        public void ShowRobotIsStartedWarning(bool show = true)
        {
            tbRobotInStartedStateWarning.Visibility = show ? Visibility.Visible : Visibility.Hidden;
        }

        private void BtnStartStop_OnClick(object sender, RoutedEventArgs e)
        {
            if (_robotController.IsExecutionStarted == false)
            {
                if (MessageBox.Show("You are going to start execution in '" + RobotMode + "' .", "",
                    MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    _robotController.Start();
                }
            }
            else
            {
                _robotController.Stop();
            }
        }

        public void BindRobotState(RobotState robotState)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (robotState != null)
                {
                    tbSize.Text = robotState.Size.ToString(CultureInfo.InvariantCulture);

                    tbDirection.Text = Math.Abs(robotState.Size) < double.Epsilon ? "None" : (robotState.Size > 0 ? "Buy" : "Sell");

                    RobotActiveOrdersGrid.ItemsSource = robotState.GetOrders().Where(x => x.Status == OrderStatuses.Active);
                }
            });
        }

        public void BindLastIterationTime(DateTime lastIterationTime, bool isOffsetExceeded)
        {
            Dispatcher.Invoke(() =>
            {
                tbLastIterationTime.Text = lastIterationTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                tbLastIterationTime.Foreground = isOffsetExceeded ? Brushes.Red : Brushes.Green;
            });
        }
        public void BindConnectionState(DateTime serverLastResponseTime, bool isConnected)
        {
            Dispatcher.Invoke(() =>
            {
                tbCurrentTime.Text = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                tbServerLastResponseTime.Text = serverLastResponseTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

                if (isConnected)
                {
                    tbRobotDataChannelStatus.Text = ConnectedString;
                    tbRobotDataChannelStatus.Foreground = Brushes.Green;
                }
                else
                {
                    tbRobotDataChannelStatus.Text = NotConnectedString;
                    tbRobotDataChannelStatus.Foreground = Brushes.DarkRed;
                }
            });
        }

        public void SetStartLabel()
        {
            btnStartStop.Content = "Start";
        }

        public void SetStopLabel()
        {
            btnStartStop.Content = "Stop";
        }

        public void BindTrades(List<RobotTrade> trades)
        {
            Dispatcher.Invoke(() =>
            {
                CompletedTradesGrid.ItemsSource = trades;
            });
        }

        public void BindOrders(List<LimitOrder> orders)
        {
            Dispatcher.Invoke(() =>
            {
                ActiveOrdersGrid.ItemsSource = orders;

            });
        }

    }
}
