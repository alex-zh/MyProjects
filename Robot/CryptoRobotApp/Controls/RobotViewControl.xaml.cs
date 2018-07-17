using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Common.Classes;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Classes.StrategyTester;
using Robot.Core;
using Robot.Core.Classes;
using Robot.Core.Keepers;
using Robot.Core.Providers;
using Robot.DataLayer.Repositories;

namespace Robot.TradeApplication.Controls
{    
    public partial class RobotViewControl
    {
        private readonly AutoResetEvent _stateUpdater = new AutoResetEvent(false);

        private const string ConnectedString = "Connected";
        private const string NotConnectedString = "Not Connected";

        private ILogger _logger = LoggerKeeper.Get();
        private readonly IRobotExecutor _robotExecutor = new RobotExecutor(LoggerKeeper.Get());
        private readonly RobotStateRepository _robotStateRepository = new RobotStateRepository();
        private readonly RobotRepository _robotRepository = new RobotRepository();
        private readonly RobotTradeRepository _tradesRepository = new RobotTradeRepository();

        private readonly IQuikDataProvider _dataProvider;
        private RobotNames _robotName;
        private Securities _security;
        private RobotState _robotState;
        private RobotInfo _robotInfo;
        private RobotStateSynchronizer _robotStateSynchronizer;
        private RobotExecutionOptions _executionOptions;

        public RobotViewControl()
        {
            InitializeComponent();
            
            BindRobotModes();
            BindTradeStatistics(new TradeDataStatistics());

            _dataProvider = QuikDataProviderKeeper.GetOperationalOne();
            _dataProvider.DataChanged += DataProviderOnDataChanged;

            Task.Factory.StartNew(UpdateDdeStateAndCompletedOrders, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent).LogExceptions(_logger);
        }        

        public void Init(RobotNames robotName, Securities security, RobotExecutionOptions executionOptions)
        {
            _robotName = robotName;
            _security = security;
            _executionOptions = executionOptions;

            _robotInfo = _robotRepository.GetByName(_robotName);

            cbRobotMode.SelectedValue = _robotInfo.Mode;

            if (_robotInfo.Status == RobotOperationalStatuses.Started)
            {
                tbRobotInStartedStateWarning.Visibility = Visibility.Visible;
            }

            _robotState = _robotStateRepository.GetById(_robotInfo.RobotId);
            _robotState.StateChanged += _robotState_StateChanged;

            _robotStateSynchronizer = new RobotStateSynchronizer(_logger);
            _robotStateSynchronizer.OrderExecuted += RobotOrdersSynchronizerOrderExecuted;

            BindRobotState();                        
        }        

        public RobotModes RobotMode
        {
            get
            {
                return ((KeyValuePair<string, RobotModes>)cbRobotMode.SelectedItem).Value;                
            }
        }      
        
        private void BtnStartStop_OnClick(object sender, RoutedEventArgs e)
        {
            if (_robotExecutor.IsExecutionStarted == false)
            {
                if (MessageBox.Show("You are going to start execution in '" + RobotMode + "' .", "",
                    MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    RobotStart();
                }                
            }
            else
            {
                RobotStop();
            }
        }

        private void RobotStart()
        {
            var robotMode = RobotMode;

            _logger = LoggerKeeper.GetFor(_robotName);

            var robot = RobotFactory.CreateTrendFollower(RobotMode, _robotName, _security, _robotState, _robotStateSynchronizer, _logger, _executionOptions);

            Task.Factory.StartNew(() => _robotExecutor.Start(robot), TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent).LogExceptions(_logger);

            btnStartStop.Content = "Stop";
            UpdateRobotInfo(robotMode, RobotOperationalStatuses.Started);                       
            tbRobotInStartedStateWarning.Visibility = Visibility.Hidden;
        }
     
        private void RobotStop()
        {
            _robotExecutor.Stop();

            btnStartStop.Content = "Start";
            UpdateRobotInfo(RobotOperationalStatuses.NotStarted);
        }

        private void BindRobotModes()
        {            
            foreach (RobotModes robotModeValue in Enum.GetValues(typeof(RobotModes)))
            {
                cbRobotMode.Items.Add(new KeyValuePair<string, RobotModes>(Enum.GetName(typeof(RobotModes), robotModeValue), robotModeValue));
                cbRobotMode.SelectedValuePath = "Value";
                cbRobotMode.DisplayMemberPath = "Key";
            }

            cbRobotMode.SelectedIndex = 0;
        }

        void _robotState_StateChanged(object sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                _robotStateRepository.Update(_robotInfo.RobotId, _robotState);
                BindRobotState();
            });            
        }

        void RobotOrdersSynchronizerOrderExecuted(object sender, LimitedOrder e)
        {
            Task.Factory.StartNew(() => new CompletedTradesUpdator(_robotName,
                                                                   _logger,
                                                                   () => _dataProvider.GetTrades()).Update(e),
                                  TaskCreationOptions.AttachedToParent).LogExceptions(_logger);
        }        

        private void BindRobotState()
        {
            if (_robotState != null)
            {
                tbSize.Text = _robotState.Size.ToString(CultureInfo.InvariantCulture);

                tbDirection.Text = _robotState.Size != 0 ? _robotState.Direction.ToString() : "None";

                RobotActiveOrdersGrid.ItemsSource = _robotState.Orders.ToList();
            }
        }

        private void UpdateRobotInfo(RobotOperationalStatuses status)
        {
            _robotInfo.Status = status;
            _robotRepository.Update(_robotInfo);
        }

        private void UpdateRobotInfo(RobotModes mode, RobotOperationalStatuses status)
        {
            _robotInfo.Mode = mode;
            _robotInfo.Status = status;

            _robotRepository.Update(_robotInfo);
        }

        private void DataProviderOnDataChanged(object sender, EventArgs eventArgs)
        {
            _stateUpdater.Set();            
        }       

        private void UpdateDdeStateAndCompletedOrders()
        {
            try
            {
                while (true)
                {
                    _stateUpdater.WaitOne(2000);
                    
                    UpdateDdeState();
                    UpdateCompletedOrders();
                    UpdateQuikActiveOrders();
                    UpdateLastIterationTime();
                }            
            }
            catch (TaskCanceledException) { }            
        }

        private void UpdateLastIterationTime()
        {
            Dispatcher.Invoke(() =>
            {
                var lastIterationTime = _robotExecutor.GetLastIterationTime();
                tbLastIterationTime.Text = lastIterationTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

                if ((DateTime.Now - lastIterationTime).TotalSeconds > 1.5 * RobotSettings.IterationOffset)
                {
                    tbLastIterationTime.Foreground = Brushes.Red;
                }
                else
                {
                    tbLastIterationTime.Foreground = Brushes.Green;
                }

            });
        }

        private void UpdateDdeState()
        {
            Dispatcher.Invoke(() =>
            {
                tbCurrentTime.Text = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

                var lastCandle = _dataProvider.GetLastCandle(_security);

                if (lastCandle != null && IsDdeConnected())
                {
                    tbLastCandleTime.Text = lastCandle.Date.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

                    tbRobotDdeStatus.Text = ConnectedString;
                    tbRobotDdeStatus.Foreground = Brushes.Green;                    
                }
                else
                {
                    tbRobotDdeStatus.Text = NotConnectedString;
                    tbRobotDdeStatus.Foreground = Brushes.DarkBlue;
                }
            });
        }

        private bool IsDdeConnected()
        {
            var lastCandle = _dataProvider.GetLastCandle(_security);

            return lastCandle != null && 
                   Math.Abs((lastCandle.Date - DateTime.Now).TotalSeconds) < 60 + RobotSettings.QuikDdeCandlesUpdateOffset;
        }

        private void UpdateCompletedOrders()
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    var trades = _tradesRepository.GetAllFor(_robotName, DateTime.Now).ToList();

                    CompletedTradesGrid.ItemsSource = trades;

                    if (trades.Any())
                    {
                        var dataAnalizer = new TradeDataAnalyzer(trades, _executionOptions.MaxPositionSize, trades.First().Price * _executionOptions.MaxPositionSize);

                        BindTradeStatistics(dataAnalizer.GetStatistics());
                    }
                }
                catch (Exception ex)
                {
                    /* Update view is not critical in context of robot execution aborting */
                    _logger.Log(ex.ToString(), LogMessageTypes.Error);
                }
            });
        }

        private void UpdateQuikActiveOrders()
        {
            Dispatcher.Invoke(() =>
            {
                var orders = _dataProvider.GetLimitedOrders().Where(x=>x.Status == OrderStatuses.Active).ToList();

                QuikActiveOrdersGrid.ItemsSource = orders;
                
            });
        }        


        private void BindTradeStatistics(TradeDataStatistics tradeStatistics)
        {
            var statisticsToGridConverter = new TradeDataStatisticsToGridConverter(tradeStatistics);

            ProfitLossGrid.Build(statisticsToGridConverter.ProfitLossDictionary,
                                 statisticsToGridConverter.FormatValue,
                                 statisticsToGridConverter.FormatElement,
                                 statisticsToGridConverter.FormatRowHeader);
            
        }
    }
}
