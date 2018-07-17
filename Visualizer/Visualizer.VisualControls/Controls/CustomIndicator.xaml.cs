using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using Common;
using Common.Caclulators;
using Common.Classes;
using Common.Classes.General;
using Visualizer.VisualControls.Classes;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Visualizer.VisualControls.Controls
{
    public partial class CustomIndicator : ICustomIndicatorChart
    {
        private List<Candle> _brentCandles;
        public int CorrelationLookupWindow = 10;
        private List<Candle> _allCandles;

        public CustomIndicator()
        {
            InitializeComponent();            
        }

        private void LoadBrentCandles()
        {            
            var dialog = new OpenFileDialog { Multiselect = false, Filter = "Text|*.txt|All|*.*" };
            // dialog.InitialDirectory = @"C:\Patterns\Buy";
            dialog.InitialDirectory = @"C:\MyProjects\Robot\TestData\MoexSi\1 min";

            if (dialog.ShowDialog().HasValue && dialog.ShowDialog().Value)
            {
                _brentCandles = new CandlesLoader().FromFile(dialog.FileName).ToList();
            }
        }

        public void Init(List<Candle> allCandles)
        {
            _allCandles = allCandles;
        }

        public void DrawRange(List<Candle> candles)
        {            
            var timeGraphLine = GetTimeGraphLine(candles);
            TimeGraphChart.Init(new List<TimeGraphLine> {timeGraphLine});
            TimeGraphChart.Draw();
        }

        private TimeGraphLine GetTimeGraphLine(List<Candle> viewPortCandles)
        {
            var values = new List<DatePrice>();

            foreach (var candle in viewPortCandles)
            {
                var index = _allCandles.GetIndexByDate(candle.Date);
                if (index < 20)
                {
                    values.Add(new DatePrice { Date = candle.Date, Value = 0 });
                    continue;
                }

                var previousCandles = _allCandles.TakeRange(Math.Max(0, index - 20), index).ToList();
                var calculator = new TrendnessCalculator(previousCandles, x => x.Get(PriceTypes.Close));
                var value = calculator.Calculate();

                values.Add(new DatePrice { Date = candle.Date, Value = value });
            }
          
            var points = values.Select(x => new PlainChartPoint() {DateTime = x.Date, Value = x.Value}).ToList();

            return new TimeGraphLine(points, AxisSides.Right, Colors.Red, "Trendeness");
        }
    }

    public interface ICustomIndicatorChart
    {
        void Init(List<Candle> allCandles);
        void DrawRange(List<Candle> candles);
    }
}
