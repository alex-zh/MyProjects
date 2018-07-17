using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Classes.General
{
    public class CorrelationCalculator
    {
        private readonly List<double> _firstSeries;
        private readonly List<double> _secondSeries;

        public CorrelationCalculator(IEnumerable<double> firstSeries, IEnumerable<double> secondSeries)
        {
            _firstSeries = firstSeries.ToList();
            _secondSeries = secondSeries.ToList();

            if (_firstSeries.Count != _secondSeries.Count)
                throw new ArgumentException("Series have different numbers of elements");
        }

        public double Calculate(CorrelationTypes correlationType)
        {
            if (correlationType == CorrelationTypes.Spearman)
            {
                return CalculateSpearman();
            }
            else if (correlationType == CorrelationTypes.Pearson)
            {
                return CalculatePeason();
            }

            throw new NotSupportedException(correlationType + " correlation is not supported");
        }

        private double CalculateSpearman()
        {
            var firstRankedSeries = _firstSeries.Select(x => _firstSeries.OrderBy(y => y).ToList().IndexOf(x) + 1).ToList();
            var secondRankedSeries = _secondSeries.Select(x => _secondSeries.OrderBy(y => y).ToList().IndexOf(x) + 1).ToList();

            var squaredSum = 0.0;
            var numberOfElements = firstRankedSeries.Count;

            for (int i = 0; i < numberOfElements; i++)
            {
                squaredSum += Math.Pow(firstRankedSeries[i] - secondRankedSeries[i], 2);
            }

            return 1 - (6 * squaredSum) / (Math.Pow(numberOfElements, 3) - numberOfElements);
        }

        private double CalculatePeason()
        {
            var firstCalculator = new StatisticsCalculator(_firstSeries);
            var secondCalculator = new StatisticsCalculator(_secondSeries);

            var firstMean = firstCalculator.Mean();
            var secondMean = secondCalculator.Mean();

            var firstSigma = firstCalculator.Sigma();
            var secondSigma = secondCalculator.Sigma();

            var sum = 0.0;
            var numberOfElements = _firstSeries.Count;

            for (int i = 0; i < numberOfElements; i++)
            {
                sum += (_firstSeries[i] - firstMean)*(_secondSeries[i] - secondMean);
            }

            return sum / (firstSigma * secondSigma * (numberOfElements - 1));
        }
    }
}
