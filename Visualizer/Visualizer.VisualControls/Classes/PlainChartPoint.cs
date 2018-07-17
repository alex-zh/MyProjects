using System;
using Visualizer.VisualControls.Controls;
using Visualizer.VisualControls.Controls.Classes;
using Visualizer.VisualControls.Controls.Classess;

namespace Visualizer.VisualControls.Classes
{
    public class PlainChartPoint
    {
        public int Index { get; set; }

        public DateTime DateTime { get; set; }

        public double Value { get; set; }

        public string ToString(ChartTimeRanges range)
        {
            var date = "";

            switch (range)
            {
                case ChartTimeRanges.Day:
                    date = DateTime.ToString("dd/MM");
                    break;
                case ChartTimeRanges.Minute:
                    date = DateTime.ToString("HH/mm");
                    break;
            }

            return String.Format("DateTime: {0}; Value: {1}", date, Value);
        }
    }
}