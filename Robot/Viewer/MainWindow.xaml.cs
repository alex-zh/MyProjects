using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Common;
using Common.Classes;
using Common.Classes.Builders;
using Common.Classes.General;
using Common.Finders;
using Viewer.classes;
using Viewer.Windows;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Candle> _allCandles;
        private List<TrendFeature> _similarFeatures;
        private List<Structure> _foundStructures;

        public MainWindow()
        {
            this.Loaded += MainWindow_Loaded;
            this.KeyDown += OnPreviewKeyDown;
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
            var dialog = new OpenFileDialog { Multiselect = false, Filter = "Text|*.txt|All|*.*" };
           // dialog.InitialDirectory = @"C:\Patterns\Buy";
            dialog.InitialDirectory = @"C:\MyProjects\Robot\TestData\MoexSi\1 min";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _allCandles = new CandlesLoader().FromFile(dialog.FileName).ToList();

                Viewer.ShowCandles(_allCandles);
            }
        }


        private void StartAndShowPattern_OnClick(object sender, RoutedEventArgs args)
        {            
           


           //var builder = new StructureBuilder(_allCandles);
           // var structureInfo = StructureParser.ParseFromText(_allCandles, tbStartIndex.Text, false);

           // var r = builder.Build();
           //ShowFoundStructures(new List<List<StructureItem>> { r });
           // ShowFoundStructures(r);

            //return;
           

            try
            {
                var structure  = StructureTester.FindStructureById(tbStartIndex.Text.Trim());

                if (structure == null) return;

                var structureFinder = new EmaRecursiveStructureFinder(_allCandles, structure, false, true);
                var result = structureFinder.FindAll();
                MessageBox.Show("Found: " + result.Count);
                tbResult.Text =  string.Join("; ", result.Select(x=>x[0].StartIndex));

                ShowFoundStructures(result);
        
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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

        private void ShowFoundStructures(List<Structure> result)
        {
            _foundStructures = result;
            Viewer.InitCustomDrawRangeFunction(CustomDrawRangeFunctionForFoundStructure);
            Viewer.ShowCandles(_allCandles);
        }

        private void ShowSimilarFeatures(List<TrendFeature> similarFeatures)
        {
            _similarFeatures = similarFeatures;
            Viewer.InitCustomDrawRangeFunction(CustomDrawRangeFunction);
            Viewer.ShowCandles(_allCandles);
        }

        public void CustomDrawRangeFunctionForFoundStructure(Canvas canvas, DateTime startDate, DateTime endDate)
        {
            if (_foundStructures == null) return;

            Viewer.ClearShapes();

            var rangeStartIndex = _allCandles.GetIndexByDate(startDate);
            var rangeEndIndex = _allCandles.GetIndexByDate(endDate);

            var rangeStructures = _foundStructures.Where(x => x[0].StartIndex >= rangeStartIndex &&
                                                            x[x.Count - 1].EndIndex <= rangeEndIndex);

            foreach (var structure in rangeStructures)
            {
                for (int i = 0; i < structure.Count; i += 1)
                {
                    var featureStartIndex =  structure[i].StartIndex;
                    var featureEndIndex = structure[i].EndIndex;

                    var shapes =
                        Viewer.BuildShapes(
                            new DatePrice()
                            {
                                Date = _allCandles[featureStartIndex].Date,
                                Value = _allCandles[featureStartIndex].Get(PriceTypes.Mean)
                            },
                            new DatePrice()
                            {
                                Date = _allCandles[featureEndIndex].Date,
                                Value = _allCandles[featureEndIndex].Get(PriceTypes.Mean)
                            });

                    Viewer._trendsShapes.AddRange(shapes);
                }
            }

            Viewer.DrawShapes();
        }

        public void CustomDrawRangeFunction(Canvas canvas, DateTime startDate, DateTime endDate)
        {
            if(_similarFeatures == null) return;
            
            Viewer.ClearShapes();

            var rangeFeatures = _similarFeatures.Where(x => x.StartDate >= startDate && 
                                                            x.EndDate <= endDate);
            var fma = _allCandles.Ema(5, PriceTypes.Mean).ToList();

            foreach (var feature in rangeFeatures)
            {
                for (int i = 0; i < feature.Values.Count; i+=1)
                {
                    var featureStartDate = feature.Values[i].StartDate;
                    var featureEndDate = feature.Values[i].EndDate;
                    var featureStartIndex = _allCandles.GetIndexByDate(featureStartDate);
                    var featureEndIndex = _allCandles.GetIndexByDate(featureEndDate);

                    var shapes =
                        Viewer.BuildShapes(
                            new DatePrice()
                            {
                                Date = featureStartDate,
                                Value = _allCandles[featureStartIndex].Close
                            },
                            new DatePrice()
                            {
                                Date = featureEndDate,
                                Value = _allCandles[featureEndIndex].Close
                            });

                    Viewer._trendsShapes.AddRange(shapes);
                }
            }            

            Viewer.DrawShapes();
        }


        private void FindStructures_OnClick(object sender, RoutedEventArgs e)
        {
            StructureTester st = new StructureTester(_allCandles);
            var result = st.FindAndSaveStructures();
            ShowFoundStructures(result);
        }

        private void BuildStatistics_OnClick(object sender, RoutedEventArgs e)
        {
            if (_allCandles.Count < 1)
            {
                MessageBox.Show("Candles are not loaded.");
                return;
            }
               
            StructureTester st = new StructureTester(_allCandles);
            st.BuildStatistics();
        }

        private void BuildAndShowSummary_OnClick(object sender, RoutedEventArgs e)
        {
            StructureTestSummaryWindow window = new StructureTestSummaryWindow();
            window.Show();
        }

        private void ShowSummaryForCurrent_OnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(tbStartIndex.Text))
            {
                MessageBox.Show("Structure pattern is not specified.");
                return;
            }

            StructureTester st = new StructureTester(_allCandles);
            var structure = StructureTester.FindStructureById(tbStartIndex.Text.Trim());

            st.BuildStatisticsFor(structure);

            StructureTestSummaryWindow window = new StructureTestSummaryWindow();
            window.Init(structure);

            window.Show();
        }
    }

    
}
