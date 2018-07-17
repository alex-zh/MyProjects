﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;

namespace Common.Classes.Builders
{
    public class TrendBuilder
    {
        private readonly List<Candle> _allCandles;
        private readonly TrendBuilderOptions _options;
        private List<TrendPart> _trendParts = new List<TrendPart>();
        private readonly Dictionary<int, double> _standardMoves = new Dictionary<int, double>();

        /// <summary>
        /// Uses for tests. 
        /// In real life standard trend should be estimated automatically based on previous data
        /// </summary>        
        public TrendBuilder(IEnumerable<Candle> candles, TrendBuilderOptions options)
        {
            _allCandles = candles.ToList();
            _options = options;

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
            _trendParts = SplitIntoMainParts();

            //if (_options.SplitIntoSubparts)
            //{
            //    int trendCoeff = _options.TrendCoefficient/2;
            //    SplitIntoSubParts(_trendParts, trendCoeff);
            //    RegroupSubParts(_trendParts);
            //}            

            return _trendParts;
        }

        private List<TrendPart> SplitIntoMainParts()
        {
            var majorParts = SplitIntoParts(_allCandles, (int)(1.5 * _options.LookupWindow), 2 * _options.TrendCoefficient);
            var sidewardParts = majorParts.Where(x => x.TrendType == TrendTypes.Sideward).ToList();

            SplitIntoSubParts(sidewardParts, _options.TrendCoefficient);

            var parts = majorParts.Where(x => x.TrendType != TrendTypes.Sideward).ToList();

            foreach (var part in sidewardParts)
            {
                foreach (var subPart in part.Parts)
                {
                    parts.Add(new TrendPart
                    {
                        StartIndex = part.StartIndex + subPart.StartIndex,
                        EndIndex = part.StartIndex + subPart.EndIndex,
                        TrendType = subPart.TrendType
                    });

                }

            }

            return parts;
        }

        private void RegroupSubParts(List<TrendPart> trendParts)
        {
            for (int i = 1; i < trendParts.Count; i++)
            {
                var currentPart = _trendParts[i];
                var previousPart = _trendParts[i - 1];

                if (currentPart.Parts.Count >= 2)
                {
                    var perviousLastSubPart = previousPart.Parts.Last();
                    var currentFirstSubPart = currentPart.Parts.First();

                    if (((previousPart.TrendType == TrendTypes.Up && currentPart.TrendType != TrendTypes.Up) ||
                         (previousPart.TrendType != TrendTypes.Down && currentPart.TrendType == TrendTypes.Down)) &&
                        currentFirstSubPart.TrendType == TrendTypes.Up)
                    {
                        previousPart.EndIndex = currentPart.StartIndex + currentFirstSubPart.EndIndex;
                        currentPart.StartIndex = currentPart.StartIndex + currentFirstSubPart.EndIndex + 1;
                    }
                    else if (previousPart.TrendType == TrendTypes.Down && currentPart.TrendType != TrendTypes.Down &&
                        currentFirstSubPart.TrendType == TrendTypes.Down)
                    {
                        previousPart.EndIndex = currentPart.StartIndex + currentFirstSubPart.EndIndex;
                        currentPart.StartIndex = currentPart.StartIndex + currentFirstSubPart.EndIndex + 1;
                    }
                    else if (previousPart.TrendType == TrendTypes.Sideward && currentPart.TrendType == TrendTypes.Down &&
                       perviousLastSubPart.TrendType == TrendTypes.Down)
                    {
                        previousPart.EndIndex = previousPart.StartIndex + perviousLastSubPart.StartIndex;
                        currentPart.StartIndex = previousPart.StartIndex + perviousLastSubPart.StartIndex + 1;
                    }

                }
            }
        }

        private void SplitIntoSubParts(IEnumerable<TrendPart> trendParts, int trendCoeff)
        {
            foreach (var part in trendParts)
            {
                var candles = _allCandles.IndexRange(part.StartIndex, part.EndIndex).ToList();
                var subParts = SplitIntoParts(candles, _options.LookupWindow, trendCoeff);
                part.Parts = subParts;
            }
        }

        private List<TrendPart> SplitIntoParts(List<Candle> candles, int lookupWindow, int trendCoeff)
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