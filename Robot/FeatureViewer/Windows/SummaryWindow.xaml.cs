using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Common;
using Common.Classes;
using Common.Finders;
using Viewer;
using Visualizer.VisualControls.Controls;
using Orientation = System.Windows.Controls.Orientation;

namespace FeatureViewer.Windows
{
    public partial class SummaryWindow
    {
        private StructureInfo _structure = null;

        public SummaryWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var workingRectangle = Screen.PrimaryScreen.WorkingArea;

            Width = workingRectangle.Width * 0.85;
            Height = workingRectangle.Height * 0.85;

            BuildAndShowSummary();
        }

        private void BuildAndShowSummary()
        {
            if (_structure == null)
            {
                foreach (var fileName in Directory.GetFiles(StructureTester.TestStructureResultFolderPath))
                {
                    var result = new JsonSerializer<TestStructureResult>(fileName).Read();
                    var resultId = Path.GetFileNameWithoutExtension(fileName);

                    BuildAndShowResult(resultId, result);
                }
            }
            else
            {
                var path = Path.Combine(StructureTester.TestFolder, _structure.Id + ".txt");
                var result = new JsonSerializer<TestStructureResult>(path).Read();

                BuildAndShowResult(_structure.Id, result, false);
            }

        }

        private void BuildAndShowResult(string resultId, TestStructureResult result, bool useFilter = true)
        {
            var bins = new List<DistributionBin>();

            var numberOfBins = result[0].Returns.Count;

            for (int i = 0; i < numberOfBins; i++)
            {
                var bin = new DistributionBin
                {
                    MinValue = i,
                    MaxValue = i + 1
                };

                bin.Count = (int) result.Average(x => x.Returns[i]);

                bins.Add(bin);
            }

            if (useFilter && bins.Max(x => Math.Abs(x.Count)) < 50)
            {
                return;
            }

            var distributionChart = new DistributionChart();
            distributionChart.Init(bins, new DistributionParameters(), true);
            distributionChart.Title = "Count: " + result.Count + "   for " + "'" + resultId + "'";
            distributionChart.MinHeight = 400;


            var differenceBins = new List<DistributionBin>();

            for (int i = 0; i < numberOfBins; i++)
            {
                var bin = new DistributionBin
                {
                    MinValue = i,
                    MaxValue = i + 1
                };

                bin.Count = (result.Count(x => x.Returns[i]>=0) - result.Count(x => x.Returns[i] < 0));

                differenceBins.Add(bin);
            }

            var differenceChart = new DistributionChart();
            differenceChart.Init(differenceBins, new DistributionParameters(), true);
            differenceChart.Title = "Positive Minus Negative";
            differenceChart.MinHeight = 400;

            var sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            sp.Children.Add(distributionChart);
            sp.Children.Add(differenceChart);

            StackPanel.Children.Add(sp);

        }

        public void ShowFor(StructureInfo structure)
        {
            throw new NotImplementedException();
        }

        public void Init(StructureInfo structure)
        {
            _structure = structure;
        }
    }
}
