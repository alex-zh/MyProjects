using System.Collections.Generic;
using Common.Classes.Robot;
using Visualizer.VisualControls.Classes;

namespace JointAssetsViewer
{
    internal class JointAssetsBivariateChartPointsProvider
    {
        private const int CandlesCount = 2500; // 3 days * 14 hours * 60 minutes
        private readonly QuikDataProvider _provider;
        private readonly bool _useLogarithmicScale;


        public JointAssetsBivariateChartPointsProvider(QuikDataProvider provider, bool useLogarithmicScale)
        {
            _provider = provider;
            _useLogarithmicScale = useLogarithmicScale;
        }

        public List<BivariateChartPoint> Get(Securities first, Securities second)
        {
            var firstCandles = _provider.GetCandles(first, CandlesCount);
            var secondCandles = _provider.GetCandles(second, CandlesCount);

            return new BrentUsdBivariatePointsBuilder(firstCandles, secondCandles, _useLogarithmicScale).Build();

        }
    }
}