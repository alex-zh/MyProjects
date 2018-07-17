using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using Common;
using Common.Classes.General;
using Visualizer.VisualControls.Classes;
using Brushes = System.Windows.Media.Brushes;

namespace JointAssetsViewer
{
    internal class BrentUsdBivariatePointsBuilder
    {
        private readonly bool _useLogarithmicScale;
        private readonly IEnumerable<Candle> _firstCandles;
        private readonly IEnumerable<Candle> _secondCandles;

        public BrentUsdBivariatePointsBuilder(IEnumerable<Candle> firstCandles, IEnumerable<Candle> secondCandles, bool useLogarithmicScale)
        {
            _useLogarithmicScale = useLogarithmicScale;

            _firstCandles = firstCandles.Select(x => x.Clone()).ToList();
            _secondCandles = secondCandles.Select(x => x.Clone()).ToList();

            //foreach (var brCandle in _secondCandles)
            //{
            //    brCandle.High = brCandle.High * 1000;
            //    brCandle.Low = brCandle.Low * 1000;
            //    brCandle.Open = brCandle.Open * 1000;
            //    brCandle.Close = brCandle.Close * 1000;
            //}
        }

        public List<BivariateChartPoint> Build()
        {
            var result = new List<BivariateChartPoint>();

            var jointPoints = _firstCandles.Join(_secondCandles, x => x.Date, y => y.Date, (x, y) => new
            {
                firstCandle = x,
                secondCandle = y,                
            }).ToList();

            const int startIndex = 10;

            for (var i = startIndex; i < jointPoints.Count() - 10; i += 30)
            {
                var p = jointPoints[i];

                var currentIndex = i;
                var pointsRange = jointPoints.Where((x, index) => index > currentIndex - 10 && index < currentIndex + 10).ToArray();
                var firstAverage = pointsRange.Select(x => x.firstCandle.Get(PriceTypes.Mean)).Average();
                var secondAverage = pointsRange.Select(x => x.secondCandle.Get(PriceTypes.Mean)).Average();
                
                result.Add(new BivariateChartPoint
                {
                    Index = i - startIndex,
                    FirstValue = firstAverage,
                    SecondValue = secondAverage ,
                    Data = new BrSiPair(p.firstCandle, p.secondCandle)
                });
            }          
            
            var lastPoint = jointPoints.LastOrDefault();

            if (lastPoint != null)

            {
                result.Add(new BivariateChartPoint
                {
                    Index = result.Count,
                    FirstValue = lastPoint.firstCandle.Close,
                    SecondValue = lastPoint.secondCandle.Close,
                    Data = new BrSiPair(lastPoint.firstCandle, lastPoint.secondCandle),
                    Radius = 10,
                    Brush = Brushes.DarkBlue,                    
                });
            }


            if (_useLogarithmicScale)
            {
                foreach (var point in result)
                {
                    point.FirstValue = Math.Log(point.FirstValue);
                    point.SecondValue = Math.Log(point.SecondValue);
                }
            }

            return result;
        }
    }
}