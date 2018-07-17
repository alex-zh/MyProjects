using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;
using Common.Classes.Logger;
using Common.Finders;

namespace Common.Classes
{
    public abstract class BaseLongTermParametersProvider
    {
        protected int FmaPeriod;
        protected int SmaPeriod;
        private ILogger _logger;
        protected List<Candle> _candlesForStatistics = new List<Candle>();
        protected List<Candle> _candlesForDeviations = new List<Candle>();
        protected List<Candle> _candlesForIntersections = new List<Candle>();

        protected const int RangeLength = 1000;

        private DistributionParameters _upDeviations = new DistributionParameters();
        private DistributionParameters _downDeviations = new DistributionParameters();
        
        private IEnumerable<IntersectionSegment> _segments;
        private CandlesStatistics _statistics;

        protected BaseLongTermParametersProvider(ILogger logger)
        {
            _logger = logger;
        }

        public DateTime GetTimeOfLastUpdate()
        {
            var timeOfDeviationsUpdate = GetTimeOfLastDeviationsUpdate();
            var timeOfIntersectionsUpdate = GetTimeOfLastIntersectionsUpdate();

            if (timeOfIntersectionsUpdate > timeOfDeviationsUpdate)
                return timeOfIntersectionsUpdate;
            else
                return timeOfDeviationsUpdate;

        }
        
        public List<IntersectionSegment> GetLastSegments(DateTime currentDate, int count)
        {
            /* нужен сегмент текущего дня, без утреннего гэпа */
            if (_segments != null && _segments.Any())
            {
                var segments = _segments.Where(x => x.EndDate <= currentDate && x.EndDate.Date == currentDate.Date).ToList();

                if (segments.Any())
                {
                    return segments.TakeLast(count).ToList();
                }

            }

            return new List<IntersectionSegment>();
        }

        public DistributionParameters GetDeviations(Direction direction)
        {            
            return direction == Direction.Buy? _upDeviations : _downDeviations;
        }

        public CandlesStatistics GetCandlesStatistics()
        {
            return _statistics;
        }

        protected void RecalculateCandlesStatistics()
        {
            if (_candlesForStatistics.Count < 1)
            {
                _logger.Log("Count of candles for statitstics is less then 1", LogMessageTypes.Warning);
                return;
            }

            _statistics = new CandlesStatisticsCalculator(_candlesForStatistics).Calculate();
        }

        protected void RecalculateIntersections()
        {
            if (_candlesForIntersections.Count < SmaPeriod)
            {
                _logger.Log("Count of candles for intersections is less then needed for sma", LogMessageTypes.Warning);
                return;    
            }

            var finder = new MovingsIntersectionFinder(_candlesForIntersections);
            var intersections = finder.Find(FmaPeriod, SmaPeriod).ToList();

            _segments = CalculateIntersectionSegments(intersections);
        }

        
        public void RecalculateDeviations()
        {
            if (_candlesForDeviations.Count < SmaPeriod)
            {
                _logger.Log("Count of candles for deviations is less then needed for fma period", LogMessageTypes.Error);
                return;
            }

            var deviationsFinder = new DeviationsFromMovingFinder(_candlesForDeviations, SmaPeriod);

            _upDeviations = new StatisticsCalculator(deviationsFinder.FindFor(Direction.Buy)).GetParameters();
            _downDeviations = new StatisticsCalculator(deviationsFinder.FindFor(Direction.Sell)).GetParameters();

        }

        private DateTime GetTimeOfLastDeviationsUpdate()
        {
            return (_candlesForDeviations != null && _candlesForDeviations.Any()) ? _candlesForDeviations.Last().Date : DateTime.MinValue;
        }

        private DateTime GetTimeOfLastIntersectionsUpdate()
        {
            return (_candlesForIntersections != null && _candlesForIntersections.Any()) ? _candlesForIntersections.Last().Date : DateTime.MinValue;
        }


        private IEnumerable<IntersectionSegment> CalculateIntersectionSegments(List<CrossingPoint> intersections)
        {
            CrossingPoint firstPoint, secondPoint;
            IntersectionSegment segment;

            var result = new List<IntersectionSegment>();

            int length = intersections.Count;           

            if (length == 0)
            {
                var firstCandle = _candlesForIntersections.First();
                var lastCandle = _candlesForIntersections.Last();

                segment = new IntersectionSegment
                {
                    StartDate = firstCandle.Date,
                    EndDate = lastCandle.Date,
                    Movement = CalculateMovement(0, _candlesForIntersections.Count - 1)
                };

                result.Add(segment);
            }      
            else if (length == 1)
            {
                var firstCandle = _candlesForIntersections.First();
                var lastPoint = intersections.Last();
                
                segment = new IntersectionSegment
                {
                    StartDate = firstCandle.Date,
                    EndDate = lastPoint.Date,
                    Movement = CalculateMovement(0, lastPoint.Index)
                };

                result.Add(segment);
            }
            else if ( length > 1)
            {
                for (int i = 0; i < length - 1; i++)
                {
                    firstPoint = intersections[i];
                    secondPoint = intersections[i + 1];

                    if (Funcs.AreDatesOfOneDay(firstPoint.Date, secondPoint.Date))
                    {
                        segment = new IntersectionSegment
                        {
                            StartDate = firstPoint.Date,
                            EndDate = secondPoint.Date,
                            Movement = CalculateMovement(firstPoint.Index, secondPoint.Index)
                        };

                        result.Add(segment);
                    }
                }              
            }

            if (length > 0)
            {
                /* add the last one*/
                firstPoint = intersections.Last();
                var lastCandle = _candlesForIntersections.Last();

                segment = new IntersectionSegment
                {
                    StartDate = firstPoint.Date,
                    EndDate = lastCandle.Date,
                    Movement = CalculateMovement(firstPoint.Index, _candlesForIntersections.Count - 1)
                };

                result.Add(segment);
            }

            return result;
        }

        private double CalculateMovement(int firstPointIndex, int secondPointIndex)
        {            
            var firstCandle = _candlesForIntersections[firstPointIndex];
            var secondCandle = _candlesForIntersections[secondPointIndex];

            Direction direction = firstCandle.Get(PriceTypes.Mean) < secondCandle.Get(PriceTypes.Mean) ? Direction.Buy : 
                                                                                                         Direction.Sell;

            var startRange = _candlesForIntersections.IndexRange(Math.Max(0, firstPointIndex - FmaPeriod), firstPointIndex);
            var endRange = _candlesForIntersections.IndexRange(Math.Max(0, secondPointIndex - FmaPeriod), secondPointIndex);

            if (direction == Direction.Buy)
            {
                return endRange.Max(x => x.Get(PriceTypes.Mean)) - startRange.Min(x => x.Get(PriceTypes.Mean));
            }
            else
            {
                return endRange.Min(x => x.Get(PriceTypes.Mean)) - startRange.Max(x => x.Get(PriceTypes.Mean));
            }
        }
    }
}