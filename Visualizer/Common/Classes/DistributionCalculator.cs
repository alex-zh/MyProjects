using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;

namespace Common.Classes
{
    public class DistributionCalculator
    {
        private readonly BinsCountEstimationMethods _binsEstimationMethods;
        private readonly int _minBinsCount;
        private readonly List<double> _values;

        public DistributionCalculator(IEnumerable<double> values, 
                                      BinsCountEstimationMethods binsEstimationMethods = BinsCountEstimationMethods.FreedmanDiaconis,
                                      int minBinsCount = 10)
        {            
            _values = values.ToList();
            _binsEstimationMethods = binsEstimationMethods;
            _minBinsCount = minBinsCount;
        }

        public DistributionParameters GetParameters()
        {
            return new StatisticsCalculator(_values).GetParameters();
        }

        public IEnumerable<DistributionBin> GetBins()
        {
            var minValue = _values.OrderBy(x => x).ToList()[_values.Count / 200];
            var maxValue = _values.OrderByDescending(x => x).ToList()[_values.Count / 200];

            //var minValue = _values.Min();
            //var maxValue = _values.Max();

            var bins = new List<DistributionBin>();

            int binsCount = Math.Max(GetBinsCount(_binsEstimationMethods), _minBinsCount);
            var step = (maxValue - minValue) / binsCount;

            var lowerBound = minValue;
            var upperBound = minValue + step;

            while (upperBound < maxValue)
            {
                var count = 0;
                if (upperBound <= minValue + step)
                {
                    count = _values.Count(x => x < upperBound);
                }
                else if (upperBound >= maxValue - step)
                {
                    count = _values.Count(x => x > upperBound);
                }
                else
                {
                    count = _values.Count(x => x >= lowerBound && x < upperBound);
                }

                bins.Add(new DistributionBin { MinValue = lowerBound, MaxValue = upperBound, Count = count });

                lowerBound = upperBound;
                upperBound = upperBound + step;
            }

            return bins;
        }

        private int GetBinsCount(BinsCountEstimationMethods method)
        {
            switch (method)
            {
                case BinsCountEstimationMethods.BinForEachElement:

                    return _values.Count;

                case BinsCountEstimationMethods.Simple:

                    return (int) Math.Sqrt(2*_values.Count);
                    
                case BinsCountEstimationMethods.FreedmanDiaconis:

                    var minValue = _values.OrderBy(x => x).ToList()[_values.Count / 200];
                    var maxValue = _values.OrderByDescending(x => x).ToList()[_values.Count / 200];
                    var interquartileRange = GetInterquartileRange();

                    var interal = 2*interquartileRange/Math.Pow(_values.Count, -1/3);

                    return (int)((maxValue - minValue) / interal);

                default:
                    throw new ArgumentOutOfRangeException("method");
            }
        }

        private double GetInterquartileRange()
        {
            var sortedValues = _values.OrderBy(x => x).ToList();
            var firstQuartileIndex = _values.Count / 3;
            var thirdQuartileIndex = _values.Count * 2 / 3;

            return sortedValues[thirdQuartileIndex] - sortedValues[firstQuartileIndex];
        }
    }

    
}
