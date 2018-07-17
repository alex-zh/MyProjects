using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;

namespace Common.Classes.Builders
{
    public class TrendBuilderNew
    {
        private readonly List<Candle> _allCandles;
        private readonly TrendBuilderOptions _options;
        private readonly Dictionary<int, double> _standardMoves = new Dictionary<int, double>();
        private readonly List<double> _trendCoefficients = new List<double> { 8, 4};
        private readonly List<double> _lookupWindowCoefficients = new List<double> { 15, 10 };

        /// <summary>
        /// Uses for tests. 
        /// In real life standard trend should be estimated automatically based on previous data
        /// </summary>        
        public TrendBuilderNew(IEnumerable<Candle> candles, TrendBuilderOptions options)
        {
            _allCandles = candles.ToList();
            _options = options;

            if (_options.LevelUpThreshold <= 0)
            {
                _options.LevelUpThreshold = _trendCoefficients[_trendCoefficients.Count/2];
            }

            if (options.IsForTest && options.TestTrendCoefficients.Count > 0)
            {
                _trendCoefficients = options.TestTrendCoefficients;
                _lookupWindowCoefficients = options.TestLookupWindowCoefficients;
            }

            FillStandardMoves();
        }

        private void FillStandardMoves()
        {
            if (_options.IsForTest)
            {
                for (int i = 0; i < _allCandles.Count; i++)
                {
                    _standardMoves.Add(i, _options.AverageOneDayMove);
                }
            }
            else
            {
                const int startIndex = 300;
                var minMove = new StatisticsCalculator(_allCandles.TakeRange(0, startIndex).Where(x => x.Date.Hour > 10 && x.Date.Hour < 23).Select(x => Math.Abs(x.Close - x.Open))).Mean();

                for (int i = 0; i < startIndex; i++)
                {
                    _standardMoves.Add(i, minMove);
                }

                for (int i = startIndex; i < _allCandles.Count; i++)
                {
                    minMove = new StatisticsCalculator(_allCandles.TakeRange(i - startIndex, i).Where(x => x.Date.Hour > 10 && x.Date.Hour < 23).Select(x => Math.Abs(x.Close - x.Open))).Mean();
                    _standardMoves.Add(i, minMove);
                }
            }
        }


        public List<TrendPart> Build()
        {
            return FindParts();
        }

        private List<TrendPart> FindParts()
        {
            var parts = new[] {new TrendPart { StartIndex = 0,
                                               EndIndex = _allCandles.Count-1,
                                               TrendType = TrendTypes.Sideward} }.ToList();

            for (var iteration = 0; iteration < _trendCoefficients.Count; iteration++)
            {
                var levelSubPartsUp = _trendCoefficients[iteration] >= _options.LevelUpThreshold;

                var lookupWindow = (int)(_lookupWindowCoefficients[iteration]);
                var trendCoefficient = _trendCoefficients[iteration];

                parts = Groom(parts, lookupWindow, trendCoefficient, levelSubPartsUp);
            }

            return parts;
        }

        private List<TrendPart> Groom(List<TrendPart> parts, int lookupWindow, double trendCoefficient, bool levelSubPartsUp)
        {
            var result = new List<TrendPart>();

            var trendParts = parts.Where(x => x.TrendType != TrendTypes.Sideward).ToList();
            var sidewardParts = parts.Where(x => x.TrendType == TrendTypes.Sideward).ToList();

            FillSubParts(trendParts, lookupWindow, trendCoefficient);
            //GroomTrendParts(allParts);

            FillSubParts(sidewardParts, lookupWindow, trendCoefficient);
            GroomSidewards(parts);

            result.AddRange(trendParts);

            if (levelSubPartsUp)
            {
                foreach (var sidewardPart in sidewardParts)
                {
                    result.AddRange(sidewardPart.Parts.Select(subPart => new TrendPart
                    {
                        StartIndex = sidewardPart.StartIndex + subPart.StartIndex,
                        EndIndex = sidewardPart.StartIndex + subPart.EndIndex,
                        TrendType = subPart.TrendType
                    }));
                }
            }
            else
            {
                result.AddRange(sidewardParts);
            }

            return result;
        }

        private void GroomTrendParts(List<TrendPart> parts)
        {
            for (int i = 0; i <= parts.Count - 1; i++)
            {
                var currentPart = parts[i];
                var previousPart = i > 0 ? parts[i - 1] : null;
                var nextPart = (i < parts.Count - 1) ? parts[i + 1] : null;

                if (currentPart.TrendType != TrendTypes.Sideward && currentPart.Parts.Count >= 2)
                {
                    var currentFirstSubPart = currentPart.Parts.First();
                    var currentLastSubPart = currentPart.Parts.Last();

                    if (previousPart != null)
                    {
                        if ((previousPart.TrendType == TrendTypes.Sideward && currentPart.TrendType == TrendTypes.Up &&
                             currentFirstSubPart.TrendType != TrendTypes.Up) ||
                            (previousPart.TrendType == TrendTypes.Sideward && currentPart.TrendType == TrendTypes.Down &&
                             currentFirstSubPart.TrendType != TrendTypes.Down))
                        {
                            previousPart.EndIndex = currentPart.StartIndex + currentFirstSubPart.EndIndex;
                            currentPart.StartIndex = currentPart.StartIndex + currentFirstSubPart.EndIndex + 1;
                        }
                        else if ((previousPart.TrendType == TrendTypes.Down && currentPart.TrendType == TrendTypes.Up &&
                                  currentFirstSubPart.TrendType == TrendTypes.Down) ||
                                 (previousPart.TrendType == TrendTypes.Up && currentPart.TrendType == TrendTypes.Down &&
                                  currentFirstSubPart.TrendType == TrendTypes.Up))
                        {
                            previousPart.EndIndex = currentPart.StartIndex + currentFirstSubPart.EndIndex;
                            currentPart.StartIndex = currentPart.StartIndex + currentFirstSubPart.EndIndex + 1;
                        }
                    }

                    if (nextPart != null)
                    {
                        if ((nextPart.TrendType == TrendTypes.Sideward && currentPart.TrendType == TrendTypes.Up &&
                             currentLastSubPart.TrendType != TrendTypes.Up) ||
                            (nextPart.TrendType == TrendTypes.Sideward && currentPart.TrendType == TrendTypes.Down &&
                             currentLastSubPart.TrendType != TrendTypes.Down))
                        {
                            nextPart.StartIndex = currentPart.StartIndex + currentLastSubPart.StartIndex;
                            currentPart.EndIndex = currentPart.StartIndex + currentLastSubPart.StartIndex - 1;
                        }
                        else if ((nextPart.TrendType == TrendTypes.Down && currentPart.TrendType == TrendTypes.Up &&
                                  currentLastSubPart.TrendType == TrendTypes.Down) ||
                                 (nextPart.TrendType == TrendTypes.Up && currentPart.TrendType == TrendTypes.Down &&
                                  currentLastSubPart.TrendType == TrendTypes.Up))
                        {
                            nextPart.StartIndex = currentPart.StartIndex + currentLastSubPart.StartIndex;
                            currentPart.EndIndex = currentPart.StartIndex + currentLastSubPart.StartIndex - 1;
                        }
                    }

                }

            }
        }

        private void GroomSidewards(List<TrendPart> allParts)
        {
            for (int i = 1; i < allParts.Count - 1; i++)
            {
                var currentPart = allParts[i];
                var nextPart = allParts[i + 1];
                var previousPart = allParts[i - 1];

                if (currentPart.TrendType == TrendTypes.Sideward && currentPart.Parts.Count >= 2)
                {
                    var currentFirstSubPart = currentPart.Parts.First();
                    var currentLastSubPart = currentPart.Parts.Last();
                    var currentPartStartIndex = currentPart.StartIndex;

                    if (currentPartStartIndex == previousPart.EndIndex + 1 )
                    {
                        if ((previousPart.TrendType == TrendTypes.Up && currentFirstSubPart.TrendType == TrendTypes.Up) ||
                            (previousPart.TrendType == TrendTypes.Down && currentFirstSubPart.TrendType == TrendTypes.Down))
                        {
                            var currentFirstSubPartEndIndex = currentFirstSubPart.EndIndex;

                            previousPart.EndIndex = currentPartStartIndex + currentFirstSubPartEndIndex;
                            currentPart.StartIndex = currentPartStartIndex + currentFirstSubPartEndIndex + 1;

                            /* after start index has been changed all subparts have incorrect offsets */

                            foreach (var subpart in currentPart.Parts)
                            {
                                subpart.StartIndex = subpart.StartIndex - currentFirstSubPartEndIndex - 1;
                                subpart.EndIndex = subpart.EndIndex - currentFirstSubPartEndIndex - 1;
                            }
                            
                            currentPart.Parts.Remove(currentFirstSubPart);
                        }
                    }

                    if (currentPart.EndIndex == nextPart.StartIndex - 1)
                    {
                        if ((nextPart.TrendType == TrendTypes.Up && currentLastSubPart.TrendType == TrendTypes.Up) ||
                            (nextPart.TrendType == TrendTypes.Down && currentLastSubPart.TrendType == TrendTypes.Down))
                        {
                            nextPart.StartIndex = currentPartStartIndex + currentLastSubPart.StartIndex;
                            currentPart.EndIndex = currentPartStartIndex + currentLastSubPart.StartIndex - 1;
                        
                            currentPart.Parts.Remove(currentLastSubPart);
                        }
                    }
                }
            }
        }

        private void FillSubParts(IEnumerable<TrendPart> trendParts, int lookupWindow, double trendCoeff)
        {
            foreach (var part in trendParts)
            {
                var candles = _allCandles.IndexRange(part.StartIndex, part.EndIndex).ToList();

                part.Parts = SplitIntoParts(candles, lookupWindow, trendCoeff);
            }
        }

        private List<TrendPart> SplitIntoParts(List<Candle> candles, int lookupWindow, double trendCoeff)
        {
            var candlesCount = candles.Count;
            var trendParts = new List<TrendPart>();

            for (int i = 0; i < candlesCount; i++)
            {
                double startValue = candles[i].Open;
                double standardTrend = _standardMoves[i] * trendCoeff;

                int lookupIndex;
                int lookupBoundary = Math.Min(candlesCount, i + lookupWindow);

                for (lookupIndex = i; lookupIndex < lookupBoundary; lookupIndex++)
                {
                    double endValue = candles[lookupIndex].Close;

                    if (Math.Abs(endValue - startValue) >= standardTrend)
                    {
                        var trendType = endValue > startValue ? TrendTypes.Up : TrendTypes.Down;

                        trendParts.Add(new TrendPart { StartIndex = i, EndIndex = lookupIndex, TrendType = trendType });
                        break;
                    }
                }

                if (lookupIndex < lookupBoundary)
                {
                    i = lookupIndex;
                }
            }

            var sidewardParts = GetSidewardParts(trendParts, candlesCount);
            trendParts.AddRange(sidewardParts);
            trendParts = trendParts.OrderBy(x => x.StartIndex).ToList();

            return trendParts;
        }

        private IEnumerable<TrendPart> GetSidewardParts(List<TrendPart> trendParts, int candlesCount)
        {
            var sidewardParts = new List<TrendPart>();

            if (trendParts.Count == 0)
            {
                sidewardParts.Add(new TrendPart
                {
                    StartIndex = 0,
                    EndIndex = candlesCount - 1,
                    TrendType = TrendTypes.Sideward
                });
            }
            else if (trendParts.Count == 1)
            {
                if (trendParts[0].StartIndex > 0)
                {
                    sidewardParts.Add(new TrendPart
                    {
                        StartIndex = 0,
                        EndIndex = trendParts[0].StartIndex - 1,
                        TrendType = TrendTypes.Sideward
                    });
                }

                if (trendParts[0].EndIndex < candlesCount - 1)
                {
                    sidewardParts.Add(new TrendPart
                    {
                        StartIndex = trendParts[0].EndIndex + 1,
                        EndIndex = candlesCount - 1,
                        TrendType = TrendTypes.Sideward
                    });
                }

            }
            else
            {
                for (int i = 1; i < trendParts.Count; i++)
                {
                    var prevPart = trendParts[i - 1];
                    var curPart = trendParts[i];

                    if (prevPart.EndIndex < curPart.StartIndex - 1)
                    {
                        sidewardParts.Add(new TrendPart
                        {
                            StartIndex = prevPart.EndIndex + 1,
                            EndIndex = curPart.StartIndex - 1,
                            TrendType = TrendTypes.Sideward
                        });
                    }
                }
            }

            return sidewardParts;
        }
    }
}
