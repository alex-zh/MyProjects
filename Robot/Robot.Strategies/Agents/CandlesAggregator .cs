using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;

namespace Robot.Strategies.Agents
{
    public class CandlesAggregator
    {
        private readonly AggregationSettings _settings;

        public CandlesAggregator(AggregationSettings settings)
        {
            _settings = settings;
        }

        public List<Candle> Aggregate(List<Candle> candles)
        {
            switch (_settings.AggregationInteral)
            {
                case AggregationInteral.Hour:
                    return AggregateHours(candles);
                case AggregationInteral.Day:
                    return AggregateDays(candles);
            }

            throw new NotSupportedException($"Aggregation for {_settings.AggregationInteral} is not supported");
        }

        private List<Candle> AggregateDays(List<Candle> candles)
        {
            var result = new List<Candle>();

            if (candles.Any() == false) return result;

            var currentDate = candles[0].Date.Date;
            var currentCandleList = new List<Candle>();

            foreach (var currentCandle in candles)
            {
                if (currentCandle.Date.Date == currentDate)
                {
                    currentCandleList.Add(currentCandle);
                }
                else
                {
                    var dayCandle = GenerateCandle(currentCandleList);
                    result.Add(dayCandle);

                    currentCandleList.Clear();
                    currentDate = currentCandle.Date.Date;
                    currentCandleList.Add(currentCandle);
                }
            }

            if (currentCandleList.Any())
                result.Add(GenerateCandle(currentCandleList));

            return result;
        }

        private Candle GenerateCandle(List<Candle> currentCandleList)
        {
            var candles = currentCandleList.OrderBy(x => x.Date).ToList();

            var date = (_settings.DateTimeType == CandleDateTimeType.StartOfCandle)
                ? candles.First().Date
                : candles.Last().Date;

            return new Candle()
            {
                Date = date,
                Open = candles.First().Open,
                Close = candles.Last().Close,
                Low = candles.GetMin(),
                High = candles.GetMax()
            };
        }

        private List<Candle> AggregateHours(List<Candle> candles)
        {
            var result = new List<Candle>();

            if (candles.Any() == false) return result;

            var currentDate = candles[0].Date.Date;
            var currentHour = candles[0].Date.Hour;

            var currentCandleList = new List<Candle>();

            foreach (var currentCandle in candles)
            {
                if (currentCandle.Date.Date == currentDate &&
                    currentCandle.Date.Hour == currentHour)
                {
                    currentCandleList.Add(currentCandle);
                }
                else
                {
                    var dayCandle = GenerateCandle(currentCandleList);
                    result.Add(dayCandle);

                    currentCandleList.Clear();
                    currentDate = currentCandle.Date.Date;
                    currentHour = currentCandle.Date.Hour;
                    currentCandleList.Add(currentCandle);
                }
            }

            if (currentCandleList.Any())
                result.Add(GenerateCandle(currentCandleList));

            return result;
        }
    }
}