using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Visualizer.Core;
using Visualizer.Core.Classes;
using Visualizer.VisualControls.Classes;

namespace Visualizer.VisualControls.Controls
{
    public partial class DateTimeAxis
    {
        private List<DateTime> _gridLinePlaces = new List<DateTime>();
        private List<DateTime> _gridLineGroupPlaces = new List<DateTime>();        
        private ChartCalculator _chartCalculator;

        public DateTimeAxis()
        {
            InitializeComponent();            
            grid.MouseLeftButtonDown += canvasAxis_MouseLeftButtonDown;
            grid.MouseLeftButtonUp += canvasAxis_MouseLeftButtonUp;
        }

        bool mouseDown = false;
        Point mouseDownPoint, mouseUpPoint;

        void canvasAxis_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {                        
            mouseUpPoint = e.GetPosition(canvasAxis);

            if (mouseDown && (mouseDownPoint.X - mouseUpPoint.X) >= 40)
            {
                MessageBox.Show("Right");
            }
            mouseDown = false;
        }
        
        void canvasAxis_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
            mouseDownPoint = e.GetPosition(canvasAxis);
        }               

        public void Init(ChartCalculator calculator, GroupingInterval groupingInterval)
        {            
            _chartCalculator = calculator;

            GroupingInterval = groupingInterval;

            SetUpGridLinePlaces();

            AddValues();
            DrawTicks();
        }

        public double AxisWidth
        {
            get { return canvasAxis.Width; }
            set { canvasAxis.Width = value; }
        }

        public GroupingInterval GroupingInterval
        {
            get;
            set;
        }     

        private void SetUpGridLinePlaces()
        {
            _gridLinePlaces.Clear();
            _gridLineGroupPlaces.Clear();

            var startDate = _chartCalculator.GetMinDate();
            var endDate = _chartCalculator.GetMaxDate();
            
            if (GroupingInterval == GroupingInterval.Hour)
            {
                var datesWithHours = new SortedList(_chartCalculator.Candles.Select(x => new DateTime(x.Date.Year, x.Date.Month, x.Date.Day, x.Date.Hour, 0, 0)).Distinct().ToDictionary(x => x, x => x));
                var datesWithDays = new SortedList(_chartCalculator.Candles.Select(x => new DateTime(x.Date.Year, x.Date.Month, x.Date.Day, 0, 0, 0)).Distinct().ToDictionary(x => x, x => x));

                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddHours(1))
                {
                    var date = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                    if (datesWithHours.ContainsKey(date))
                    {
                        _gridLinePlaces.Add(new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0));    
                    }                    
                }

                for (DateTime dt = startDate.Date; dt <= endDate; dt = dt.AddDays(1))
                {
                    var date = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);

                    if (datesWithDays.ContainsKey(date))
                    {
                        _gridLineGroupPlaces.Add(dt);
                    }                                        
                }                
            }
            else if (GroupingInterval == GroupingInterval.Day)
            {                
                for (DateTime dt = startDate.GetFirstDayOfWeekDate().AddDays(1); dt <= endDate; dt = dt.AddDays(1))
                {
                    if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday) continue;
                    
                    this._gridLinePlaces.Add(dt);
                }

                for (DateTime dt = startDate.GetFirstDayOfWeekDate(); dt <= endDate; dt = dt.AddDays(7))
                {
                    this._gridLineGroupPlaces.Add(dt);
                }
            }
            else if (GroupingInterval == GroupingInterval.Week)
            {
                for (DateTime dt = startDate.GetFirstDayOfWeekDate().AddDays(7.0); dt <= endDate; dt = dt.AddDays(7.0))
                {
                    this._gridLinePlaces.Add(dt);
                }

                for (DateTime dt = startDate.GetFirstDayOfMonth(); dt <= endDate; dt = dt.AddMonths(1))
                {
                    this._gridLineGroupPlaces.Add(dt);
                }
            }
            else if (GroupingInterval == GroupingInterval.TwoWeeks)
            {                
                for (DateTime dt = startDate.GetFirstDayOfWeekDate(); dt <= endDate; dt = dt.AddDays(14.0))
                {
                    this._gridLinePlaces.Add(dt);
                }

                for (DateTime dt = startDate.GetFirstDayOfMonth(); dt <= endDate; dt = dt.AddMonths(1))
                {
                    this._gridLineGroupPlaces.Add(dt);
                }

            }
        }

        private string GetText(DateTime dt, bool firstLevel)
        {
            if (GroupingInterval == GroupingInterval.Week)
            {
                if (firstLevel)
                {
                    return dt.Day.ToString();
                }
                else
                {
                    return dt.ToString("MMMM");
                }               
            }
            else if (GroupingInterval == GroupingInterval.Day)
            {
                if (firstLevel)
                {
                    return dt.Date.ToString("dd");
                }
                else
                {
                    return String.Format("Week {0} of {1} {2}", dt.Date.WeekOfMonth(), dt.Date.ToString("MMMM"), dt.Date.ToString("yyyy"));
                }               
            }
            else if (GroupingInterval == GroupingInterval.Hour)
            {
                if (firstLevel)
                {
                    return dt.TimeOfDay.ToString("hh")+"h";
                }
                else
                {
                    return String.Format("{0} of {1} {2}", dt.Date.ToString("dd"), dt.Date.ToString("MMMM"), dt.Date.ToString("yyyy")); 
                }
            }
            return "not specified";
        }

        private void AddValues()
        {
            canvasAxis.Children.Clear();
            
            if (_gridLinePlaces.Count > 0)
            {
                int count = _gridLinePlaces.Count;
                
                for (int i = 0; i < count; i++)
                {
                    double coordinateByValue = _chartCalculator.GetCoordinateByDate(_gridLinePlaces[i], false);

                    if (!double.IsNaN(coordinateByValue))
                    {                        
                        TextBlock element = new TextBlock();

                        //element.Padding = new Thickness(!double.IsNaN(base.ValuesFontSize) ? (base.ValuesFontSize / 10.0) : (!double.IsNaN(base.Chart.FontSize) ? (base.Chart.FontSize / 10.0) : 1.0));
                        element.Padding = new Thickness(3.0, 1.0, 3.0, 1.0);
                        element.Text = GetText(_gridLinePlaces[i], true);
                        element.TextAlignment = TextAlignment.Center;
                        element.SetValue(Canvas.TopProperty, 0.0);
                        element.SetValue(Canvas.LeftProperty, coordinateByValue);
                        canvasAxis.Children.Add(element);
                    }
                }
            }

            if (_gridLineGroupPlaces.Count > 0)
            {
                int count = _gridLineGroupPlaces.Count;

                for (int i = 0; i < count - 1; i++)
                {
                    double value = _chartCalculator.GetCoordinateByDate(_gridLineGroupPlaces[i], false);
                    double nextValue = _chartCalculator.GetCoordinateByDate(_gridLineGroupPlaces[i + 1], false);

                    if (!double.IsNaN(value))
                    {
                        TextBlock element = new TextBlock();
                        
                        element.Padding = new Thickness(3.0);
                        element.Text = GetText(_gridLineGroupPlaces[i], false);
                        element.TextAlignment = TextAlignment.Center;

                        element.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                        element.SetValue(Canvas.TopProperty, canvasAxis.ActualHeight / 2);
                        element.SetValue(Canvas.LeftProperty, (nextValue + value) / 2 - (element.DesiredSize.Width / 2));
                        canvasAxis.Children.Add(element);
                    }
                }
            }
        }


        protected void DrawTicks()
        {
            var element = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 0.1,
                X1 = 0.0,
                X2 = canvasAxis.Width,
                Y1 = (double)(int)canvasAxis.ActualHeight / 2,
                Y2 = (double)(int)canvasAxis.ActualHeight/2
            };

            canvasAxis.Children.Add(element);

            for (int i = 0; i < _gridLinePlaces.Count; i++)
            {
                double coordinateByValue = _chartCalculator.GetCoordinateByDate(_gridLinePlaces[i], false);                

                if (!double.IsNaN(coordinateByValue) && (coordinateByValue >= 0.0))
                {                                                            
                    element = new Line();
                    element.Stroke = Brushes.Black;
                    element.StrokeThickness = 0.1;
                    element.X1 = coordinateByValue;
                    element.Y1 = 0.0;
                    element.X2 = coordinateByValue;
                    element.Y2 = canvasAxis.ActualHeight/2;
                    canvasAxis.Children.Add(element);
                }
            }

            for (int i = 0; i < _gridLineGroupPlaces.Count; i++)
            {
                double coordinateByValue = _chartCalculator.GetCoordinateByDate(_gridLineGroupPlaces[i], false); 
                
                if (!double.IsNaN(coordinateByValue) && (coordinateByValue >= 0.0))
                {
                    element = new Line
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 0.1,
                        X1 = coordinateByValue,
                        X2 = coordinateByValue,
                        Y1 = canvasAxis.ActualHeight/2,
                        Y2 = canvasAxis.ActualHeight
                    };
                    canvasAxis.Children.Add(element);
                }
            }
        }
    
    }
}
