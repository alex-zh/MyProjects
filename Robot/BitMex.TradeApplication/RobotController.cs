using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BitMex.Robot.DataLayer.Repositories;
using BitMex.TradeApplication.Classes;
using Common.Classes;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Classes.StrategyTester;
using Common.Robots.Common;
using Common.StrategiesOptions;
using Robot.Core;
using Robot.Core.MarketDataProviders;
using Robot.Strategies.Agents;
using Robot.TradeApplication;
using RobotViewControl = BitMex.TradeApplication.Controls.RobotViewControl;

namespace BitMex.TradeApplication
{
    class RobotController
    {
        private readonly AutoResetEvent _robotViewUpdaterResetEvent = new AutoResetEvent(false);

        private readonly RobotViewControl _robotView;
        private ILogger _logger = LoggerKeeper.Get();
        private readonly IRobotExecutor _robotExecutor = new RobotExecutor(LoggerKeeper.Get());
        private readonly RobotRepository _robotRepository = new RobotRepository();
        private readonly RobotTradeRepository _robotTradeRepository = new RobotTradeRepository();

        private IMarketDataProvider _marketDataProvider;
        private IMarketOperationExecutor _marketOperationExecutor;
        private string _robotId;
        private string _symbol;
        private RobotState _robotState;
        private RobotInfo _robotInfo;
        private IExecutableRobot _robot;
        private IStrategyOptions _strategyConfig;

        public RobotController(RobotViewControl robotView)
        {
            _robotView = robotView;
        }

        public bool IsExecutionStarted => _robotExecutor.IsExecutionStarted;

        public void Init(string robotId, string symbol)
        {
            _robotId = robotId;
            _symbol = symbol;
            _strategyConfig = StrategyOptionsFactory.GetForRobot(_robotId);

            _robotInfo = _robotRepository.GetById(_robotId);
            _robotState = _robotRepository.GetStateByRobotId(_robotId);

            _robotView.RobotMode = _robotInfo.Mode;
            _robotView.BindRobotState(_robotState);

            if (_robotInfo.Status == RobotOperationalStatuses.Started)
            {
                _robotView.ShowRobotIsStartedWarning();
            }

            _marketDataProvider = MarketDataProviderKeeper.Get();
            _marketOperationExecutor = MarketOperationExecutorKeeper.Get();

            Task.Factory.StartNew(UpdateRobotView, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent).LogExceptions(_logger);
        }
        
            
        public void Start()
        {
            var robotMode = _robotView.RobotMode;
            var strategyOptions = _robotView.SelectedStrategyOptions;

            _logger = LoggerKeeper.GetFor(_robotId);
            
            _robot = new NetThrowerRobot(_robotId, _symbol, _robotState, (NetThrowerOptions) strategyOptions, _marketDataProvider, _marketOperationExecutor, _logger); //RobotFactory.CreateTrendFollower(robotMode, _robotId, _symbol, _robotStateOld, _logger);

            Task.Factory.StartNew(() => _robotExecutor.Start(_robot),
                                  TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent)
                        .LogExceptions(_logger);


            _robot.StateManager.StateChanged += StateManagerOnStateChanged;

            _robotInfo.Mode = robotMode;
            _robotInfo.Status = RobotOperationalStatuses.Started;

            _robotRepository.Update(_robotInfo);

            _robotView.SetStopLabel();
            _robotView.ShowRobotIsStartedWarning(false);
            _robotView.ShowRobotIsStartedWarning(false);
        }


        public void Stop()
        {
            _robotExecutor.Stop();

            _robotInfo.Status = RobotOperationalStatuses.NotStarted;
            _robotRepository.Update(_robotInfo);

            _robotView.SetStartLabel();
        }


        void StateManagerOnStateChanged(object sender, EventArgs e)
        {
            _robotRepository.Update(_robotInfo.RobotId, _robotState);

            _robotView.BindRobotState(_robotState);
        }

        //private void StateManagerOnOrdersCompleted(object sender, OrdersCompletedEventArgs eventArgs)
        //{
        //    Task.Factory.StartNew(() => new TradesUpdator(_robotId, _robotRepository, _robotTradeRepository, _marketDataProvider, _logger).Update(),
        //                          TaskCreationOptions.AttachedToParent).LogExceptions(_logger);
        //}


        private void UpdateRobotView()
        {
            try
            {
                while (true)
                {
                    _robotViewUpdaterResetEvent.WaitOne(1000);

                    BindConnectionState();
                    UpdateCompletedTrades();
                    UpdateActiveOrders();
                    UpdateLastIterationTime();
                }
            }
            catch (TaskCanceledException e)
            {
                _logger.Log($"Error while updating robot view: {e}", LogMessageTypes.Error);
            }
        }

        private void UpdateLastIterationTime()
        {
            var lastIterationTime = _robotExecutor.GetLastIterationTime();
            var isOffsetExceeded = (DateTime.Now - lastIterationTime).TotalSeconds > 1.5 * RobotSettings.IterationOffset;

            _robotView.BindLastIterationTime(lastIterationTime, isOffsetExceeded);
        }

        private void UpdateActiveOrders()
        {
            var activeOrders = _marketDataProvider.GetLimitOrders().Where(x => x.Status == OrderStatuses.Active).ToList();
            _robotView.BindOrders(activeOrders);
        }

        private void UpdateCompletedTrades()
        {
            var trades = _robotTradeRepository.GetAll(_robotId).ToList();
            _robotView.BindTrades(trades);

            if (trades.Any())
            {                
                var dataAnalizer = new TradeDataAnalyzer(trades, _strategyConfig.MaxPositionSize, trades.First().Price * _strategyConfig.MaxPositionSize);

                _robotView.BindTradeStatistics(dataAnalizer.GetStatistics(), _strategyConfig.MaxPositionSize);
            }

        }

        private void BindConnectionState()
        {
            var lastServerTime = _marketDataProvider.GetServerTime();

            var isConnected = Math.Abs((lastServerTime - DateTime.Now).TotalSeconds) < RobotSettings.LostConnectionTimeOutInSeconds;

            _robotView.BindConnectionState(lastServerTime, isConnected);
        }
    }
}
