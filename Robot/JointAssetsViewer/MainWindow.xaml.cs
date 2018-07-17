using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Navigation;
using Common.Classes;
using Common.Classes.General;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Regression;
using JointAssetsViewer.Controls;
using Visualizer.VisualControls.Classes;

namespace JointAssetsViewer
{    
    public partial class MainWindow
    {
        private QuikDataProvider _dataProvider = new QuikDataProvider();

        private List<BivariateChartPoint> _list = new List<BivariateChartPoint>();
        private Task _updateViewTask;
        private const int UpdateTimeout = 3000; // 3 seconds
      

        public MainWindow()
        {
            InitializeComponent();

            BivariateChart.UseZeroY = false;
            
            Loaded += MainWindow_Loaded;

            SummaryControl.QuikDataProvider = _dataProvider;
            BivariateChart.SizeChanged += BivariateChart_SizeChanged;
            BindAssetComboBoxes();

            _list = new RandomChartPointGenerator().Generate(100, x => new BrSiPair(
                                                             new Candle() { Date = DateTime.Now.AddMinutes(x) },
                                                             new Candle() { Date = DateTime.Now.AddMinutes(x) }));            
        }

        private void BindAssetComboBoxes()
        {
            cbFirstAsset.ItemsSource = GetComboBoxItems(); 
            cbSecondAsset.ItemsSource = GetComboBoxItems(); 

            cbFirstAsset.SelectedIndex = 2;
            cbSecondAsset.SelectedIndex = 0;
        }

        public Securities FirstSecurity
        {
            get
            {
                return Dispatcher.Invoke(() => (Securities)((ComboBoxItem)(cbFirstAsset.SelectedItem)).DataContext);
            }
        }

        public Securities SecondSecurity
        {
            get
            {
               return  Dispatcher.Invoke(() => (Securities)((ComboBoxItem)(cbSecondAsset.SelectedItem)).DataContext);
            }
        }

        private IEnumerable<ComboBoxItem> GetComboBoxItems()
        {
            return SecuritiesExtensions.ToList().Select(x => new ComboBoxItem { Content = ((Securities)x.Key).ToText(), DataContext = x.Key }).ToList();
        }

        public bool UseLogarithmicScale
        {
            get
            {
                return false; //return UseLocarithmicScale.IsChecked != null && UseLocarithmicScale.IsChecked.Value; 
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {            
            SetupWindowSize();

            Task.Factory.StartNew(DoUpdate, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent).LogExceptions(new LoggerToFile());         
        }

        private void DoUpdate()
        {
            while (true)
            {             
                UpdatePoints();                

                Thread.Sleep(UpdateTimeout);
            }
        }

        private void UpdatePoints()
        {
            try
            {
                _list = GetPoints();

                Dispatcher.InvokeAsync(Draw);      
            }
            catch (Exception ex)
            {                
                new LoggerToFile().Log(ex.ToString(), LogMessageTypes.Error);
            }            
        }

        private List<BivariateChartPoint> GetPoints()
        {
            return new JointAssetsBivariateChartPointsProvider(_dataProvider, UseLogarithmicScale).Get(FirstSecurity, 
                                                                                                       SecondSecurity);          
        }

        void BivariateChart_SizeChanged(object sender, EventArgs e)
        {
            if (_list != null)
            {
                Draw();
            }
        }

        private void Draw()
        {
            if (_list.Any() == false)
            {
                return;
            }

            BivariateChart.Init(_list);

            DrawStatisticsAxes();
            DrawRegressionLines();
            DrawPoints();
        }     

        private void DrawStatisticsAxes()
        {
            var xValues = _list.Select(x => x.FirstValue);
            var xStatistics = new StatisticsCalculator(xValues);

            var yValues = _list.Select(y => y.SecondValue);
            var yStatistics = new StatisticsCalculator(yValues);

            BivariateChart.AddHorizontalLine(yStatistics.Mean() - yStatistics.Sigma(), 1.5, Colors.CadetBlue,
                new double[] { 1, 3 });
            BivariateChart.AddHorizontalLine(yStatistics.Mean() + yStatistics.Sigma(), 1.5, Colors.CadetBlue,
                new double[] { 1, 3 });

            BivariateChart.AddVerticalLine(xStatistics.Mean() - xStatistics.Sigma(), 1.5, Colors.CadetBlue,
                new double[] { 1, 3 });
            BivariateChart.AddVerticalLine(xStatistics.Mean() + xStatistics.Sigma(), 1.5, Colors.CadetBlue,
                new double[] { 1, 3 });
        }

        private void DrawRegressionLines()
        {            
            AddRegressionLine(_list, Colors.DarkRed, allPointsBivariateChartInfo);

            for (int i = _list.Count - 20; i < _list.Count; i++)
            {
                _list[i].Brush = Brushes.LightBlue;
            }

            var last20Candles = _list.OrderByDescending(x => ((BrSiPair) x.Data).SiCandle.Date).Take(20)
                                     .OrderBy(x => ((BrSiPair)x.Data).SiCandle.Date).ToList();

            AddRegressionLine(last20Candles, Colors.Blue, last20PointsBivariateChartInfo);            
        }

        private void AddRegressionLine(List<BivariateChartPoint> list, Color color, BivariateChartInfo chartInfo)
        {            
            var regressionBuilder = new LinearRegressionBuilder(list.Select(p => p.FirstValue).ToArray(), 
                                                                list.Select(p => p.SecondValue).ToArray());
            var regression = regressionBuilder.Build();

            var minX = list.Min(x => x.FirstValue);
            var maxX = list.Max(x => x.FirstValue);
            var minY = regression.a0 + regression.a1 * minX;
            var maxY = regression.a0 + regression.a1 * maxX;

            BivariateChart.AddLine(minX, minY, maxX, maxY, 1.5, color);

            chartInfo.Init(regression, list);            
        }

        private void DrawPoints()
        {            
            BivariateChart.Draw();
        }

        private void SetupWindowSize()
        {
            var workingRectangle = Screen.PrimaryScreen.WorkingArea;

            Width = workingRectangle.Width * 0.85;
            Height = workingRectangle.Height * 0.85;
        }

        private void UseLocarithmicScale_OnClick(object sender, RoutedEventArgs e)
        {
            UpdatePoints();
        }
    }
}
