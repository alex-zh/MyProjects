using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common.Classes.Robot;
using Common.Classes.StrategyTester;
using Common.Robots.Common;
using Common.Robots.TrendFollower;
using Robot.Core.Classes;

namespace Robot.TradeApplication.Controls
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

        public void BindTradeStatistics(TradeDataStatistics tradeStatistics, int maxPositionSize)
        {
            var statisticsToGridConverter = new TradeDataStatisticsToGridConverter(tradeStatistics, maxPositionSize);

            ProfitLossGrid.Build(statisticsToGridConverter.ProfitLossDictionary,
                                 statisticsToGridConverter.FormatValue,
                                 statisticsToGridConverter.FormatElement,
                                 statisticsToGridConverter.FormatRowHeader);
        }

        public void Init(RobotNames robotName, Securities security)
        {
            _robotController.Init(robotName, security);
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

                    tbDirection.Text = robotState.Size != 0 ? robotState.Direction.ToString() : "None";

                    RobotActiveOrdersGrid.ItemsSource = robotState.Orders.ToList();
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
        public void BindConnectionState(DateTime lastCandleTime, bool isConnected)
        {
            Dispatcher.Invoke(() =>
            {
                tbCurrentTime.Text = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                tbLastCandleTime.Text = lastCandleTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

                if (isConnected)
                {                
                    tbRobotDdeStatus.Text = ConnectedString;
                    tbRobotDdeStatus.Foreground = Brushes.Green;
                }
                else
                {
                    tbRobotDdeStatus.Text = NotConnectedString;
                    tbRobotDdeStatus.Foreground = Brushes.DarkRed;
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

        public void BindActiveOrders(List<LimitOrder> orders)
        {
            Dispatcher.Invoke(() =>
            {
                ActiveOrdersGrid.ItemsSource = orders;

            });
        }
        
    }
}
