using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Common.Classes.General;
using Common.Classes.StrategyTester;
using Robot.TesterApplication.Classes;

namespace Robot.TesterApplication.Controls
{    
    public partial class DealsDistributionViewer
    {
        private DealsDistributionChartCalculator _chartCalculator;
        private List<Deal> _deals;        
        private const int BarOffset = 3;
        private const int BarHalfWidth = 3;
        private Deal _clickedDeal = null;
        private Func<Deal, bool> _dealsFilter = deal => true;
        private List<Candle> _candles;

        public DealsDistributionViewer()
        {
            InitializeComponent();
            Canvas.PreviewMouseDown += CanvasChartOnPreviewMouseDown;
        }        

        public DealsDistributionChartCalculator ChartCalculator
        {
            get
            {
                if (_chartCalculator == null)
                {
                    var count = _deals.Count;
                    //var maxValue = _deals.Max(x => x.GetProfitInPercent());
                    //var minValue = _deals.Min(x => x.GetProfitInPercent());
                    var maxValue = _deals.Max(x => x.GetProfitInMoney());
                    var minValue = _deals.Min(x => x.GetProfitInMoney());
                    _chartCalculator = new DealsDistributionChartCalculator(Canvas.ActualHeight, Canvas.ActualWidth, BarHalfWidth * 2, BarOffset, 0, count, minValue, maxValue);        
                }

                return _chartCalculator;
            }
        }
        public void Init(IEnumerable<Deal> deals, List<Candle> candles)
        {
            _chartCalculator = null;

            _deals = deals.ToList();
            _candles = candles;
            
            Draw();
        }

        public void Draw()
        {
            if (_deals.Any() == false) return;

            var filteredDeals = _deals.Where(_dealsFilter).ToList();

            Canvas.Children.Clear();

            Canvas.Width = Math.Max(CanvawScrollViewer.ActualWidth, ChartCalculator.GetCanvasWidth(filteredDeals.Count));

            DrawHorizontalLine();

            for (int i = 0; i < filteredDeals.Count; i++)
            {
                DrawDeal(filteredDeals[i], i);
            }
        }        

        private void DrawHorizontalLine()
        {
            DrawLine(ChartCalculator.GetX(0), ChartCalculator.GetY(0), ChartCalculator.GetX(_deals.Count), ChartCalculator.GetY(0), Brushes.Gray, 2);
        }
        
        private void DrawDeal(Deal deal, int dealIndex)
        {
            //var dealProfit = deal.GetProfitInPercent();
            var dealProfit = deal.GetProfitInMoney();

            var x = _chartCalculator.GetX(dealIndex);

            var openValue = _chartCalculator.GetY(0);
            var closeValue = _chartCalculator.GetY(dealProfit);

            var ptOpen1 = new Point(x - BarHalfWidth, openValue);
            var ptOpen2 = new Point(x + BarHalfWidth, openValue);

            var ptClose1 = new Point(x + BarHalfWidth, closeValue);
            var ptClose2 = new Point(x - BarHalfWidth, closeValue);

            var brush = dealProfit > 0 ? Brushes.Green : Brushes.Red;
            var plg = new Polygon { Stroke = brush, StrokeThickness = 2 };

            Brush fillColor = Brushes.White;            

            plg.Fill = fillColor;

            plg.Points.Add(ptOpen1);
            plg.Points.Add(ptOpen2);
            plg.Points.Add(ptClose1);
            plg.Points.Add(ptClose2);

            plg.DataContext = deal;

            Canvas.Children.Add(plg);
        }

        private void DrawLine(double x1, double y1, double x2, double y2, Brush lineColor, double lineThickness)
        {
            var line = new Line
            {
                Stroke = lineColor,
                StrokeThickness = lineThickness,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };

            Canvas.Children.Add(line);
        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _chartCalculator = null;

            if (_deals != null && _deals.Any())
            {
                Draw();
            }
        }

        private void CanvasChartOnPreviewMouseDown(object sender, MouseButtonEventArgs args)
        {
            if (args.ClickCount == 2)
            {
                _clickedDeal = null;

                Point pt = args.GetPosition((UIElement)sender);

                VisualTreeHelper.HitTest(Canvas, null, DealClickedTestResult, new PointHitTestParameters(pt));

                if (_clickedDeal != null)
                {

                    dealInfoControl.BindData(_clickedDeal, _candles.GetIndexByDate(_clickedDeal.TradeItems.First().DateTime));
                }
            }
        }

        private HitTestResultBehavior DealClickedTestResult(HitTestResult result)
        {
            var hit = result.VisualHit as Polygon;

            if (hit != null)
            {
                if (((Polygon)(result.VisualHit)).DataContext is Deal)
                {
                    _clickedDeal = (Deal)hit.DataContext;
                    return HitTestResultBehavior.Stop;
                }
            }

            return HitTestResultBehavior.Continue;
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (_deals == null || _deals.Any() == false) return;

            if (btnShowOnlyComplex.IsChecked.HasValue && btnShowOnlyComplex.IsChecked.Value)
            {
                _dealsFilter = deal => deal.TradeItems.Count > 2;
            }
            else
            {
                _dealsFilter = deal => true;
            }

            Draw();
        }
    }
}
