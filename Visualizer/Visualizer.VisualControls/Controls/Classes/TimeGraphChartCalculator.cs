using System;
using System.Collections.Generic;
using System.Linq;
using Visualizer.VisualControls.Classes;
using Visualizer.VisualControls.Controls.Classes;
using Visualizer.VisualControls.Controls.Classess;

namespace Visualizer.VisualControls.Controls
{
    public class TimeGraphChartCalculator
    {
        private readonly List<TimeGraphLine> _lines;
        private readonly ChartTimeRanges _timeRange;
        private readonly List<PlainChartPoint> _allPoints = new List<PlainChartPoint>();
        private DateTime _startDate;
        private DateTime _endDate;

        public TimeGraphChartCalculator(List<TimeGraphLine> lines, ChartTimeRanges timeRange = ChartTimeRanges.Day)
        {            
            if (lines.Any() == false) return;

            _lines = lines;
            _timeRange = timeRange;

            foreach (var line in _lines)
            {
                _allPoints.AddRange(line.Points);
            }

            if (_allPoints.Any() == false) return;

            _startDate = _allPoints.Select(x => x.DateTime).OrderBy(x => x).First();
            _endDate = _allPoints.Select(x => x.DateTime).OrderByDescending(x => x).First();

            MinX = 0;
            MaxX = CalculateNumberOfElements();

            MinY = _allPoints.Min(p => p.Value);
            MaxY = _allPoints.Max(p => p.Value);
        }

        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }

        public List<PlainChartPoint> AllPoints
        {
            get { return _allPoints; }
        }

        private int CalculateNumberOfElements()
        {
            if (_timeRange == ChartTimeRanges.Day)
            {
                return (int) (_endDate - _startDate).TotalDays + 1;
            }
            else if (_timeRange == ChartTimeRanges.Minute)
            {
                return _allPoints.Count;
            }            

            throw new NotSupportedException("Time range " + _timeRange + " is not supported.");

        }

        public int GetIndex(PlainChartPoint point)
        {
            if (point.DateTime < _startDate || point.DateTime > _endDate)
            {
                throw new  ArgumentOutOfRangeException("DateTime is out of range.");
            }

            if (_timeRange == ChartTimeRanges.Day)
            {
                return (int)(point.DateTime - _startDate).TotalDays;
            }
            else if (_timeRange == ChartTimeRanges.Minute)
            {
                return _allPoints.FindIndex(x => x.DateTime == point.DateTime);
            }
            
            throw new NotSupportedException("Time range " + _timeRange + " is not supported.");
        }

        public DateTime GetDateTimeFor(int tick)
        {
            if (_timeRange == ChartTimeRanges.Day)
            {
                return _startDate.AddDays(tick);                
            }
            else if (_timeRange == ChartTimeRanges.Minute)
            {
                return _startDate.AddMinutes(tick); ;
            }
            
            throw new NotSupportedException("Time range " + _timeRange + " is not supported.");

        }
    }
}