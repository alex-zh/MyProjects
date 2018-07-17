using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes;
using Common.Classes.General;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Finders;

namespace Robot.Strategies.Agents
{    
    public class BrentToUsdMeanRevertionAgent : IStrategyAgent
    {
        private List<Candle> _candles = new List<Candle>();
        private readonly StrategyOptions _options;
        private readonly ILogger _logger;
        private MovingsDivergencyFinder _usdDivergencyFinder;
        private MovingsAnalizer _usdMovingsAnalizer;
        private DistributionParameters _upParameters;
        private DistributionParameters _downParameters;
        private List<IntersectionSegment> _lastIntersectionSegments;
        private IEnumerable<Candle> _usdSeries;
        private IEnumerable<Candle> _brentSeries;

        public BrentToUsdMeanRevertionAgent(StrategyOptions options, ILogger logger)
        {                      
            _options = options;
            _logger = logger;
        }

        public void Update(IEnumerable<Candle> candles)
        {
            throw new NotImplementedException("This agent expects two series: usd and brent");
        }

        public void Update(IEnumerable<Candle> usdSeries, IEnumerable<Candle> brentSeries )
        {
            _usdSeries = usdSeries;
            _brentSeries = brentSeries;
        }

        public void UpdateLongTermParameters(DistributionParameters upParameters,
                                             DistributionParameters downParameters,
                                             List<IntersectionSegment> lastIntersectionSegments)
        {
            _upParameters = upParameters;
            _downParameters = downParameters;
            _lastIntersectionSegments = lastIntersectionSegments;
        }
        
        public bool ShouldOpen(AgentContext context, out AgentActionParams actionParams)
        {
            actionParams = null;

            if (_candles.Any() == false)
            {
                _logger.Log("Candles list is empty in ShouldOpen method", LogMessageTypes.Error);

                return false;
            }

            DivergencyPoint point;
            var index = _candles.Count() - 1;
            var currentCandle = _candles[index];

            if (IsLateToOpenPostion()) return false;

            if (IsPointAfterSignificantMovement(_usdDivergencyFinder.GetLongAtr(index)))
            {
                //return false;
            }

            if (_usdDivergencyFinder.IsTimeInValidRange(currentCandle.Date) == false)
            {
                return false;
            }

            if (_usdDivergencyFinder.IsDivergencyPoint(index, out point))
            {
                var add = CalculateAddition(point.Direction);

                var price = (point.FmaPrice) + add;

                actionParams = new AgentActionParams { Direction = point.Direction, Price = price };
            }
            
            if(actionParams != null)
            {
                /* Нужно проверить, что на этой же свече нет сигнала к закрытию позиции, 
                   Иначе может получится большое количество пустых сделок */
                AgentActionParams testForCloseParams;
                if (ShouldClose(context, out testForCloseParams))
                {
                    if (actionParams.Direction == testForCloseParams.Direction)
                    {
                        return false;
                    }
                }

                var direction = actionParams.Direction;
                if (_lastIntersectionSegments.Any(x => x.Direction != direction &&
                                                       Math.Abs(x.Movement) > 5 * _usdDivergencyFinder.GetLongAtr(index) &&
                                                       (int)(currentCandle.Date - x.EndDate).TotalMinutes < x.TimeSpan / 2))
                {
                    return false;
                }                
            }

            return actionParams != null;
            
        }

        private double CalculateAddition(Direction direction)
        {
            var parameters = direction == Direction.Up ? _upParameters : _downParameters;
            const double factor = 0;

            var add = parameters.Median - factor*parameters.MedianSigma;

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

            var index = _candles.Count() - 1;

            var closeUp = false;
            var closeDown = false;

            if (IsTimeToExit())
            {
                /* выход по времени должен идти первым */
                orderParams = new AgentActionParams { Price = _candles.Last().Close };

                return true;
            }
            if (_usdDivergencyFinder.AreTwoLastCandlesAboveMovings(index))
            {
                closeDown = true;                     
            }
            if (_usdDivergencyFinder.AreTwoLastCandlesBelowMovings(index))
            {
                closeUp = true;
            }
            if (_usdDivergencyFinder.IsCandleAboveMovingsAndThreeNextAboveSlowMoving(index))
            {
                closeDown = true;                     
            }
            if (_usdDivergencyFinder.IsCandleBelowMovingsAndThreeNextBelowSlowMoving(index))
            {
                closeUp = true;                     
            }
            
            if (closeDown || closeUp)
            {
                var direction = (closeDown && closeUp) ? (Direction?)null : (closeUp ? Direction.Up : Direction.Down);

                orderParams = new AgentActionParams { Direction = direction, Price = 0.0 };

                return true;

            }

            return false;
        }

        private bool IsPointAfterSignificantMovement(double atr)
        {
            if (_lastIntersectionSegments.Any() == false) return false;

            var lastSegment = _lastIntersectionSegments.Last();

            var time = lastSegment.TimeSpan;
            var movement = Math.Abs(lastSegment.Movement);

            if (movement > 5*atr && movement/time > 9.5)
            {
                return true;
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
            orderParams = null;

            if (_candles.Any() == false)
            {
                _logger.Log("Candles list is empty in method ShouldTakeProfit", LogMessageTypes.Error);

                return false;
            }

            var index = _candles.Count() - 1;

            if (_usdDivergencyFinder.IsTimeInValidRange(_candles[index].Date) == false)
            {
                return false;
            }

            if (context.CurrentPositionDirection== Direction.Down)
            {
                var price = _usdMovingsAnalizer.GetDeviatedPrice(Direction.Down, _downParameters, _options.TakeProfitFactor);
                orderParams = new AgentActionParams { Direction = Direction.Up, Price = price };
            }
            else if (context.CurrentPositionDirection == Direction.Up)
            {
                var price = _usdMovingsAnalizer.GetDeviatedPrice(Direction.Up, _upParameters, _options.TakeProfitFactor);
                orderParams = new AgentActionParams { Direction = Direction.Down, Price = price };
            }

            return (orderParams != null);
        }

        public bool ShouldSetStopLoss(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = null;
            return false;
        }

        public bool ShouldAddToPosition(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = null;
            return false;
        }
    }
}