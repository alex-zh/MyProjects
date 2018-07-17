using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;

namespace Common.CandlePattern
{
    public class CandlePattern
    {        
        private readonly Dictionary<int, List<Func<Candle, bool>>> _oneCandlePatterns = new Dictionary<int, List<Func<Candle, bool>>>();
        private readonly Dictionary<int, List<Func<Candle, Candle, bool>>> _twoCandlePatterns = new Dictionary<int, List<Func<Candle, Candle, bool>>>();
        private readonly Dictionary<int, List<Func<Candle, Candle, Candle, bool>>> _threeCandlePatterns = new Dictionary<int, List<Func<Candle, Candle, Candle, bool>>>();

        public CandlePattern(string patternName)
        {
            PatternName = patternName;
        }

        public string PatternName { get; set; }

        public int PatternCandlesCount
        {
            get
            {
                if (_threeCandlePatterns.Keys.Any()) return 3;
                if (_twoCandlePatterns.Keys.Contains(1)) return 3;
                if (_oneCandlePatterns.Keys.Contains(2)) return 3;

                if (_twoCandlePatterns.Keys.Any()) return 2;
                if (_oneCandlePatterns.Keys.Contains(1)) return 2;

                if (_oneCandlePatterns.Keys.Any()) return 1;

                return 0;
            }
        }
        public List<Func<Candle, bool>> GetOneCandlePatterns(int index)
        {
            return _oneCandlePatterns.ContainsKey(index) ? _oneCandlePatterns[index] :
                                                           new List<Func<Candle, bool>>();
        }

        public List<Func<Candle, Candle, bool>> GetTwoCandlePatterns(int index)
        {
            return _twoCandlePatterns.ContainsKey(index) ? _twoCandlePatterns[index] : 
                                                           new List<Func<Candle, Candle, bool>>();
        }

        public List<Func<Candle, Candle, Candle, bool>> GetThreeCandlePatterns(int index)
        {
            return _threeCandlePatterns.ContainsKey(index) ? _threeCandlePatterns[index] : 
                                                             new List<Func<Candle, Candle, Candle, bool>>();
        }

        public void Add(int index, Func<Candle, bool> pattern)
        {
            if (index > 2)
            {
                throw new ArgumentException("Maximum supported number of candles is 3");
            }

            if (_oneCandlePatterns.ContainsKey(index) == false)
            {
                _oneCandlePatterns.Add(index, new List<Func<Candle, bool>>());
            }

            _oneCandlePatterns[index].Add(pattern);
        }

        public void Add(Func<Candle, Candle, bool> pattern)
        {
            Add(0, pattern);
        }

        public void Add(int index, Func<Candle, Candle, bool> pattern)
        {
            if (index > 1)
            {
                throw new ArgumentException("Maximum supported number of candles is 3");
            }

            if (_twoCandlePatterns.ContainsKey(index) == false)
            {
                _twoCandlePatterns.Add(index, new List<Func<Candle, Candle, bool>>());
            }

            _twoCandlePatterns[index].Add(pattern);
        }

        public void Add(Func<Candle, Candle, Candle, bool> pattern)
        {
            Add(0, pattern);
        }

        public void Add(int index, Func<Candle, Candle, Candle, bool> pattern)
        {
            if (_threeCandlePatterns.ContainsKey(index) == false)
            {
                _threeCandlePatterns.Add(index, new List<Func<Candle, Candle, Candle, bool>>());
            }

            _threeCandlePatterns[index].Add(pattern);
        }

    }
}
