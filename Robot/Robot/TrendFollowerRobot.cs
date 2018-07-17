using System;
using System.Linq;
using Common;
using Common.Classes.General;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using Common.StrategiesOptions;
using Robot.Core.Classes;
using Robot.Core.MarketDataProviders;
using Robot.Strategies;

namespace Robot.Core
{
    public class TrendFollowerRobot : IExecutableRobot
    {
        private bool _isIterateInProgress;

        private readonly Securities _security;

        private readonly RobotStateOld _stateOld;
        private readonly StrategyOptions _strategyOptions;
        private readonly IStrategyAgent _strategyAgent;

        private readonly ILogger _logger;
        private readonly IMarketDataProvider _marketDataProvider;
        private readonly IRobotOrderScheduler _robotOrderScheduler;
        private readonly RobotOperationExecutor _operationExecutor;
        private readonly RobotStateManager _stateManager;
        private readonly RobotIterationState _iterationState = new RobotIterationState();
        private Candle _longSeriesLastCandle;

        public TrendFollowerRobot(string robotId,
                                  string symbol,
                                  RobotStateOld stateOld,
                                  StrategyOptions strategyOptions,
                                  IStrategyAgent strategyAgent,
                                  IMarketDataProvider marketDataProvider,
                                  IMarketOperationExecutor marketOperationExecutor,
                                  ILogger logger)
        {
            RobotId = robotId;

            Symbol = symbol;


            _stateOld = stateOld;

            _strategyAgent = strategyAgent;
            _marketDataProvider = marketDataProvider;

            

            _logger = logger;
            _strategyOptions = strategyOptions;
            _stateManager = new RobotStateManager(_stateOld);
            _robotOrderScheduler = new RobotOrderScheduler(_stateManager);
            _operationExecutor = new RobotOperationExecutor(_security, _stateManager, marketOperationExecutor);

            _stateManager.StateChanged += StateManagerOnStateChanged;
            _marketDataProvider.LimitOrdersChanged += MarketDataProviderOnOrdersChanged;
        }

        public string RobotId { get; }

        public string Symbol { get; }

        public RobotStateManager StateManager => _stateManager;

        public void Iterate()
        {
            if (_isIterateInProgress == false)
            {
                _isIterateInProgress = true;

                try
                {
                    IterateInternal();
                }
                catch (Exception)
                {
                    _isIterateInProgress = false;
                    throw;
                }

                _isIterateInProgress = false;
            }
        }

        private void IterateInternal()
        {
            _iterationState.Reset();

            UpdateStrategyAgent();

            SetupOpenCloseOrders();

            if (_strategyOptions.UseTakeProfit)
            {
                SetupTakeProfitOrder();
            }

            if (_strategyOptions.UseStopLoss)
            {
                SetupStopLossOrder();
            }

            CleanUpOrders();
        }

        private void UpdateStrategyAgent()
        {
            var candles = _marketDataProvider.GetCandles(_security, _strategyOptions.NumberOfCandles).ToList();

            if (candles.Any() == false)
            {
                _logger.Log("No candles come from marketDataProvider", LogMessageTypes.Warning);
                return;
            }

            _strategyAgent.Update(candles);

            if (_longSeriesLastCandle == null ||                
                _robotOrderScheduler.ShouldUpdateLongSeries(candles.Last(), _longSeriesLastCandle))
            {
                candles = _marketDataProvider.GetCandles(_security, _strategyOptions.LongNumberOfCandles).ToList();

                _longSeriesLastCandle = candles.Last();
                _strategyAgent.UpdateLongSeries(candles);

            }
        }

        private AgentContext GetAgentContext()
        {
            return new AgentContext()
            {
                PositionDirection = _stateOld.Direction,
            };
        }

        private void SetupOpenCloseOrders()
        {
            OrderParams orderParams = null;
            AgentActionParams actionParams;
            RobotOrderTypes? robotOrderType = null;

            var sizeCalculator = new OrderParamsSizeCaclulator(_strategyOptions);

            if (ShouldClose(out actionParams))
            {
                _iterationState.ShouldClose = true;

                robotOrderType = RobotOrderTypes.ClosePosition;
                orderParams = sizeCalculator.Get(RobotOrderTypes.ClosePosition, actionParams, _stateOld.Size);
            }
            else if (_strategyAgent.ShouldOpen(GetAgentContext(), out actionParams))
            {
                _iterationState.ShouldOpen = true;

                robotOrderType = RobotOrderTypes.OpenPosition;
                orderParams = sizeCalculator.Get(RobotOrderTypes.OpenPosition, actionParams, _stateOld.Size);
            }

            if (orderParams != null && orderParams.Size != 0)
            {
                if (_robotOrderScheduler.ShouldReplace(robotOrderType.Value, orderParams))
                {
                    _operationExecutor.SendOrder(robotOrderType.Value, orderParams);
                }
            }
        }

        private void SetupTakeProfitOrder()
        {
            if (_stateOld.HasPositionOrders()) return;
            if (_iterationState.ShouldClose) return;
            if (_stateOld.Size == 0) return;

            var sizeCaclulator = new OrderParamsSizeCaclulator(_strategyOptions);

            AgentActionParams actionParams;

            if (IsFull())
            {
                _strategyAgent.ShouldTakeProfit(GetAgentContext(), out actionParams);
                var orderParams = sizeCaclulator.Get(RobotOrderTypes.TakeProfit, actionParams, _stateOld.Size);

                if (_robotOrderScheduler.ShouldReplace(RobotOrderTypes.TakeProfit, orderParams))
                {                    
                    _operationExecutor.SendOrder(RobotOrderTypes.TakeProfit, orderParams);
                }
            }
            else
            {
                _strategyAgent.ShouldAddToPosition(GetAgentContext(), out actionParams);
                var orderParams = sizeCaclulator.Get(RobotOrderTypes.AddToPosition, actionParams, _stateOld.Size);
                if (_robotOrderScheduler.ShouldReplace(RobotOrderTypes.AddToPosition, orderParams))                                                                                  
                {
                    
                    _operationExecutor.SendOrder(RobotOrderTypes.AddToPosition, orderParams);
                }
            }

        }

        private void SetupStopLossOrder()
        {
            if (_stateOld.Size == 0) return;

            var sizeCaclulator = new OrderParamsSizeCaclulator(_strategyOptions);

            if (_iterationState.ShouldClose == false)
            {
                if (_stateOld.HasPositionOrders()) return;

                AgentActionParams actionParams;

                _strategyAgent.ShouldSetStopLoss(GetAgentContext(), out actionParams);
                var orderParams = sizeCaclulator.Get(RobotOrderTypes.StopLoss, actionParams, _stateOld.Size);

                if (_robotOrderScheduler.ShouldReplace(RobotOrderTypes.StopLoss, orderParams))
                {
                    _operationExecutor.SendOrder(RobotOrderTypes.StopLoss, orderParams);
                }
            }
        }

        private void CleanUpOrders()
        {
            if (_iterationState.ShouldClose)
            {
                /* вспомогательные ордера должны быть сняты */
                _operationExecutor.WithdrawOrder(RobotOrderTypes.StopLoss);
                _operationExecutor.WithdrawOrder(RobotOrderTypes.TakeProfit);
                _operationExecutor.WithdrawOrder(RobotOrderTypes.AddToPosition);
            }

            if (_iterationState.ShouldOpen == false)
            {
                _operationExecutor.WithdrawOrder(RobotOrderTypes.OpenPosition);
            }
        }

        private bool IsFull()
        {
            return Math.Abs(_stateOld.Size) >= Math.Abs(_strategyOptions.MaxPositionSize);
        }

        private bool ShouldClose(out AgentActionParams actionParams)
        {
            actionParams = null;

            return _stateOld.Size != 0 &&
                   _strategyAgent.ShouldClose(GetAgentContext(), out actionParams)
            && (actionParams.Direction.HasValue == false || _stateOld.Direction == actionParams.Direction);
        }

        private void StateManagerOnStateChanged(object sender, EventArgs eventArgs)
        {
            /* Исполнился ордер на закрытие или сработал стоп лосс, позиция закрыта, 
               поэтому  вспомогательные ордера должны быть сняты */

            if (_stateOld.Size == 0)
            {
                WithdrawAllOrders();
            }
        }

        void MarketDataProviderOnOrdersChanged(object sender, EventArgs e)
        {
            _stateManager.SynchronizeOrders(_marketDataProvider.GetLimitOrders().ToList());
            _stateManager.CleanOrders();
        }

        private void WithdrawAllOrders()
        {
            _operationExecutor.WithdrawOrder(RobotOrderTypes.StopLoss);
            _operationExecutor.WithdrawOrder(RobotOrderTypes.TakeProfit);
            _operationExecutor.WithdrawOrder(RobotOrderTypes.AddToPosition);
        }

    }
}