using System;
using Common.Classes;

namespace Visualizer.VisualControls.Controls.Classes
{
    public class HeatMapChartCalculator<TX, TY>
                     where TX : IComparable<TX>
                     where TY : IComparable<TY>
    {
        private readonly HeatMap<TX, TY> _heatMap;

        public HeatMapChartCalculator(HeatMap<TX, TY> heatMap)
        {            
            if (heatMap.Any() == false) return;

            _heatMap = heatMap;

            MinX = 0;
            MaxX = heatMap.MaxX;

            MinY = 0;
            MaxY = heatMap.MaxY;
        }

        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }

    }
}