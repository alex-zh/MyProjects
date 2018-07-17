using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Common;
using Common.Classes;
using Common.Classes.General;
using Common.Classes.Robot;
using Common.Classes.StrategyTester;
using Common.Robots.Common;
using Common.StrategiesOptions;
using Robot.Core;
using Robot.Core.MarketDataProviders;
using Robot.Core.OperationExecutors;
using Robot.Strategies.Agents;
using Robot.TesterApplication.Classes;
using Robot.TesterApplication.Windows;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Robot.TesterApplication
{
    public partial class MainWindow
    {
        private IExecutableRobot _robot;
        private TesterExecutor _robotExecutor;
        private TestMarketDataProvider _marketDataProvider;
        private MarketOperationTestExecutor _marketOperationExecutor = new MarketOperationTestExecutor();
        private List<Candle> _allCandles = new List<Candle>();
        private TradeDataStatistics _lastResult = new TradeDataStatistics();
        private IStrategyOptions _strategyOptions;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            KeyDown += OnPreviewKeyDown;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            BindStrategyConfigCombobox();
        }

        private void BindStrategyConfigCombobox()
        {
            comboboxStrategyConfig.ItemsSource = StrategyConfigsExtensions.ToList()
                .Select(x => new ComboBoxItem { Content = ((StrategyConfigs)x.Key).ToText(), DataContext = x.Key });

            comboboxStrategyConfig.SelectedIndex = 0;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.G)
            {
                if (tabControl.SelectedIndex == 1)
                {
                    var goToForm = new StockChartGoToForm { Owner = this };
                    var result = goToForm.ShowDialog();
                    if (result.HasValue && result.Value)
                    {
                        var index = goToForm.SelectedIndex;
                        tradesViewer.GoTo(index);
                    }
                }
            }
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show(e.ToString());
            e.SetObserved();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        public StrategyConfigs SelectedStrategyConfig
        {
            get
            {
                return Dispatcher.Invoke(() => (StrategyConfigs)((ComboBoxItem)(comboboxStrategyConfig.SelectedItem)).DataContext);
            }
        }
        
        private void InitState()
        {
            _robotExecutor = null;
            
            var robotState = new RobotState(0, new List<LimitOrder>());
            _strategyOptions = StrategyOptionsFactory.GetByName(SelectedStrategyConfig, StrategyConfigTypes.Test);

            _marketDataProvider = new TestMarketDataProvider();
            _marketOperationExecutor = new MarketOperationTestExecutor();
            
            _robotExecutor = new TesterExecutor(
                robotState,
                _allCandles,
                _strategyOptions,
                _marketDataProvider,
                _marketOperationExecutor,
                1);

            _robot = new NetThrowerRobot("testRobot", 
                                         "testSymbol", 
                                         robotState, 
                                         (NetThrowerOptions)_strategyOptions,
                                        _marketDataProvider,
                                        _marketOperationExecutor, 
                                        new TestStrategyLogger());
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_allCandles.Any() == false)
            {
                MessageBox.Show("Candles are not loaded");
                return;
            }

            InitState();

            if (_robotExecutor.IsExecutionStarted == false)
            {
                tbStartButton.Text = "Stop";

                var task = Task.Run(() => _robotExecutor.Start(_robot));

                task.ContinueWith(ShowTestResults, TaskScheduler.FromCurrentSynchronizationContext()).LogExceptions(new TestStrategyLogger());
            }
            else
            {
                _robotExecutor.Stop();

                tbStartButton.Text = "Start";
            }
        }


        private void ShowTestResults(Task previous)
        {
            Dispatcher.Invoke(() =>
            {
                _robotExecutor.Stop();
                tbStartButton.Text = "Start";

                var rawData = _robotExecutor.GetTestData();

                ShowTrades(rawData.ToList<ITradeItem>(), false);
            });
        }

        private void ShowTrades(List<ITradeItem> rawData, bool useBinarySignals)
        {
            var dataAnalizer = new TradeDataAnalyzer(rawData, _strategyOptions.MaxPositionSize,
                double.Parse("10000"));

            _lastResult = dataAnalizer.GetStatistics();

            StrategyStatisticsControl.BindData(_lastResult, _strategyOptions.MaxPositionSize);

            dealsDistributionViewer.Init(dataAnalizer.Deals, _allCandles);

            tradesViewer.ShowTrades(_allCandles, TradeDataConverter.ConvertToTradeSignals(_allCandles, rawData, useBinarySignals));

            MiscellaneousInfo.BindData(_allCandles);
        }

        private void LoadMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            _robotExecutor = null;

            var dialog = new OpenFileDialog { Multiselect = false, Filter = "Text|*.txt|All|*.*" };
            dialog.InitialDirectory = @"C:\Users\aaz\Documents\My Projects\TradeIdeasTester\Visualizer\Visualizer\AppData";

            if (dialog.ShowDialog() == true)
            {
                _allCandles = new CandlesLoader().FromFile(dialog.FileName).ToList();

                tradesViewer.ShowCandles(_allCandles);
            }
        }

        private void LoadTradesMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            _robotExecutor = null;

            var dialog = new OpenFileDialog { Multiselect = false, Filter = "Text|*.csv|All|*.*" };
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (dialog.ShowDialog() == true)
            {
                var trades = new TradesLoader("RIZ5").FromFile(dialog.FileName).ToList();
                ShowTrades(trades, true);
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var workingRectangle = Screen.PrimaryScreen.WorkingArea;

            Width = workingRectangle.Width * 0.85;
            Height = workingRectangle.Height * 0.85;
        }

        private void BtnExportDialog_OnClick(object sender, RoutedEventArgs e)
        {
            var exportResultWindow = new ExportResultWindow { Owner = this };
            exportResultWindow.BindData(_lastResult, _strategyOptions);

            exportResultWindow.ShowDialog();
        }

        private void ButtonMfe_OnClick(object sender, RoutedEventArgs e)
        {
            var meWindow = new MeWindow { Owner = this, ExcursionType = MaximumExcursionTypes.Favorable };
            meWindow.Init(_lastResult.Deals, _allCandles);
            meWindow.Show();
        }

        private void ButtonMae_OnClick(object sender, RoutedEventArgs e)
        {
            var meWindow = new MeWindow { Owner = this, ExcursionType = MaximumExcursionTypes.Adverse };
            meWindow.Init(_lastResult.Deals, _allCandles);
            meWindow.Show();
        }
    }

}
