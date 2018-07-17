using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Caclulators;
using Common.Classes;
using Common.Classes.Analizers;
using Common.Classes.General;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Finders;
using Common.Robots.Common;
using Common.StrategiesOptions;

namespace Robot.Strategies.Agents
{    
    public class TrendFollowerStrategyAgent : IStrategyAgent
    {
        public ILongRangeDataProvider LongRangeDataProvider;
        private List<Candle> _candles = new List<Candle>();
        private readonly StrategyOptions _options;
        private readonly ILogger _logger;
        private MovingsDivergencyFinder _usdDivergencyFinder;
        private MovingsAnalizer _movingsAnalizer;
        private DistributionParameters _upParameters;
        private DistributionParameters _downParameters;
        private List<IntersectionSegment> _lastIntersectionSegments;

        public TrendFollowerStrategyAgent(StrategyOptions options, 
                                          ILongRangeDataProvider longRangeDataProvider,
                                          ILogger logger)
        {           
            _options = options;
            _logger = logger;
            LongRangeDataProvider = longRangeDataProvider;
        }

        public void Update(IEnumerable<Candle> candles)
        {
            _candles = candles.ToList();

            if (_options.SmaPeriod > _candles.Count())
            {
                _logger.Log("Sma period is bigger than number of usd candles", LogMessageTypes.Error);

                return;
            }

            _usdDivergencyFinder = new MovingsDivergencyFinder(_candles, _options.FmaPeriod, _options.SmaPeriod);
            _movingsAnalizer = new MovingsAnalizer(_candles, _options.FmaPeriod, _options.SmaPeriod);
        }

        public void Update(IEnumerable<Candle> firstSeries, IEnumerable<Candle> secondSeries)
        {
            throw new NotImplementedException();
        }
        
        public void UpdateLongSeries(IEnumerable<Candle> series)
        {          
            var lastCandle = _candles.LastOrDefault();

            if (lastCandle == null) return;

            var lastCandleTime = lastCandle.Date;

            LongRangeDataProvider.RecalculateDeviations(series);
            _upParameters = LongRangeDataProvider.GetDeviations(Direction.Buy);
            _downParameters = LongRangeDataProvider.GetDeviations(Direction.Sell);        
        }

        public void UpdateLongSeries(IEnumerable<Candle> firstSeries, IEnumerable<Candle> secondSeries)
        {
            throw new NotImplementedException();
        }
        
        public bool ShouldOpen(AgentContext context, out AgentActionParams actionParams)
        {
            actionParams = null;

            if (_candles.Any() == false)
            {
                _logger.Log("ShouldOpen: candles list is empty", LogMessageTypes.Error);

                return false;
            }

            DivergencyPoint point;
            var index = _candles.Count() - 1;
            var currentCandle = _candles[index];

            if (TradingTimeScheduler.IsLateToOpenPostion(currentCandle.Date))
            {
                return false;
            }

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

                var price = (point.FmaPrice) + add;;
                price = PriceCorrector.GetPrice(price, point.Direction);

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

                if (_usdDivergencyFinder.IsParentDirectionMatch(index, point.Direction) == false)
                {
                   // return false;
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
            var parameters = direction == Direction.Buy ? _upParameters : _downParameters;
            double factor = 0;

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
            var currentCandle = _candles[index];

            var closeUp = false;
            var closeDown = false;

            if (TradingTimeScheduler.IsTimeToExit(currentCandle.Date))
            {
                /* выход по времени должен идти первым */
                orderParams = new AgentActionParams { Price = _candles.Last().Close };

                return true;
            }

            //if (_movingsAnalizer.IsFastBelowSlow())
            //{
            //    closeUp = true;                     
            //}
            //else if (_movingsAnalizer.IsFastAboveSlow())
            //{
            //    closeDown = true;                     
            //}
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
                var direction = (closeDown && closeUp) ? (Direction?)null : (closeUp ? Direction.Buy : Direction.Sell);

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

            //if (context.PositionDirection== Direction.Sell)
            //{
            //    var price = _movingsAnalizer.GetDeviatedPrice(Direction.Sell, _downParameters, _options.TakeProfitFactor);
            //    orderParams = new AgentActionParams { Direction = Direction.Buy, Price = price };
            //}
            //else if (context.PositionDirection == Direction.Buy)
            //{
            //    var price = _movingsAnalizer.GetDeviatedPrice(Direction.Buy, _upParameters, _options.TakeProfitFactor);
            //    orderParams = new AgentActionParams { Direction = Direction.Sell, Price = price };
            //}

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