using System.Collections.Generic;
using System.Linq;
using Visualizer.VisualControls.Classes;

namespace Visualizer.VisualControls.Controls
{
    /// <summary>
    /// Prepares axis' lables to fit the canvas
    /// </summary>
    public class PlainChartLablesPreparer
    {
        private readonly List<PlainChartPoint> _points;
        private readonly double _canvasWidth;

        public PlainChartLablesPreparer(List<PlainChartPoint> points, double canvasWidth)
        {
            _points = points;
            _canvasWidth = canvasWidth;

            var arePointsOneMonth = points.Select(x => x.DateTime.Month).Distinct().Count() == 1;
            var arePointsInOneDay = points.Select(x => x.DateTime.Day).Distinct().Count() == 1;
        }

        private List<KeyValuePair<int, string>> Prepare()
        {
            return null;
            //ticks = SubsampleTicksToFitCanvas(ticks, CanvasCalculator.CanvasWidth, tick => 1.25 * MeasureString(FormatDate(_points[(int)tick].DateTime)).Width);
        }

    }
}