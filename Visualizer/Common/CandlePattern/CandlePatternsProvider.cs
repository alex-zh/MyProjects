using System.Collections.Generic;
using Common.Classes.General;

namespace Common.CandlePattern
{
    /// <summary>
    /// Provides candle patterns
    /// </summary>
    public class CandlePatternsProvider
    {
        private readonly double _atrValue;
        private List<CandlePattern> _upPatterns = new List<CandlePattern>();

        public CandlePatternsProvider(double atrValue)
        {
            _atrValue = atrValue;

            Init2CandlesUpPatterns();
            Init3CandlesUpPatterns();          
        }

        private void Init2CandlesUpPatterns()
        {
            _upPatterns.Add(GetBigBlackBigWhiteOverlapingBlack());
            _upPatterns.Add(GetBigBlackWhiteWithLongLowShadow());
            _upPatterns.Add(GetBlackWithLongLowShadowBigWhiteOverlapingBlack());
        }

        private void Init3CandlesUpPatterns()
        {
            _upPatterns.Add(GetBigBlackSmallBlackBigWhiteOverlapingPreviousBlack());
            _upPatterns.Add(GetBigBlackSmallWhiteBigWhiteAlmostOverlapingPreviousBlack());
        }
        
        public List<CandlePattern> GetAllForUp()
        {
            return _upPatterns;           
        }

        public CandlePattern GetBigBlackBigWhiteOverlapingBlack()
        {
            var pattern = new CandlePattern("BigBlackBigWhiteOverlapingBlack");

            pattern.Add(0, x => (x.Open - x.Close) > 0.5 * _atrValue);
            pattern.Add(1, x => (x.Close - x.Open) > 0.5 * _atrValue);
            pattern.Add(0, (first, second) => (first.Open - first.Close) < (second.Close- second.Open));

            return pattern;
        }

        public CandlePattern GetBlackWithLongLowShadowBigWhiteOverlapingBlack()
        {
            var pattern = new CandlePattern("BlackWithLongLowShadowBigWhiteOverlapingBlack");

            pattern.Add(0, x => (x.Open - x.Close) > 0.25 * _atrValue);
            pattern.Add(0, x => (x.Open - x.Low) > 0.65 * _atrValue);

            pattern.Add(1, x => (x.Close - x.Open) > 0.5 * _atrValue);
            pattern.Add((first, second) => (first.Open - first.Low) < (second.Close - second.Low));

            return pattern;
        }

        public CandlePattern GetBigBlackWhiteWithLongLowShadow()
        {
            var pattern = new CandlePattern("BigBlackWhiteWithLongLowShadow");

            pattern.Add(0, x => (x.Open - x.Close) > 0.5 * _atrValue);
            pattern.Add(1, x => (x.Close - x.Open) > 0.25 * _atrValue);
            pattern.Add(1, x => (x.Close - x.Low) > 0.65 * _atrValue);
            pattern.Add((first, second) => (first.Open - first.Close) < (second.Close - second.Low));

            return pattern;
        }


        private CandlePattern GetBigBlackSmallBlackBigWhiteOverlapingPreviousBlack()
        {
            var pattern = new CandlePattern("BigBlackSmallBlackBigWhiteOverlapingPreviousBlack");

            pattern.Add(0, x => (x.Open - x.Close) > 0.5 * _atrValue);
            pattern.Add(1, x => (x.Open - x.Close) > 0.25 * _atrValue);
            pattern.Add(2, x => (x.Close - x.Open) > 0.5 * _atrValue);
            pattern.Add((first, second, third) => third.Close > first.Get(PriceTypes.Mean));

            return pattern;
        }

        private CandlePattern GetBigBlackSmallWhiteBigWhiteAlmostOverlapingPreviousBlack()
        {
            var pattern = new CandlePattern("BigBlackSmallWhiteBigWhiteAlmostOverlapingPreviousBlack");

            pattern.Add(0, x => (x.Open - x.Close) > 0.5 * _atrValue ||
                                (x.Open - x.Close) > 0.25 * _atrValue && (x.Open - x.Low) > 0.65 * _atrValue);

            pattern.Add(2, x => (x.Close - x.Open) > 0.5 * _atrValue);

            pattern.Add(0, (f, s) => (f.Open > s.Open) && f.Low < s.Low);

            pattern.Add((first, second, third) => third.Close > first.Open - 0.1 * _atrValue);

            return pattern;

        }

    }
}
