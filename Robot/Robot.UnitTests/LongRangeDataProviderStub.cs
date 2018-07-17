using System;
using System.Collections.Generic;
using Common;
using Common.Classes;
using Common.Classes.General;

namespace Robot.UnitTests
{
    internal class LongRangeDataProviderStub : ILongRangeDataProvider
    {
        public DateTime GetTimeOfLastDeviationsUpdate()
        {
            return DateTime.MinValue;
        }

        public DateTime GetTimeOfLastIntersectionsUpdate()
        {
            return DateTime.MinValue;
        }

        public void RecalculateDeviations(IEnumerable<Candle> candles)
        {
            //Do nothing
        }

        public void RecalculateIntersections(IEnumerable<Candle> candles)
        {
            //
        }

        public void RecalculateCandlesStatistics(IEnumerable<Candle> candles)
        {
            throw new NotImplementedException();
        }

        public CandlesStatistics GetCandlesStatistics()
        {
            throw new NotImplementedException();
        }

        public DistributionParameters GetDeviations(Direction direction)
        {
            return new DistributionParameters();
        }
       
        public List<IntersectionSegment> GetLastSegments(DateTime currentDate, int count)
        {
            return new List<IntersectionSegment>();
        }
        
        public DateTime GetTimeOfLastUpdate()
        {
            return DateTime.MinValue;
        }

        public DistributionParameters GetUp(DateTime date)
        {
            return new DistributionParameters();
        }

        public DistributionParameters GetDown(DateTime date)
        {
            return new DistributionParameters();
        }

        
    }
}