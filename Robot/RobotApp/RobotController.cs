using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Classes;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Classes.StrategyTester;
using Common.Robots.Common;
using Common.Robots.TrendFollower;
using Robot.Core;
using Robot.Core.Classes;
using Robot.Core.Keepers;
using Robot.Core.MarketDataProviders;
using Robot.Core.Providers;
using Robot.DataLayer.Repositories;
using Robot.Strategies.Agents;
using Robot.TradeApplication.Controls;
using Robot.TradeApplication.Keepers;

namespace Robot.TradeApplication
{
    class RobotController
    {
        private readonly AutoResetEvent _robotViewUpdaterResetEvent = new AutoResetEvent(false);

        private readonly RobotViewControl _robotView;
        private ILogger _logger = LoggerKeeper.Get();
        private readonly IRobotExecutor _robotExecutor = new RobotExecutor(LoggerKeeper.Get());
        private readonly RobotStateRepository _robotStateRepository = new RobotStateRepository();
        private readonly RobotRepository _robotRepository = new RobotRepository();
        private readonly RobotTradeRepository _tradesRepository = new RobotTradeRepository();

        private IMarketDataProvider _marketDataProvider;
        private RobotNames _robotName;
        private Securities _security;
        private RobotState _robotState;
        private RobotInfo _robotInfo;
        private IExecutableRobot _robot;

        public RobotController(RobotViewControl robotView)
        {
            _robotView = robotView;
         }

        public bool IsExecutionStarted => _robotExecutor.IsExecutionStarted;

        public void Init(RobotNames robotName, Securities security)
        {
            _robotName = robotName;
            _security = security;

            _robotInfo = _robotRepository.GetByName(_robotName);
            _robotState = _robotStateRepository.GetById(_robotInfo.RobotId);            

            _robotView.RobotMode = _robotInfo.Mode;
            _robotView.BindRobotState(_robotState);

            if (_robotInfo.Status == RobotOperationalStatuses.Started)
            {
                _robotView.ShowRobotIsStartedWarning();
            }

            _marketDataProvider = MarketDataProviderKeeper.Get(robotName, MarketDataProviderTypes.Operative);

            Task.Factory.StartNew(UpdateRobotView, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent).LogExceptions(_logger);

        }

        public void Start()
        {
            var robotMode = _robotView.RobotMode;

            _logger = LoggerKeeper.GetFor(_robotName);

            _robot = RobotFactory.CreateTrendFollower(robotMode, _robotName, _security, _robotState, _logger);

            Task.Factory.StartNew(() => _robotExecutor.Start(_robot),
                                  TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent)
                        .LogExceptions(_logger);


            _robot.StateManager.StateChanged += StateManagerOnStateChanged;
            _robot.StateManager.OrdersCompleted += StateManagerOnOrdersCompleted;

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
            _robotStateRepository.Update(_robotInfo.RobotId, _robotState);

            _robotView.BindRobotState(_robotState);
        }

        private void StateManagerOnOrdersCompleted(object sender, OrdersCompletedEventArgs eventArgs)
        {
            Task.Factory.StartNew(() => new CompletedTradesUpdator(_robotName,
                                                                        _logger,
                                                                         () => _marketDataProvider.GetTrades()).Update(eventArgs.Orders),
                                       TaskCreationOptions.AttachedToParent).LogExceptions(_logger);
        }

        
        private void UpdateRobotView()
        {
            try
            {
                while (true)
                {
                    _robotViewUpdaterResetEvent.WaitOne(2000);

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
            var isOffsetExceeded = (DateTime.Now - lastIterationTime).TotalSeconds > 1.5*RobotSettings.IterationOffset;

            _robotView.BindLastIterationTime(lastIterationTime, isOffsetExceeded);            
        }

        private void UpdateActiveOrders()
        {
            var activeOrders = _marketDataProvider.GetLimitedOrders().Where(x => x.Status == OrderStatuses.Active).ToList();
            _robotView.BindActiveOrders(activeOrders);
        }

        private void UpdateCompletedTrades()
        {
            var trades = _tradesRepository.GetAllFor(_robotName, DateTime.Now).ToList();
            _robotView.BindTrades(trades);

            if (trades.Any())
            {
                var strategyConfig = StrategyOptionsFactory.GetForRobot(_robotName);
                var dataAnalizer = new TradeDataAnalyzer(trades, strategyConfig.MaxPositionSize, trades.First().Price * strategyConfig.MaxPositionSize);

                _robotView.BindTradeStatistics(dataAnalizer.GetStatistics(), strategyConfig.MaxPositionSize);
            }

        }

        private void BindConnectionState()
        {
            var lastCandle = _marketDataProvider.GetLastCandle(_security);

            if (lastCandle != null)
            {
                var isConnected = Math.Abs((lastCandle.Date - DateTime.Now).TotalSeconds) < 60 + RobotSettings.QuikDdeCandlesUpdateOffset;
                _robotView.BindConnectionState(lastCandle.Date, isConnected);
            }
            else
            {
                _robotView.BindConnectionState(DateTime.MinValue, false);
            }
        }
    }
}
