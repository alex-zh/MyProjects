using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common.Classes.Analizers;
using Common.Classes.General;
using Common.Classes.StrategyTester;
using Visualizer.VisualControls.Classes;

namespace Robot.TesterApplication.Windows
{
    public enum MaximumExcursionTypes
    {
        Adverse,
        Favorable
    }

    public partial class MeWindow
    {
        public MeWindow()
        {
            InitializeComponent();
        }

        public MaximumExcursionTypes ExcursionType
        {
            get; 
            set;
        }

        public void Init(List<Deal> deals, List<Candle> candles)
        {
            BivariateChart.Init(GetBivariatePoints(deals, candles).ToList());
        }

        private IEnumerable<BivariateChartPoint> GetBivariatePoints(List<Deal> deals, List<Candle> candles)
        {
            var maximumExcursionAnalizer = new MaximumExcursionAnalizer(deals, candles);

            var points = (ExcursionType== MaximumExcursionTypes.Favorable) ? maximumExcursionAnalizer.GetMfe(): 
                                                                             maximumExcursionAnalizer.GetMae();

            foreach (var p in points)
            {
                SolidColorBrush brush = p.RealizedValue> 0 ? Brushes.Green : Brushes.Red;
                yield return new BivariateChartPoint() { FirstValue = p.MaximumValue, SecondValue = p.RealizedValue, Brush = brush};
            }
        }
    }
}
