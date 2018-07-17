using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
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
    public class CryptoTrendFollowerStrategyAgent : IStrategyAgent
    {
        private List<Candle> _candles = new List<Candle>();
        private readonly ILogger _logger;
        private MovingsAnalizer _movingsAnalizer;
        private List<Candle> _hoursCandles;
        private List<Candle> _daysCandles;
        private MovingsAnalizer _hoursMovingsAnalizer;
        private DistributionParameters _upParameters;
        private DistributionParameters _downParameters;
        private readonly CandlesAggregator _hoursCandlesAggregator;
        private readonly CandlesAggregator _daysCandlesAggregator;
        private MovingsAnalizer _daysMovingsAnalizer;
        private CandlesStatistics _candesStatistics;
        private Candle _lastCandleWithHighMarginOffset;
        private List<Candle> _aggregatedCandles6;
        private List<Candle> _aggregatedCandles12;
        private MovingsAnalizer _movingsAnalizer6;
        private MovingsAnalizer _movingsAnalizer12;
        private StrategyOptions _strategyOptions;
        public ILongRangeDataProvider LongRangeDataProvider;
        private PositionCloseAnalyzer _positionCloseAnalyzer;


        public CryptoTrendFollowerStrategyAgent(StrategyOptions strategyOptions,
             ILongRangeDataProvider longRangeDataProvider,
                                          ILogger logger)
        {
            _strategyOptions = strategyOptions;
            LongRangeDataProvider = longRangeDataProvider;
            _logger = logger;
            _hoursCandlesAggregator = new CandlesAggregator(new AggregationSettings()
            {
                AggregationInteral = AggregationInteral.Hour,
                DateTimeType = CandleDateTimeType.StartOfCandle
            });
            _daysCandlesAggregator = new CandlesAggregator(new AggregationSettings()
            {
                AggregationInteral = AggregationInteral.Day,
                DateTimeType = CandleDateTimeType.StartOfCandle
            });

            _positionCloseAnalyzer = new PositionCloseAnalyzer();

            //_strategyOptions = StrategyOptionsFactory.GetByName(StrategyConfigs.Btc5Min);
        }

        public void Update(IEnumerable<Candle> candles)
        {
            _candles = candles.ToList();
            _hoursCandles = _hoursCandlesAggregator.Aggregate(_candles);
            _daysCandles = _daysCandlesAggregator.Aggregate(_candles);
            _aggregatedCandles6 = new CandlesSimpleAggregator(6).Aggregate(_candles);
            _aggregatedCandles12 = new CandlesSimpleAggregator(12).Aggregate(_candles);

            if (_strategyOptions.SmaPeriod > _candles.Count())
            {
                _logger.Log("Sma period is bigger than number of usd candles", LogMessageTypes.Error);

                return;
            }

            _movingsAnalizer6 = new MovingsAnalizer(_aggregatedCandles6, _strategyOptions.FmaPeriod, _strategyOptions.SmaPeriod);
            _movingsAnalizer12 = new MovingsAnalizer(_aggregatedCandles12, _strategyOptions.FmaPeriod, _strategyOptions.SmaPeriod);

            _movingsAnalizer = new MovingsAnalizer(_candles, _strategyOptions.FmaPeriod, _strategyOptions.SmaPeriod);
            _hoursMovingsAnalizer = new MovingsAnalizer(_hoursCandles, _strategyOptions.FmaPeriod, _strategyOptions.SmaPeriod);
            _daysMovingsAnalizer = new MovingsAnalizer(_daysCandles, _strategyOptions.FmaPeriod, _strategyOptions.SmaPeriod);
            _positionCloseAnalyzer.Update(_candles, _movingsAnalizer, _upParameters, _downParameters, _candesStatistics);
        }

        public void Update(IEnumerable<Candle> firstSeries, IEnumerable<Candle> secondSeries)
        {
            throw new NotImplementedException();
        }

        public void UpdateLongSeries(IEnumerable<Candle> series)
        {
            var lastCandle = _candles.LastOrDefault();

            if (lastCandle == null) return;
            var index = series.Count() - 1;

            LongRangeDataProvider.RecalculateDeviations(series);
            LongRangeDataProvider.RecalculateCandlesStatistics(series.TakeRange(index - 120, index));

            _upParameters = LongRangeDataProvider.GetDeviations(Direction.Buy);
            _downParameters = LongRangeDataProvider.GetDeviations(Direction.Sell);
            _candesStatistics = LongRangeDataProvider.GetCandlesStatistics();
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

            var index = _candles.Count - 1;

            var currentCandle = _candles[index];
            var prevCandle = _candles[index - 1];

            var isPriceAboveFast = _candles[index].Close >= _movingsAnalizer.GetFma(index);
            //var areTwoMeanPricesAboveFast = currentCandle.Get(PriceTypes.Mean) > _movingsAnalizer.GetFma(index) &&
            //                                prevCandle.Get(PriceTypes.Mean) > _movingsAnalizer.GetFma(index - 1);

            var deviationToOpen = _strategyOptions.DeviationToOpen;

            var isPositionAllowedByOffset = true;
            if (_lastCandleWithHighMarginOffset != null &&
                GetNumberOfCandlesByTimespan(currentCandle.Date - _lastCandleWithHighMarginOffset.Date) < _strategyOptions.NumberOfCandlesAfterMarginClose)
            {
                _logger.Log("Is position allowed by offset: false. Offset by candles is not reached", LogMessageTypes.Info);
                //isPositionAllowedByOffset = false;
                //deviationToOpen = deviationToOpen/2;
            }


            if ((_movingsAnalizer.IsFastAboveSlow()) && isPriceAboveFast && isPositionAllowedByOffset &&
                (_hoursMovingsAnalizer.IsFastAboveSlow() || !_strategyOptions.Smoothers.Hour) &&
                (_movingsAnalizer6.IsFastAboveSlow() || !_strategyOptions.Smoothers.Aggregation6) &&
                 (_movingsAnalizer12.IsFastAboveSlow() || !_strategyOptions.Smoothers.Aggregation12) &&
                (_daysMovingsAnalizer.IsFastAboveSlow() || !_strategyOptions.Smoothers.Day))
            {
                var add = CalculateAddition(Direction.Buy);

                var fmaValue = _movingsAnalizer.GetFma(index);
                var smaValue = _movingsAnalizer.GetSma(index);
                var price = fmaValue + add;
                //price = Math.Min(price, smaValue * (1 + _strategyOptions.DeviationToOpen));
                price = smaValue * (1 + deviationToOpen);
                price = Math.Min(price, (currentCandle.Close + fmaValue)/2);               
                //  price = PriceCorrector.GetPrice(price, direction);
                actionParams = new AgentActionParams { Direction = Direction.Buy, Price = price };
            }
            else if (_movingsAnalizer.IsFastBelowSlow() && !isPriceAboveFast && isPositionAllowedByOffset &&
                (_hoursMovingsAnalizer.IsFastBelowSlow() || !_strategyOptions.Smoothers.Hour) &&
                 (_movingsAnalizer6.IsFastBelowSlow() || !_strategyOptions.Smoothers.Aggregation6) &&
                 (_movingsAnalizer12.IsFastBelowSlow() || !_strategyOptions.Smoothers.Aggregation12) &&
                (_daysMovingsAnalizer.IsFastBelowSlow() || !_strategyOptions.Smoothers.Day))
            {
                var add = CalculateAddition(Direction.Sell);

                var fmaValue = _movingsAnalizer.GetFma(index);
                var price = fmaValue + add;
                var smaValue = _movingsAnalizer.GetSma(index);
                //price = Math.Max(price, smaValue * (1 - _strategyOptions.DeviationToOpen)); 
                //It should depend on volatility!
                price = smaValue * (1 - deviationToOpen);
                price = Math.Max(price, (currentCandle.Close + fmaValue) / 2);                
                //price = PriceCorrector.GetPrice(price, direction);
                actionParams = new AgentActionParams { Direction = Direction.Sell, Price = price };
            }

            //if (_usdDivergencyFinder.IsDivergencyPoint(index, out point))
            //{                
            //    var add = CalculateAddition(point.Direction);

            //    var price = (point.FmaPrice) + add;;
            //    price = PriceCorrector.GetPrice(price, point.Direction);

            //    actionParams = new AgentActionParams { Direction = point.Direction, Price = price };
            //}

            if (actionParams != null)
            {
                /* Нужно проверить, что на этой же свече нет сигнала к закрытию позиции, 
                   Иначе может получится большое количество пустых сделок */
                AgentActionParams testForCloseParams;
                if (ShouldClose(context, out testForCloseParams))
                {
                    if (actionParams.Direction == testForCloseParams.Direction)
                    {
                        _logger.Log("ShouldClose: false. Sygnal to close", LogMessageTypes.Info);
                        return false;
                    }
                }
            }


            var logText = GetTextToLog(index, actionParams);

            _logger.Log(logText, LogMessageTypes.Info);

            return actionParams != null;
        }

        private string GetTextToLog(int index, AgentActionParams actionParams)
        {
            var candle = _candles[index];
            var shouldOpen = actionParams != null;
            var builder = new StringBuilder();
            builder.Append(candle.Date.ToString("yyyy MMMM dd HH:mm"));
            builder.Append(" : ");
            builder.AppendFormat("should open: {0}", shouldOpen);
            if (shouldOpen)
            {
                builder.Append(" : ");
                builder.AppendFormat("direction: {0}", actionParams.Direction);
                builder.Append(" : ");
                builder.AppendFormat("price: {0}", actionParams.Price);
            }
            return builder.ToString();
        }

        private int GetNumberOfCandlesByTimespan(TimeSpan timeSpan)
        {
            var totalMinutes = timeSpan.TotalMinutes;
            return (int)totalMinutes / (int)_strategyOptions.TimeFrame;
        }

        private double CalculateAddition(Direction direction)
        {
            var parameters = direction == Direction.Buy ? _upParameters : _downParameters;
            double factor = 0;

            var add = parameters.Median - factor * parameters.MedianSigma;

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
            var prevCandle = _candles[index - 1];

            var closeUp = false;
            var closeDown = false;

            var fmaValue = _movingsAnalizer.GetFma(index);
            var smaValue = _movingsAnalizer.GetSma(index);
            var prevSmaValue = _movingsAnalizer.GetSma(index - 1);

            var deviationsFinder = new DeviationsFromMovingFinder(_candles, _strategyOptions.FmaPeriod);

            var upDeviations = new StatisticsCalculator(deviationsFinder.FindFor(Direction.Buy)).GetParameters();
            var downDeviations = new StatisticsCalculator(deviationsFinder.FindFor(Direction.Sell)).GetParameters();

            //var lastThreeCandlesDownDeviation =
            //    ((_movingsAnalizer.GetSma(index) - _candles[index].Low) +
            //    (_movingsAnalizer.GetSma(index - 1) - _candles[index - 1].Low) +
            //    (_movingsAnalizer.GetSma(index - 2) - _candles[index - 2].Low))/3;

            var takeProfitUpDeviation = Math.Max(_upParameters.Percentile90, upDeviations.Percentile90);
            var takeProfitDownDeviation = Math.Max(_downParameters.Percentile90, _downParameters.Percentile90);
         //  var takeProfitDownDeviation = 2 * _downParameters.ThirdQuartile;//Math.Max(, lastThreeCandlesDownDeviation);


            //var lastThreeCandlesUpDeviation =
            //   -1 * ((_movingsAnalizer.GetSma(index) - _candles[index].High) +
            //    (_movingsAnalizer.GetSma(index - 1) - _candles[index - 1].High) +
            //    (_movingsAnalizer.GetSma(index - 2) - _candles[index - 2].High))/3;

            
           // var takeProfitUpDeviation = 2 * _upParameters.ThirdQuartile;// Math.Max(_downParameters.ThirdQuartile, lastThreeCandlesUpDeviation);

            var price = 0.0;

            var deviationToClose = _strategyOptions.DeviationToClose;

            if (_movingsAnalizer.IsFastBelowSlow())
            {
                closeUp = true;
            //    price = (fmaValue + 4 * currentCandle.Close) / 5;

            }
            else if (_movingsAnalizer.IsFastAboveSlow())
            {
                closeDown = true;
                //price = currentCandle.Close * (1 - _strategyOptions.DeviationToTakeProfit/2);
              //  price = (4 * currentCandle.Close + fmaValue) / 5;
            }

            var takeProfitToCloseDown = _strategyOptions.TakeProfitToClose;
            // takeProfitToCloseDown = Math.Max(0.75*_downParameters.Percentile99/smaValue, _strategyOptions.TakeProfitToClose);
            var takeProfitToCloseUp = _strategyOptions.TakeProfitToClose;
            //takeProfitToCloseUp = Math.Max(0.75 * _upParameters.Percentile99/smaValue, _strategyOptions.TakeProfitToClose);            

            if (_movingsAnalizer.IsFastBelowSlow() && context.PositionDirection == Direction.Sell)
            {
                if (context.PositionDirection == Direction.Sell)
                {
                    var shouldTakeProfit = _strategyOptions.UseFoundDeviationsToTakeProfit
                      ? (fmaValue - currentCandle.Close) >= takeProfitDownDeviation
                      : (smaValue - currentCandle.Close) / smaValue >= takeProfitToCloseDown;

                    if (shouldTakeProfit)
                    {
                        closeDown = true;

                        price = currentCandle.Close * (1 - _strategyOptions.DeviationToTakeProfit);

                        _lastCandleWithHighMarginOffset = currentCandle;
                        _logger.Log("CloseDown: LastClosedByMargin: " + currentCandle.Date.ToString("s"),
                            LogMessageTypes.Info);
                    }

                    if (_strategyOptions.UseDeviationToClose &&
                        currentCandle.Close > smaValue &&
                        (currentCandle.Close - fmaValue) / fmaValue >= _strategyOptions.DeviationToClose)
                    {
                        closeDown = true;
                    }

                    //if (_positionCloseAnalyzer.AreTwoCandlesClosedAboveMoving(index))
                    //{
                    //    closeDown = true;
                    //    price = currentCandle.Close * (1 - _strategyOptions.DeviationToTakeProfit);
                    //}

                    if (_strategyOptions.UseCandlesCrossingMovings &&
                        _positionCloseAnalyzer.IsOneorTwoCandlesCrossingMovings(index))
                    {
                        closeDown = true;
                        //_lastCandleWithHighMarginOffset = currentCandle;
                        //_lastCandleWithHighMarginOffset = currentCandle;
                        _logger.Log("CloseDown: IsOneorTwoCandlesCrossingMovings: " + currentCandle.Date.ToString("s"),
                            LogMessageTypes.Info);

                        //price = currentCandle.Close * (1 - _strategyOptions.DeviationToTakeProfit);
                     //   price = (fmaValue + 4 * currentCandle.Close) / 5;
                    }

                    if (_strategyOptions.UseTwoCandlesClosedAboveOfBelowMoving &&
                       _positionCloseAnalyzer.AreTwoCandlesClosedAboveOrBelowMoving(index))
                    {
//                        closeDown = true;
                        //_lastCandleWithHighMarginOffset = currentCandle;
                        //_lastCandleWithHighMarginOffset = currentCandle;
                        _logger.Log("CloseDown: UseTwoCandlesClosedAboveOfBelowMoving: " + currentCandle.Date.ToString("s"),
                            LogMessageTypes.Info);
                    }
                }
            }

            if (_movingsAnalizer.IsFastAboveSlow() && context.PositionDirection == Direction.Buy)
            {
                if (context.PositionDirection == Direction.Buy)
                {
                    var shouldTakeProfit = _strategyOptions.UseFoundDeviationsToTakeProfit
                        ? (currentCandle.Close - fmaValue) >= takeProfitUpDeviation
                        : (currentCandle.Close - smaValue) / smaValue >= takeProfitToCloseUp;

                    if (shouldTakeProfit)
                    {
                        closeUp = true;
                        _lastCandleWithHighMarginOffset = currentCandle;
                        _logger.Log("CloseUp: LastClosedByMargin: " + currentCandle.Date.ToString("s"),
                            LogMessageTypes.Info);

                        price = currentCandle.Close * (1 + _strategyOptions.DeviationToTakeProfit);
                    }

                    if (_strategyOptions.UseDeviationToClose &&
                        smaValue > currentCandle.Close &&
                        (fmaValue - currentCandle.Close) / fmaValue >= _strategyOptions.DeviationToClose)
                    {
                        closeUp = true;
                    }

                    //if (_positionCloseAnalyzer.AreTwoCandlesClosedBelowMoving(index))
                    //{
                    //    closeUp = true;
                    //    price = currentCandle.Close * (1 + _strategyOptions.DeviationToTakeProfit);
                    //}

                    if (_strategyOptions.UseCandlesCrossingMovings &&
                        _positionCloseAnalyzer.IsOneorTwoCandlesCrossingMovings(index))
                    {
                        closeUp = true;
                        // _lastCandleWithHighMarginOffset = currentCandle;
                        //_lastCandleWithHighMarginOffset = currentCandle;
                        _logger.Log("CloseUp: IsOneorTwoCandlesCrossingMovings: " + currentCandle.Date.ToString("s"),
                            LogMessageTypes.Info);

                        //  price = currentCandle.Close * (1 + _strategyOptions.DeviationToTakeProfit);
                     //   price = (fmaValue + 4 * currentCandle.Close) / 5;
                    }

                    if (_strategyOptions.UseTwoCandlesClosedAboveOfBelowMoving &&
                       _positionCloseAnalyzer.AreTwoCandlesClosedAboveOrBelowMoving(index))
                    {
                       // closeUp = true;
                        //_lastCandleWithHighMarginOffset = currentCandle;
                        //_lastCandleWithHighMarginOffset = currentCandle;
                        _logger.Log("CloseDown: UseTwoCandlesClosedAboveOfBelowMoving: " + currentCandle.Date.ToString("s"),
                            LogMessageTypes.Info);
                    }

                }
            }

            if (closeDown || closeUp)
            {
                Direction? direction = closeUp ? Direction.Buy : Direction.Sell;

                if (closeUp && closeDown)
                    direction = null;

                //                orderParams = new AgentActionParams { Direction = direction, Price = 0.0 };
                orderParams = new AgentActionParams { Direction = direction, Price = price };

                return true;

            }

            return false;
        }

        private bool AreNumberClosedAboveFast(int index, int number)
        {
            var result = true;
            for (int i = index; i > index - number; i--)
            {
                if (i > _candles.Count || i < 0) return false;

                var candle = _candles[i];
                var fma = _movingsAnalizer.GetFma(i);
                if (candle.Get(PriceTypes.Close) < fma)
                    result = false;
                if (!result) break;
            }

            return result;
        }

        private bool AreNumberClosedBelowFast(int index, int number)
        {
            var result = true;
            for (int i = index; i > index - number; i--)
            {
                if (i > _candles.Count || i < 0) return false;

                var candle = _candles[i];
                var fma = _movingsAnalizer.GetFma(i);
                if (candle.Get(PriceTypes.Close) > fma)
                    result = false;
                if (!result) break;
            }

            return result;
        }

        public bool ShouldTakeProfit(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = null;
            return false;
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