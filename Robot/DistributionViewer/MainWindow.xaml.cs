using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Common;
using Common.Classes;
using Common.Classes.General;
using DistributionViewer.DistributionProviders;
using Visualizer.VisualControls.Controls;
using MessageBox = System.Windows.MessageBox;


namespace DistributionViewer
{
    public partial class MainWindow
    {
        private static string PatternsFolderPath = "C:\\Patterns\\Buy";
        private static Dictionary<string, List<Candle>> _dictionary = new Dictionary<string, List<Candle>>();
        private List<Candle> _candles;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void LoadCandles_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Multiselect = false, Filter = "Text|*.txt|All|*.*" };
            dialog.InitialDirectory = @"C:\Users\aaz\Desktop\TestData";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _candles = new CandlesLoader().FromFile(dialog.FileName).ToList();
            }

            if (_candles == null || _candles.Any() == false)
            {
                MessageBox.Show("There are no candles loaded.");
            }
            else
            {
                ShowDistributionChart();
            }
        }

        private void ShowDistributionChart()
        {
            var bins = new TimePerformaceDistributionProvider(_candles).Get();
            DistributionChart.Init(bins, new DistributionParameters(), true);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var workingRectangle = Screen.PrimaryScreen.WorkingArea;

            Width = workingRectangle.Width * 0.85;
            Height = workingRectangle.Height * 0.85;


        }


         

        private void BuildForEachMinute()
        {
            var count = 0;
            for (int hour = 10; hour < 23; hour++)
            {
                Dictionary<int, List<double>> minuteValues = new Dictionary<int, List<double>>();

                for (int candleIndex = 0; candleIndex < _candles.Count - 30; candleIndex++)
                {
                    var candle = _candles[candleIndex];
                    var candleNext = _candles[candleIndex + 30];

                    if (candle.Date.Hour != hour) continue;
                    if (candle.Date.Hour == 10 && candle.Date.Minute == 0)
                    {
                        count++;
                    }
                    var minute = candle.Date.TimeOfDay.Minutes;

                    if (minuteValues.ContainsKey(minute) == false)
                        minuteValues[minute] = new List<double>();

                    var list = minuteValues[minute];

                    list.Add((candleNext.Close - candle.Open) * 100 * 1000 / candle.Open);

                }

                var bins = new List<DistributionBin>();

                foreach (var key in minuteValues.Keys)
                {
                    var bin = new DistributionBin();
                    bin.MinValue = key;
                    bin.MaxValue = key;
                    bin.Count = (int)minuteValues[key].Average();
                    bins.Add(bin);
                }

                //var distributionCalculatorMeans = new DistributionCalculator(values, BinsCountEstimationMethods.BinForEachElement);

                var chart = new DistributionChart()
                {
                    Title = "hour - " + hour,
                    Name = "chart_" + hour,
                    Padding = new Thickness(6),
                    Margin = new Thickness(3),
                    MinHeight = 480
                };


                chart.Init(bins, new DistributionParameters(), true);


            }

            MessageBox.Show(count.ToString());
        }

        private void BuildForDayOfAWeek()
        {
            var count = 0;

            for (int dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++)
            {
                Dictionary<int, List<double>> minuteValues = new Dictionary<int, List<double>>();

                for (int candleIndex = 0; candleIndex < _candles.Count - 30; candleIndex++)
                {
                    var candle = _candles[candleIndex];
                    var candleNext = _candles[candleIndex + 30];

                    if (candle.Date.Hour != 15) continue;
                    if (candle.Date.DayOfWeek != (DayOfWeek)dayOfWeek) continue;

                    if (candle.Date.Hour == 10 && candle.Date.Minute == 0)
                    {
                        count++;
                    }
                    var minute = candle.Date.TimeOfDay.Minutes;

                    if (minuteValues.ContainsKey(minute) == false)
                        minuteValues[minute] = new List<double>();

                    var list = minuteValues[minute];

                    list.Add((candleNext.Close - candle.Open) * 100 * 1000 / candle.Open);

                }

                var bins = new List<DistributionBin>();

                foreach (var key in minuteValues.Keys)
                {
                    var bin = new DistributionBin();
                    bin.MinValue = key;
                    bin.MaxValue = key;
                    bin.Count = (int)minuteValues[key].Average();
                    bins.Add(bin);
                }

                //var distributionCalculatorMeans = new DistributionCalculator(values, BinsCountEstimationMethods.BinForEachElement);

                var chart = new DistributionChart()
                {
                    Title = "hour - " + (DayOfWeek)dayOfWeek,
                    Name = "chart_" + dayOfWeek,
                    Padding = new Thickness(6),
                    Margin = new Thickness(3),
                    MinHeight = 480
                };


                chart.Init(bins, new DistributionParameters(), true);

            }

            MessageBox.Show(count.ToString());
        }

    }


}
