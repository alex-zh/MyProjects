using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.CandlePattern;
using Common.Classes;
using Common.Classes.General;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Finders;

namespace Robot.Strategies
{
    public class PatternBasedStrategyAgent : IStrategyAgent
    {
        private List<Candle> _candles = new List<Candle>();
        private StrategyOptions _options;
        private readonly ILogger _logger;
        private MovingsDivergencyFinder _divergencyFinder;
        private List<DatePrice> _sma;
        private List<DatePrice> _fma;

        public PatternBasedStrategyAgent(StrategyOptions options, ILogger logger)
        {
            _options = options;
            _logger = logger;
        }

        public void Update(IEnumerable<Candle> candles)
        {
            _candles = candles.ToList();

            if (_options.SmaPeriod > _candles.Count())
            {
                _logger.Log("Sma period is bigger than number of candles", LogMessageTypes.Error);

                return;
            }

            _sma = _candles.Ema(_options.SmaPeriod, PriceTypes.Mean, true).ToList();
            _fma = _candles.Ema(_options.FmaPeriod, PriceTypes.Mean, true).ToList();
            _divergencyFinder = new MovingsDivergencyFinder(_candles, _options.FmaPeriod, _options.SmaPeriod);
        }

        public void Update(IEnumerable<Candle> firstSeries, IEnumerable<Candle> secondSeries)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateLongSeries(IEnumerable<Candle> series)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateLongSeries(IEnumerable<Candle> firstSeries, IEnumerable<Candle> secondSeries)
        {
            throw new System.NotImplementedException();
        }

        public DateTime GetLongSeriesLastUpdateTime()
        {
            return DateTime.MaxValue;
        }

        public void UpdateLongTermParameters(DistributionParameters averageUpDeviation, DistributionParameters averageDownDeviation,
            List<IntersectionSegment> lastIntersectionSegment)
        {
            //do nothing in this strategy
        }

        public bool ShouldOpen(AgentContext context, out AgentActionParams actionParams)
        {
            actionParams = null;

            if (_candles.Any() == false)
            {
                _logger.Log("_usdCandles list is empty", LogMessageTypes.Error);

                return false;
            }

            var index = _candles.Count() - 1;
            var currentCandle = _candles[index];

            if (IsLateToOpenPostion()) return false;

            if (_divergencyFinder.IsTimeInValidRange(currentCandle.Date) == false)
            {
                return false;
            }

            if (currentCandle.Close > _sma[index].Value)
            {
                return false;
            }

            var atr = new Atr(_candles.TakeRange(index - 32, index).ToArray(), 32).Calculate();
            var candlePatternsProvider = new CandlePatternsProvider(atr);
            var patternsFinder = new CandlePatternFinder();

            foreach (var upPattern in candlePatternsProvider.GetAllForUp())
            {
                var testCandles = _candles.TakeRange(index - upPattern.PatternCandlesCount + 1, index).ToArray();

                if (patternsFinder.DoesMatch(testCandles, upPattern))
                {
                    actionParams = new AgentActionParams { Direction = Direction.Up, Price = _candles.Last().Close };
                }
            }

            return actionParams != null;
        }

        public bool ShouldClose(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = null;

            if (_candles.Any() == false)
            {
                _logger.Log("Candles list is empty", LogMessageTypes.Error);

                return false;
            }

            var index = _candles.Count() - 1;

            if (IsTimeToExit())
            {
                /* выход по времени должен идти первым */
                orderParams = new AgentActionParams { Price = _candles.Last().Close };

                return true;
            }
            else if (_divergencyFinder.AreTwoLastCandlesAboveMovings(index))
            {
                //orderParams = new AgentActionParams { PositionDirection = PositionDirection.Up, Price = 0.0 };
                //orderParams = new AgentActionParams { PositionDirection = PositionDirection.Down, Price = _usdCandles.Last().Close };

                // return true;
            }

            return false;
        }


        private bool IsTimeToExit()
        {
            var candle = _candles.LastOrDefault();

            if (candle != null && candle.Date.Hour == 23 && candle.Date.Minute > 30)
            {
                return true;
            }

            return false;
        }

        private bool IsLateToOpenPostion()
        {
            var candle = _candles.LastOrDefault();

            if (candle != null && candle.Date.Hour >= 23)
            {
                return true;
            }

            return false;
        }

        public bool ShouldTakeProfit(AgentContext context, out AgentActionParams orderParams)
        {
            var index = _candles.Count() - 1;
            var currentCandle = _candles[index];

            var atr = new Atr(_candles.TakeRange(index - 32, index).ToArray(), 32).Calculate();

            orderParams = new AgentActionParams { Direction = context.PositionDirection.GetOpposite() };

            if (context.PositionDirection == Direction.Up)
            {
                orderParams.Price = currentCandle.Close + 0.5 * atr;
            }
            else
            {
                orderParams.Price = currentCandle.Close - 0.5 *atr;
            }

            return true;
        }

        public bool ShouldSetStopLoss(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = null;

            var index = _candles.Count() - 1;
            var currentCandle = _candles[index];

            if (currentCandle.Close > _fma[index].Value)
            {
                return false;
            }

            var atr = new Atr(_candles.TakeRange(index - 32, index).ToArray(), 32).Calculate();

            orderParams = new AgentActionParams { Direction = context.PositionDirection.GetOpposite() };

            if (context.PositionDirection == Direction.Up)
            {
                orderParams.Price = currentCandle.Low - 0.1 * atr;
            }
            else
            {
                orderParams.Price = currentCandle.High + 0.1 * atr;
            }

            return true;
        }

        public bool ShouldAddToPosition(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = null;
            return false;
        }
    }
}