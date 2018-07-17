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

namespace Robot.Strategies.Agents
{
    public class ImpulseCatcherStrategyAgent : IStrategyAgent
    {
        private List<Candle> _candles = new List<Candle>();
        private readonly StrategyOptions _options;
        private readonly ILogger _logger;
        private MovingsDivergencyFinder _divergencyFinder;
        private List<DatePrice> _sma;
        private List<DatePrice> _fma;
        private readonly Func<DateTime, bool> _isValidDateTime = (date) => (date.Hour > 10 || (date.Hour == 10 && date.Minute > 45));
        private List<IntersectionSegment> _lastIntersectionSegments;

        public ImpulseCatcherStrategyAgent(StrategyOptions options, ILogger logger)
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
        }

        public void Update(IEnumerable<Candle> firstSeries, IEnumerable<Candle> secondSeries)
        {
            throw new NotImplementedException();
        }

        public void UpdateLongSeries(IEnumerable<Candle> series)
        {
            throw new NotImplementedException();
        }

        public void UpdateLongSeries(IEnumerable<Candle> firstSeries, IEnumerable<Candle> secondSeries)
        {
            throw new NotImplementedException();
        }

        public DateTime GetLongSeriesLastUpdateTime()
        {
            return DateTime.MaxValue;            
        }

        public void UpdateLongTermParameters(DistributionParameters averageUpDeviation, 
                                             DistributionParameters averageDownDeviation,
                                             List<IntersectionSegment> lastIntersectionSegment)
        {
            _lastIntersectionSegments = lastIntersectionSegment;
        }

        public bool ShouldOpen(AgentContext context, out AgentActionParams actionParams)
        {
            actionParams = null;

            if (_candles.Any() == false)
            {
                _logger.Log("Candles list is empty", LogMessageTypes.Error);

                return false;
            }

            var index = _candles.Count() - 1;
            var currentCandle = _candles[index];

            if (_isValidDateTime(currentCandle.Date) == false)
                return false;

            if (IsLateToOpenPostion()) return false;
           
            var atr = new SimpleAtr(_candles.TakeRange(index - 42, index).ToArray()).Calculate();

            if (currentCandle.Open > _fma[index].Value)
                return false;

            if (currentCandle.Close - currentCandle.Open > 1 * atr)
            {
                actionParams = new AgentActionParams { Direction = Direction.Up, Price = currentCandle.Close + 0.01 * atr };
            }

            if (actionParams != null)
            {
                if (_lastIntersectionSegments.Any(x => x.Direction == Direction.Down && Math.Abs(x.Movement) > 5 * atr))
                {
                    return true;
                }

                return false;
            }

            return false;
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
            var currentCandle = _candles[index];

            if (IsTimeToExit())
            {
                /* выход по времени должен идти первым */
                orderParams = new AgentActionParams { Price = _candles.Last().Close };

                return true;
            }


            if (context.LastCompletedOrderType == RobotOrderTypes.OpenPosition)
            {
                if ((currentCandle.Date - context.LastCompletedOrder.DateTime).TotalMinutes > 50)
                {
                    orderParams = new AgentActionParams { Price = 0.0 };
                    return true;
                }
            }

            //if (currentCandle.Close < _fma[index].Value)
            //{
            //    orderParams = new AgentActionParams { Price = 0.0 };
            //    return true;
            //}                

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
            orderParams = null;            

            return false;
        }

        public bool ShouldSetStopLoss(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = new AgentActionParams { Direction = context.PositionDirection.GetOpposite() };

            if (context.LastCompletedOrder.RobotOrderType == RobotOrderTypes.OpenPosition)
            {
                var openPositionDateTime = context.LastCompletedOrder.DateTime;
                var index = _candles.GetIndexByDate(openPositionDateTime);

                if (index > 1)
                {
                    var candle = _candles[index - 1];
                    orderParams.Price = candle.Low;
                    return true;
                }                
            }          

            return false;
        }

        public bool ShouldAddToPosition(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = null;
            return false;
        }
    }
}