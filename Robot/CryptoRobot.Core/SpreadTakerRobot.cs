using System;
using System.Linq;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Robot.Core.Classes;
using Robot.Core.Providers;
using Robot.Strategies;

namespace Robot.Core
{
    public class SpreadTakerRobot : IExecutableRobot
    {        
        private bool _isIterateInProgress;
        
        private readonly RobotNames _robotName;
        private readonly Securities _security;

        private readonly RobotState _state;
        private readonly IStrategyAgent _agent;

        private readonly ILogger _logger;
        private readonly RobotExecutionOptions _executionOptions;
        private readonly IQuikDataProvider _quikDataProvider;
        private readonly IRobotOperationTimeScheduler _robotOperationTimeScheduler;        
        private readonly TradeOperationExecutor _operationExecutor;
        private readonly RobotOrdersSynchronizer _ordersSynchronizer;


        public SpreadTakerRobot(RobotNames robotName,
                                Securities firstSecurity,
                                Securities secondSecurity,                        
                                RobotState state,
                                IStrategyAgent agent,
                                IQuikTradeExecutor quikTradeExecutor,
                                IQuikDataProvider quikDataProvider,
                                IRobotOperationTimeScheduler robotOperationTimeScheduler,
                                RobotOrdersSynchronizer ordersSynchronizer,
                                ILogger logger,
                                RobotExecutionOptions executionOptions)
        {
            _robotName = robotName;
            //_security = security;

            _state = state;

            _agent = agent;
            _quikDataProvider = quikDataProvider;

            _robotOperationTimeScheduler = robotOperationTimeScheduler;
            _ordersSynchronizer = ordersSynchronizer;

            _logger = logger;
            _executionOptions = executionOptions;            
            _operationExecutor = new TradeOperationExecutor(_security, _state, quikTradeExecutor);

            _quikDataProvider.LimitedOrdersChanged += LimitedOrdersChanged;
        }

        public RobotNames RobotName
        { get { return _robotName; } }

        public RobotState State => _state;

        public RobotExecutionOptions ExecutionOptions => _executionOptions;

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
            bool shouldClosePosition, shouldOpenPosition;

            UpdateAgent();

            SetupPositionOrders(out shouldClosePosition, out shouldOpenPosition);

            if (_executionOptions.UseTakeProfit)
            {
                SetupTakeProfitOrder(shouldClosePosition);
            }

            SetupStopLossOrder(shouldClosePosition);

            CleanUpOrders(shouldClosePosition, shouldOpenPosition);
        }

        private void UpdateAgent()
        {
            var candles = _quikDataProvider.GetCandles(_security, _executionOptions.OperationalCandlesCount).ToList();

            if (candles.Any() == false)
            {
                _logger.Log("No candles come from QuikDataProvider", LogMessageTypes.Warning);

                return;
            }

            _agent.Update(candles);

            if (_robotOperationTimeScheduler.IsEnoughTimeElapsedToUpdateLongSeries(candles.Last().Date, _agent.GetLongSeriesLastUpdateTime()))
            {
                _agent.UpdateLongSeries(_quikDataProvider.GetCandles(_security, _executionOptions.LongSeriesCandlesCount));
            }
        }

        private void SetupPositionOrders(out bool shouldClosePosition, out bool shouldOpenPosition)
        {
            shouldClosePosition = false;
            shouldOpenPosition = false;

            OrderParams orderParams = null;
            AgentActionParams actionParams;
            RobotOrderTypes? robotOrderType = null;

            var sizeCaclulator = new OrderParamsSizeCaclulator(_executionOptions);

            if (ShouldClose(out actionParams))
            {
                shouldClosePosition = true;
                robotOrderType = RobotOrderTypes.ClosePosition;
                orderParams = sizeCaclulator.Get(RobotOrderTypes.ClosePosition, actionParams, _state.Size);
            }
            else if (_agent.ShouldOpen(GetAgentContext(), out actionParams))
            {
                shouldOpenPosition = true;
                robotOrderType = RobotOrderTypes.OpenPosition;
                orderParams = sizeCaclulator.Get(RobotOrderTypes.OpenPosition, actionParams, _state.Size);
            }

            if (orderParams != null && orderParams.Size != 0)
            {
                if (_robotOperationTimeScheduler.IsEnoughTimeElapsedToReplaceOrder(_state.Get(robotOrderType.Value)))
                {
                    _operationExecutor.UpdateOrder(robotOrderType.Value, orderParams);
                }
            }
        }

        private AgentContext GetAgentContext()
        {
            return new AgentContext()
            {
                CurrentPositionDirection = _state.Direction,
                LastCompletedOrderType = _state.LastCompletedOrderType,
                LastCompletedOrder = _state.LastCompletedOrder
            };
        }

        private void SetupTakeProfitOrder(bool shouldClosePosition)
        {
            if (_state.State != PositionStates.Open) return;

            var sizeCaclulator = new OrderParamsSizeCaclulator(_executionOptions);

            if (shouldClosePosition == false)
            {
                if (_state.HasPositionOrders()) return;

                AgentActionParams actionParams;

                if (IsFull())
                {
                    if (_robotOperationTimeScheduler.IsEnoughTimeElapsedToReplaceOrder(_state.Get(RobotOrderTypes.TakeProfit)) &&
                        _agent.ShouldTakeProfit(GetAgentContext(), out actionParams))
                    {
                        var orderParams = sizeCaclulator.Get(RobotOrderTypes.TakeProfit, actionParams, _state.Size);
                        _operationExecutor.UpdateOrder(RobotOrderTypes.TakeProfit, orderParams);
                    }
                }
                else
                {
                    if (
                        _robotOperationTimeScheduler.IsEnoughTimeElapsedToReplaceOrder(_state.Get(RobotOrderTypes.AddToPosition)) &&
                                                                                       _agent.ShouldAddToPosition(GetAgentContext(), out actionParams))
                    {
                        var orderParams = sizeCaclulator.Get(RobotOrderTypes.AddToPosition, actionParams, _state.Size);
                        _operationExecutor.UpdateOrder(RobotOrderTypes.AddToPosition, orderParams);
                    }
                }
            }            
        }

        private void SetupStopLossOrder(bool shouldClosePosition)
        {
            if (_state.State != PositionStates.Open) return;

            var sizeCaclulator = new OrderParamsSizeCaclulator(_executionOptions);

            if (shouldClosePosition == false)
            {
                if (_state.HasPositionOrders()) return;

                AgentActionParams actionParams;

                if (_robotOperationTimeScheduler.IsEnoughTimeElapsedToReplaceOrder(_state.Get(RobotOrderTypes.StopLoss)) &&
                    _agent.ShouldSetStopLoss(GetAgentContext(), out actionParams))
                {
                    var orderParams = sizeCaclulator.Get(RobotOrderTypes.StopLoss, actionParams, _state.Size);

                    _operationExecutor.UpdateOrder(RobotOrderTypes.StopLoss, orderParams);
                }
            }
        }

        private void CleanUpOrders(bool shouldClosePosition, bool shouldOpenPosition)
        {
            if(shouldClosePosition)
            {
                /* вспомогательные ордера должны быть сняты */                
                _operationExecutor.KillOrder(RobotOrderTypes.StopLoss);
                _operationExecutor.KillOrder(RobotOrderTypes.TakeProfit);
                _operationExecutor.KillOrder(RobotOrderTypes.AddToPosition);    
            }

            if (shouldOpenPosition == false)
            {
                _operationExecutor.KillOrder(RobotOrderTypes.OpenPosition);                
            }
        }

        private bool IsFull()
        {
            return Math.Abs(_state.Size) >= Math.Abs(_executionOptions.MaxPositionSize);
        }

        private bool ShouldClose(out AgentActionParams actionParams)
        {
            actionParams = null;

            return _state.State == PositionStates.Open &&
                   _agent.ShouldClose(GetAgentContext(), out actionParams) &&
                   (actionParams.Direction.HasValue == false || _state.Direction == actionParams.Direction);
        }

        void LimitedOrdersChanged(object sender, EventArgs e)
        {
            _ordersSynchronizer.Synchronize(_state, _quikDataProvider.GetLimitedOrders().ToList());
            
            HandleOrdersExecution();
        }

        /// <summary>
        /// Этого здесь быть не должно. Должно быть в отдельном классе. 
        /// плюс в обаботчике события это выглядить некрасиво. 
        /// Часть уходит в state, часть здесь. Плюс это должно выполниться позже. 
        /// Вообщем нужно переделать!
        /// </summary>
        private void HandleOrdersExecution()
        {
            if (_state.State == PositionStates.NotOpen)
            {
                /* Исполнился ордер на закрытие или сработал стоп лосс, позиция закрыта, 
                   поэтому  вспомогательные ордера должны быть сняты */
                _operationExecutor.KillOrder(RobotOrderTypes.StopLoss);
                _operationExecutor.KillOrder(RobotOrderTypes.TakeProfit);
                _operationExecutor.KillOrder(RobotOrderTypes.AddToPosition);
            }
        }
    }
}