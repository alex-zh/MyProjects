using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;
using Common.Regression;
using Visualizer.VisualControls.Classes;

namespace JointAssetsViewer.Controls
{    
    public partial class BivariateChartInfo
    {
        public BivariateChartInfo()
        {
            InitializeComponent();
        }

        internal void Init(LinearRegression regression, List<BivariateChartPoint> list)
        {
            SetRegressionInfo(regression, list);
            SetDistancesInfo(regression, list);
        }

        private void SetRegressionInfo(LinearRegression regression, List<BivariateChartPoint> list)
        {
            tbSe.Text = regression.StandardEstimateError.ToString("F3");
            tbRSquared.Text = regression.RSquared.ToString("F3");
            tbCausedPercent.Text = CalculateCausedPercent(regression, list);

            var sign = regression.a1 > 0 ? "+" : "-";
            tbRegression.Text = String.Format("{0} {1} {2} * X",regression.a0.ToString("F3"), sign, Math.Abs(regression.a1).ToString("F3"));

            tbCoefficient.Text = regression.RegressionCoefficient.ToString("F3");
        }

        private static string CalculateCausedPercent(LinearRegression regression, List<BivariateChartPoint> list)
        {
            var a0 = regression.a0;
            var a1 = regression.a1;            
            var lastValue = list.Last().FirstValue;

            return ((a1 * lastValue) / (a0 + a1 * lastValue)).ToString("F3") + " %";
        }


        private void SetDistancesInfo(LinearRegression regression, List<BivariateChartPoint> list)
        {
            var gauger = new DistanceFromPointToRegressionLineGauger(regression);

            var distances = list.Select(t => gauger.GetDistance(t.FirstValue, t.SecondValue)).ToList();

            var upDistances = distances.Where(x => x > 0);
            var downDistances = distances.Where(x => x < 0);

            var upStatistics = new StatisticsCalculator(upDistances);
            var downStatistics = new StatisticsCalculator(downDistances);

            tbUp10thPercentile.Text = upStatistics.Quantile((double)10/9).ToString("F3");
            tbDown10thPercentile.Text = downStatistics.Quantile(9).ToString("F3");

            tbCurrentDistance.Text = gauger.GetDistance(list.Last().FirstValue, list.Last().SecondValue).ToString("F3");
        }
    }
}
