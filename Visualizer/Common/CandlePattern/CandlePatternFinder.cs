using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Common.Classes.General;

namespace Common.CandlePattern
{
    public class CandlePatternFinder
    {
        public List<int> FindAll(List<Candle> candles, CandlePattern pattern)
        {
            var result = new List<int>();
            var patternDimension = pattern.PatternCandlesCount;

            if (patternDimension < 1)
            {
                throw new ArgumentException("Pattern has zero dimention (candles count the pattent based on)");
            }

            for (int i = 0; i < candles.Count - patternDimension; i++)
            {
                var testCandles = candles.TakeRange(i, i + patternDimension - 1);

                if (DoesMatch(testCandles, pattern))
                {
                    result.Add(i);
                }
            }

            return result;
        }

        public bool DoesMatch(IEnumerable<Candle> testCandles, CandlePattern pattern)
        {
            var candles = testCandles.ToList();

            if (candles.Count < 1 || candles.Count > 3)
            {
                throw new ArgumentException("There must be less than 3 and more than 1 candles");
            }

            if (candles.Count != pattern.PatternCandlesCount)
            {
                throw new ArgumentException("Pattern expects different amount of test candles");
            }

            for (var i = 0; i < candles.Count; i++)
            {
                var patterns = pattern.GetOneCandlePatterns(i);

                if (patterns.Any(x => x(candles[i]) == false))
                {
                    return false;
                }
            }

            for (var i = 0; i < candles.Count - 1; i++)
            {
                var patterns = pattern.GetTwoCandlePatterns(i);

                if (patterns.Any(x => x(candles[i], candles[i + 1]) == false))
                {
                    return false;
                }
            }

            return true;
        }
    }
}