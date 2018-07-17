using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes;
using Common.Classes.General;
using Common.Classes.StrategyTester;
using Common.Finders;

namespace Robot.TesterApplication.Controls
{   
    public partial class MiscellaneousInfoControl
    {
        public MiscellaneousInfoControl()
        {
            InitializeComponent();            
        }

        public void BindData(IEnumerable<Candle> allCandles)
        {
            var candles = allCandles.ToList();
            tbCandlesCount.Text = candles.Count.ToString();

            var movingsDeviationsFinder = new DeviationsFromMovingFinder(candles, 11);

            //tbMovingsIntersectionsCount.Text = movingsDeviationsFinder.GetIntersections().Count().ToString();

            tbUpAverageMaximumDeviation.Text = movingsDeviationsFinder.FindFor(Direction.Buy).Average().ToString("N2");
            tbDownAverageMaximumDeviation.Text = movingsDeviationsFinder.FindFor(Direction.Sell).Average().ToString("N2");

            //UpDistributionViewer.Init(movingsDeviationsFinder.GetBins(PositionDirection.Buy));
            //DownDistributionViewer.Init(movingsDeviationsFinder.GetBins(PositionDirection.Sell));
            var distributionCalculatorUp = new DistributionCalculator(movingsDeviationsFinder.FindFor(Direction.Buy), BinsCountEstimationMethods.Simple);
            var distributionCalculatorDown = new DistributionCalculator(movingsDeviationsFinder.FindFor(Direction.Sell), BinsCountEstimationMethods.Simple);

            DistributionChartUp.Init(distributionCalculatorUp.GetBins(), distributionCalculatorUp.GetParameters());
            DistributionChartDown.Init(distributionCalculatorDown.GetBins(), distributionCalculatorUp.GetParameters());
        }
    }
}
