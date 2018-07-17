using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;

namespace Common.Finders
{
    public class DeviationsBetweenMovingsIntersectionsFinder
    {
        private List<double> _upDeviations;
        private List<double> _downDeviations;
        private readonly List<CrossingPoint> _intersections;
        private readonly SortedList<DateTime, Candle> _sortedCandles;
        private readonly Func<DateTime, DateTime, bool> _isStartAndEndTimeOfOneDay = (startDate, endDate) =>
                                 (startDate.Hour > 10 || (startDate.Hour == 10 && startDate.Minute >= 30)) && (startDate.Day == endDate.Day);
        private readonly Func<DateTime, bool> _isValidDateTime = (date) => (date.Hour > 10 || (date.Hour == 10 && date.Minute > 59));

        private readonly List<DatePrice> _fma;
        private readonly List<DatePrice> _sma;
        private List<Candle> _candles;

        public DeviationsBetweenMovingsIntersectionsFinder(List<Candle> candles, int fmaPeriod, int smaPeriod)
        {
            _candles = candles;
            _fma = candles.Ema(fmaPeriod, PriceTypes.Mean).ToList();
            _sma = candles.Ema(smaPeriod, PriceTypes.Mean).ToList();

            _sortedCandles = new SortedList<DateTime, Candle>(candles.ToDictionary(x => x.Date));

            var movingsDeviationsFinder = new MovingsIntersectionFinder(candles);
            _intersections = movingsDeviationsFinder.Find(fmaPeriod, smaPeriod).ToList();

            CalculateDeviations();
        }

        public List<CrossingPoint> GetIntersections()
        {
            return _intersections;
        }

        public List<double> GetFor(Direction direction)
        {
            switch (direction)
            {
                case Direction.Buy:
                    return _upDeviations;
                case Direction.Sell:
                    return _downDeviations;
                default:
                    throw new NotSupportedException(direction.ToString());
            }
        }

        public IEnumerable<DistributionBin> GetBins(Direction direction)
        {
            var values = GetFor(direction);

            var minValue = values.Min();
            var maxValue = values.Max();

            var bins = new List<DistributionBin>();

            int binsCount = (int)Math.Sqrt(2 * values.Count);
            var step = (maxValue - minValue)/binsCount;

            var lowerBound = minValue;
            var upperBound = minValue + step;

            while (upperBound < maxValue)
            {
                var count = values.Count(x => x >= lowerBound && x < upperBound);

                bins.Add(new DistributionBin() { MinValue = lowerBound, MaxValue = upperBound, Count = count });

                lowerBound = upperBound;
                upperBound = upperBound + step;
            }

            return bins;
        }

        private void CalculateDeviations()
        {
            _upDeviations = new List<double>();
            _downDeviations = new List<double>();

            for (int i = 1; i < _intersections.Count; i++)
            {
                var startDate = _intersections[i - 1].Date;
                var endDate = _intersections[i].Date;

                if (_isStartAndEndTimeOfOneDay(startDate, endDate))
                {
                    var deviation = CaclulateMaximumDeviation(_sortedCandles, startDate, endDate);

                    if (_intersections[i - 1].Direction == Direction.Buy)
                    {
                        _upDeviations.Add(deviation);
                    }
                    else
                    {
                        _downDeviations.Add(deviation);
                    }
                }
            }
        }

        private double CaclulateMaximumDeviation(SortedList<DateTime, Candle> candles, DateTime startDate, DateTime endDate)
        {
            var deviations = new List<double>();

            var startIndex = candles.IndexOfKey(startDate);
            var endIndex = candles.IndexOfKey(endDate);

            for (int i = startIndex; i < endIndex; i++)
            {
                var mean = (_fma[i].Value + _sma[i].Value) / 2;
                var deviation = Math.Max(Math.Abs(mean - candles.Values[i].Low), Math.Abs(candles.Values[i].High - mean));

                deviations.Add(deviation);
            }

            return deviations.Any() ? deviations.Max() : 0;
        }       
    }
}
