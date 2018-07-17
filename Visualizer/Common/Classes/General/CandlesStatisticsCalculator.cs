using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Classes.General
{
    public class CandlesStatisticsCalculator
    {
        private readonly IEnumerable<Candle> _candles;

        public CandlesStatisticsCalculator(IEnumerable<Candle> candles)
        {
            _candles = candles;
        }

        public CandlesStatistics Calculate()
        {
            DistributionParameters bodyResult = null,
                                   tailsUpResult = null,
                                   tailsDownResult = null,
                                   swingResult = null,
                                   volumeResult = null;

            Parallel.Invoke(
                () =>
                {
                    var tailsUp = _candles.Select(x => x.High - Math.Max(x.Close, x.Open));
                    tailsUpResult = new StatisticsCalculator(tailsUp).GetParameters();
                },
                () =>
                {
                    var tailsDown = _candles.Select(x => Math.Min(x.Close, x.Open) - x.Low);
                    tailsDownResult = new StatisticsCalculator(tailsDown).GetParameters();
                },
                () =>
                {
                    var body = _candles.Select(x => Math.Abs(x.Close - x.Open));
                    bodyResult = new StatisticsCalculator(body).GetParameters();
                },
                () =>
                {
                    var swing = _candles.Select(x => Math.Abs(x.High - x.Low));
                    swingResult = new StatisticsCalculator(swing).GetParameters();
                },
                () =>
                {
                    var volume = _candles.Select(x => x.Volume);
                    volumeResult = new StatisticsCalculator(volume).GetParameters();
                });

            return new CandlesStatistics()
            {
                Count = _candles.Count(),
                Body = bodyResult,
                TailsUp = tailsUpResult,
                TailsDown = tailsDownResult,
                Swing = swingResult,
                Volume = volumeResult
            };
        }
    }
}
