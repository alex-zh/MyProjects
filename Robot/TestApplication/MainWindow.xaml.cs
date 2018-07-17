using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Common;
using Common.Classes.General;
using Microsoft.Win32;

namespace TestApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Candle> _candles;
        private List<CrossingPoint> _intersections;
        private string _exportFileName = @"C:\result\result.txt";
        private readonly Func<DateTime, DateTime, bool> _isStartAndEndTimeOfOneDay = (startDate, endDate) =>
                                 (startDate.Hour > 10 || (startDate.Hour == 10 && startDate.Minute >= 30)) && (startDate.Day == endDate.Day);

        private readonly Func<DateTime, DateTime, DateTime, DateTime, bool> _areDatesOfOneDay = (startDate, second, third, fourth) =>
                                 (startDate.Hour > 10 || (startDate.Hour == 10 && startDate.Minute >= 30)) && (startDate.Day == second.Day && second.Day== third.Day && fourth.Day == third.Day);

        private SignificantMovementAtr _atr;

        public MainWindow()
        {
            InitializeComponent();

        //    Loaded += MainWindow_Loaded;
        }

        
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Multiselect = false, Filter = "Text|*.txt|All|*.*" };
            dialog.InitialDirectory = @"C:\Users\aaz\Desktop\TestData";

            if (dialog.ShowDialog() == true)
            {
                _candles = new CandlesLoader().FromFile(dialog.FileName).ToList();
            }

            if (_candles.Any() == false)
            {
                MessageBox.Show("There are no candles loaded.");
                return;
            }

            _atr = new SignificantMovementAtr(_candles.ToArray(), 21);

            var movingsDeviationsFinder = new MovingsIntersectionFinder(_candles);
            _intersections = movingsDeviationsFinder.Find(11, 21).ToList();

            Validate();

            var segments = CalculateIntersectionSegments();

            var result = CalculateStatistics(segments.ToList());
            Export(result);
        }

        private IEnumerable<Tuple<double, double, double, double>> CalculateStatistics(List<IntersectionSegment> segments)
        {
            var result = new List<Tuple<double, double, double, double>>();

            for (int i = 0; i < segments.Count - 4; i++)
            {
                var firstSegment = segments[i];
                var secondSegment = segments[i + 1];
                var thirdSegment = segments[i + 2];
                var fourthSegment = segments[i + 3];

                var atrValue = _atr.Calculate(_candles.GetIndexByDate(firstSegment.EndDate));

                if (Math.Abs(firstSegment.Movement) > 5*atrValue)
                {
                    if (_areDatesOfOneDay(firstSegment.StartDate, secondSegment.EndDate, thirdSegment.EndDate, fourthSegment.EndDate))
                    {
                        result.Add(new Tuple<double, double, double, double>(firstSegment.Movement, secondSegment.Movement, thirdSegment.Movement, fourthSegment.Movement));
                    }
                }
            }

            return result;
        }

        private void Validate()
        {
            var sortedIntersections = _intersections.OrderBy(x => x.Date);
            if (_intersections.SequenceEqual(sortedIntersections) == false)
            {
                throw new Exception("intersections have incorrect order");
            }
        }

        private IEnumerable<IntersectionSegment> CalculateIntersectionSegments()
        {
            var result = new List<IntersectionSegment>();

            for (int i = 0; i < _intersections.Count - 2; i++)
            {
                var firstPoint =  _intersections[i];
                var secondPoint = _intersections[i + 1];

                if (_isStartAndEndTimeOfOneDay(firstPoint.Date, secondPoint.Date))
                {
                    var segment = new IntersectionSegment();
                    segment.StartDate = firstPoint.Date;
                    segment.EndDate = secondPoint.Date;

                    segment.Movement = _candles[secondPoint.Index].Get(PriceTypes.Mean) - _candles[firstPoint.Index].Get(PriceTypes.Mean);
                    //segment.Movement = Math.Abs(segment.Movement);

                    result.Add(segment);
                }
            }

            return result;
        }

        private void Export(IEnumerable<Tuple<double, double, double, double>> points)
        {
            using (var streamWriter = new StreamWriter(_exportFileName))
            {
                streamWriter.WriteLine("Significant" + " " + "Next");

                foreach (var p in points)
                {
                    streamWriter.WriteLine(p.Item1 + "\t" + p.Item2 + "\t" + p.Item3 + "\t" + p.Item4);
                }

            }
        }
    }

    internal class IntersectionSegment
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Movement { get; set; }
    }
}
