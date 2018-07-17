using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes;
using Common.Classes.General;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Finders;

namespace Robot.Strategies
{    
    public class DeviationEaterStrategyAgent : IStrategyAgent
    {
        private List<Candle> _candles = new List<Candle>();
        private readonly StrategyOptions _options;
        private readonly ILogger _logger;
        private MovingsAnalizer _movingsAnalizer;
        private DistributionParameters _upParameters;
        private DistributionParameters _downParameters;

        public DeviationEaterStrategyAgent(StrategyOptions options, ILogger logger)
        {                      
            _options = options;
            _logger = logger;
        }

        public void Update(IEnumerable<Candle> candles)
        {
            _candles = candles.ToList();

            if (_options.SmaPeriod > _candles.Count())
            {
                _logger.Log("Sma period is bigger than number of usd candles", LogMessageTypes.Error);

                return;
            }

            _movingsAnalizer = new MovingsAnalizer(_candles, _options.FmaPeriod, _options.SmaPeriod);
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

        public void UpdateLongTermParameters(DistributionParameters upParameters,
                                             DistributionParameters downParameters,
                                             List<IntersectionSegment> lastIntersectionSegments)
        {
            _upParameters = upParameters;
            _downParameters = downParameters;
        }
        
        public bool ShouldOpen(AgentContext  context, out AgentActionParams actionParams)
        {
            actionParams = null;

            if (_candles.Any() == false)
            {
                _logger.Log("Candles list is empty in ShouldOpen method", LogMessageTypes.Error);

                return false;
            }

            var index = _candles.Count() - 1;
            var currentCandle = _candles[index];

            if (IsLateToOpenPostion()) return false;

            if (context.LastCompletedOrderType == RobotOrderTypes.StopLoss)
            {
                if ((currentCandle.Date - context.LastCompletedOrder.DateTime).TotalMinutes < 30)
                {
                    return false;
                }
            }
            
            var maValue = _movingsAnalizer.GetFma(index);

            if (currentCandle.Close > maValue)
            {
                var add = CalculateDeviation(Direction.Up);

                var price = (maValue) + add;

                actionParams = new AgentActionParams { Direction = Direction.Down, Price = price };

            }else if (currentCandle.Close < maValue)
            {
                var add = CalculateDeviation(Direction.Down);

                var price = (maValue) + add;

                actionParams = new AgentActionParams { Direction = Direction.Up, Price = price };
            }

            return actionParams != null;
            
        }

        private double CalculateDeviation(Direction direction)
        {
            var parameters = direction == Direction.Up ? _upParameters : _downParameters;
            const double factor = -1;

            var add = Math.Max(_upParameters.Median, _downParameters.Median) - factor * Math.Max(_upParameters.MedianSigma, _downParameters.MedianSigma) ;

            return add * direction.ToInt();
        }

        public bool ShouldClose(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = null;

            if (_candles.Any() == false)
            {
                _logger.Log("Candles list is empty in ShouldClose method", LogMessageTypes.Error);

                return false;
            }

            if (IsTimeToExit())
            {
                /* выход по времени должен идти первым */
                orderParams = new AgentActionParams { Price = _candles.Last().Close };

                return true;
            }

            var index = _candles.Count() - 1;
            var currentCandle = _candles[index];

            var add = Math.Max(_upParameters.Median, _downParameters.Median);

            var maValue = _movingsAnalizer.GetFma(index);

            bool closeDown = false, closeUp=false;

            if (currentCandle.Low < maValue)
            {
                closeDown = true;
            }
            if (currentCandle.High > maValue)
            {
                closeUp = true;
            }

            if (closeDown || closeUp)
            {
                var direction = (closeDown && closeUp) ? (Direction?)null : (closeUp ? Direction.Up : Direction.Down);

                orderParams = new AgentActionParams { Direction = direction, Price = 0.0 };

                return true;

            }

            return orderParams != null;
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
            orderParams = null;

            if (context.LastCompletedOrderType == RobotOrderTypes.OpenPosition)
            {
                orderParams = new AgentActionParams { Direction = context.PositionDirection.GetOpposite() };

                if (context.PositionDirection == Direction.Up)
                {
                    orderParams.Price = context.LastCompletedOrder.Params.Price * (1 - 0.002);
                }
                else
                {
                    orderParams.Price = context.LastCompletedOrder.Params.Price * (1 + 0.002);
                }
            }

            return orderParams != null;
        }

        public bool ShouldAddToPosition(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = null;
            return false;
        }
    }
}