using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Common;
using Common.Classes.General;

namespace Viewer.Windows
{
    public partial class PatternWindow
    {
        private TrendFeature _trendFeature;
        private List<Candle> _allCandles;

        public PatternWindow()
        {
            InitializeComponent();
        }

        public void InitCandles(List<Candle> candles, int workingSize)
        {
            _allCandles = candles;
            _trendFeature = new HierarchyFeatureDetector(candles, workingSize, 1).Detect()[0];
            Viewer.InitCustomDrawRangeFunction(CustomDrawRangeFunction);
            Viewer.ShowCandles(candles);
        }

        public void CustomDrawRangeFunction(Canvas canvas, DateTime startDate, DateTime endDate)
        {
            Viewer.ClearShapes();

            var fma = _allCandles.Ema(5, PriceTypes.Mean).ToList();

            if (_trendFeature != null)
            {
                for (int i = 0; i < _trendFeature.Values.Count; i+=1)
                {
                    var featureStartDate = _trendFeature.Values[i].StartDate;
                    var featureEndDate = _trendFeature.Values[i].EndDate;
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
    }
}
