using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Classes.General
{
    public class StatisticsCalculator
    {
        private readonly IEnumerable<double> _values;

        public StatisticsCalculator(IEnumerable<double> values)
        {
            _values = values;
        }

        public double Mean()
        {
            if (_values.Any() == false)
            {
                return 0;
            }

            return _values.Sum() / (_values.Count());
        }

        public double Sigma()
        {
            if (_values.Any() == false)
            {
                return 0;
            }

            var mean = Mean();

            return Math.Sqrt(_values.Sum(x => Math.Pow(x - mean, 2)) / (_values.Count() - 1));
        }

        public double Median()
        {
            return Quantile(2);            
        }

        public double Quantile(double divisor)
        {
            if (_values.Any() == false)
            {
                return 0;
            }

            var quatileIndex = (int)(_values.Count() / divisor);
            return _values.OrderBy(x => x).ToArray()[quatileIndex];
        }        

        public double MedianSigma()
        {
            if (_values.Any() == false)
            {
                return 0;
            }

            var median = Median();

            return new StatisticsCalculator(_values.Select(x => Math.Abs(x - median))).Median();
        }

        public DistributionParameters GetParameters()
        {                        
            return new DistributionParameters
            {
                Mean = Mean(),
                Sigma = Sigma(),
                Median = Median(),
                MedianSigma = MedianSigma(),
                FirstQuartile = Quantile(8),
                ThirdQuartile = Quantile((double)8/7),
                Percentile90 = Quantile((double)100/90),
                Percentile95 = Quantile((double)100/95),
                Percentile97 = Quantile((double)100 / 97),
                Percentile99 = Quantile((double)100/99)
            };

        }
    }
}
