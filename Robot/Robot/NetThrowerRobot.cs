using System;
using System.Linq;
using Common.Classes.General;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using Common.StrategiesOptions;
using Robot.Core.MarketDataProviders;
using Robot.Strategies;

namespace Robot.Core
{
    public class NetThrowerRobot : IExecutableRobot
    {
        private bool _isIterateInProgress;

        private readonly RobotState _state;
        private readonly NetThrowerOptions _strategyOptions;

        private readonly ILogger _logger;
        private readonly IMarketDataProvider _marketDataProvider;
        private readonly IRobotOrderScheduler _robotOrderScheduler;
        private readonly NetThrowerRobotOperationExecutor _operationExecutor;
        private readonly RobotStateManager _stateManager;
        private readonly RobotIterationState _iterationState = new RobotIterationState();
        private readonly NetThrowerOrdersPreparer _ordersPreparer;

        public NetThrowerRobot(string robotId,
                               string symbol,
                                  RobotState state,
                                  NetThrowerOptions strategyOptions,
                                  IMarketDataProvider marketDataProvider,
                                  IMarketOperationExecutor marketOperationExecutor,
                                  ILogger logger)
        {
            RobotId = robotId;

            Symbol = symbol;

            _state = state;

            _marketDataProvider = marketDataProvider;


            _logger = logger;
            _strategyOptions = strategyOptions;
            _stateManager = new RobotStateManager(_state, _logger);
            _ordersPreparer = new NetThrowerOrdersPreparer(Symbol, _state, _marketDataProvider, _strategyOptions);
            _operationExecutor = new NetThrowerRobotOperationExecutor(Symbol, _stateManager, marketOperationExecutor, _logger);

            _stateManager.StateChanged += StateManagerOnStateChanged;
            _marketDataProvider.TradesChanged += _marketDataProvider_TradesChanged; ;
            _marketDataProvider.OrdersChanged += MarketDataProviderOnOrdersChanged;
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
            SetupOrders();
        }

        private void SetupOrders()
        {
            var orders = _ordersPreparer.GetOrders();
            _operationExecutor.UpdateOrders(orders);
        }

        private bool IsFull()
        {
            return _state.Size >= _strategyOptions.MaxPositionSize || _state.Size <= _strategyOptions.MinPositionSize;
        }


        private void StateManagerOnStateChanged(object sender, EventArgs eventArgs)
        {
            /* Исполнился ордер на закрытие или сработал стоп лосс, позиция закрыта, 
               поэтому  вспомогательные ордера должны быть сняты */
        }

        void MarketDataProviderOnOrdersChanged(object sender, EventArgs e)
        {
            _stateManager.Synchronize(_marketDataProvider.GetLimitOrders().ToList());
            //_stateManager.CleanOrders();
        }

        private void _marketDataProvider_TradesChanged(object sender, EventArgs e)
        {
            _stateManager.Synchronize(_marketDataProvider.GetTrades().ToList());
        }

    }
}