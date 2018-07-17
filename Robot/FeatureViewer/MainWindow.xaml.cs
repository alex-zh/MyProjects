using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Common;
using Common.Classes.General;
using FeatureViewer.Classes;
using FeatureViewer.Features;
using FeatureViewer.Windows;
using Viewer;
using Viewer.Windows;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace FeatureViewer
{
    public partial class MainWindow
    {
        private List<Candle> _allCandles;
        private List<int> _foundResult;

        public MainWindow()
        {
            Loaded += MainWindow_Loaded;
            KeyDown += OnPreviewKeyDown;
            InitializeComponent();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var workingRectangle = Screen.PrimaryScreen.WorkingArea;

            Width = workingRectangle.Width * 0.85;
            Height = workingRectangle.Height * 0.85;
        }

        private void LoadMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Multiselect = false, Filter = @"Text|*.txt|All|*.*" };
            dialog.InitialDirectory = @"C:\CryptoData";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _allCandles = new CandlesLoader().FromFile(dialog.FileName).ToList();

                Viewer.ShowCandles(_allCandles);
            }
        }

        private void StartAndShow_OnClick(object sender, RoutedEventArgs args)
        {
            try
            {
                //var featureDetector = new BigCandleCrossingMovingsUpFeatureDetector(_allCandles, 200, 11, 21);
                var featureDetector = new ImpulseFeatureDetector(_allCandles, 11, 21);

                _foundResult = featureDetector.Detect();

                MessageBox.Show(_foundResult.Count.ToString());

                ShowFoundResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ShowFoundResult()
        {
            Viewer.InitCustomDrawRangeFunction(CustomDrawRangeFunctionForFoundResult);
            Viewer.ShowCandles(_allCandles);
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.G)
            {
                var goToForm = new StockChartGoToForm { Owner = this };
                var result = goToForm.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    var index = goToForm.SelectedIndex;
                    Viewer.Chart.GoTo(index);
                }
            }
        }

        public void CustomDrawRangeFunctionForFoundResult(Canvas canvas, DateTime startDate, DateTime endDate)
        {
            if (_foundResult == null || _foundResult.Count < 1) return;

            Viewer.ClearShapes();

            var rangeStartIndex = _allCandles.GetIndexByDate(startDate);
            var rangeEndIndex = _allCandles.GetIndexByDate(endDate);

            // var rangeResult = _foundResult.Where(x => x.StartIndex >= rangeStartIndex && x.EndIndex <= rangeEndIndex);

            var rangeResult = _foundResult.Where(x => x >= rangeStartIndex && x <= rangeEndIndex);

            foreach (var x in rangeResult)
            {
                //var candle = _allCandles[index];
                var rangeShapes = new SignalBuilder(Viewer.StockChart).BuildHorizontalRange(x-60, x-1);

                Viewer._shapes.AddRange(rangeShapes);
            }

            Viewer.DrawShapes();
        }

        private void BuildAndShowSummary_OnClick(object sender, RoutedEventArgs e)
        {
            if (_allCandles.Count < 1)
            {
                MessageBox.Show("Candles are not loaded.");
                return;
            }

            StructureTester st = new StructureTester(_allCandles);
            st.BuildStatistics();

            SummaryWindow window = new SummaryWindow();
            window.Show();
        }
    }
}
