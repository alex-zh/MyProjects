using System;
using System.Windows.Controls;

namespace Visualizer.VisualControls.Controls.HeatMaps
{
    public partial class DayToHourHeatMapChart
    {
        public DayToHourHeatMapChart()
        {
            InitializeComponent();
            Grid.SizeChanged += SizeChangedHandler;
        }
        
        public override Canvas Canvas => mainCanvas;
        
        public override string FormatTitle(DateTime x)
        {
            return x.Date.ToString("dd.MM");
        }

        public override string FormatTitle(int y)
        {
            return y.ToString("00") + ":00";
        }

        private void Grid_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {

        }
    }
}
