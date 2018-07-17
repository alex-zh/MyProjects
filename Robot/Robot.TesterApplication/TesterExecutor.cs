using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Classes.General;
using Common.Classes.Robot;
using Common.Classes.StrategyTester;
using Common.Robots.Common;
using Common.StrategiesOptions;
using Robot.Core;
using Robot.Core.MarketDataProviders;
using Robot.Core.OperationExecutors;

namespace Robot.TesterApplication
{
    public class TesterExecutor : IRobotExecutor
    {
        private IExecutableRobot _robot;
        private readonly TestMarketDataProvider _marketDataProvider;
        private readonly MarketOperationTestExecutor _operationExecutor;
        private readonly IRobotStateGetter _robotState;
        private readonly IStrategyOptions _strategyOptions;
        private readonly List<Candle> _allCandles;
        private readonly int _startIndex;
        private readonly List<TestTradeItem> _testTradeItems;
        private Candle _notCompletedCandle;

        public TesterExecutor(IRobotStateGetter robotState,
                               IEnumerable<Candle> allCandles,
                               IStrategyOptions strategyOptions,
                               TestMarketDataProvider marketDataProvider,
                               MarketOperationTestExecutor operationExecutor,                                                                   
                               int startIndex)
        {
            _testTradeItems = new List<TestTradeItem>();

            _marketDataProvider = marketDataProvider;
            _operationExecutor = operationExecutor;
            _allCandles = allCandles.ToList();

            _robotState = robotState;
            _strategyOptions = strategyOptions;

            _startIndex = startIndex;

            _marketDataProvider.LimitOrdersChanged += MarketDataProviderLimitedOrdersChanged;
            operationExecutor.OrderKilled += ExecutorOrderKilled;
        }

        void MarketDataProviderLimitedOrdersChanged(object sender, EventArgs e)
        {
            var orders = _marketDataProvider.GetLimitOrders().Where(x => x.Status == OrderStatuses.Completed).ToList();
            
            foreach (var order in orders)
            {
                _testTradeItems.Add(new TestTradeItem
                {
                    DateTime = _notCompletedCandle.Date,
                    Price = order.Price,
                    Size = order.Size * (order.OrderSide == OrderSides.Sell ? -1 : 1)
                });
            }
        }

        public bool IsStarted
        {
            get;
            private set;
        }

        public DateTime GetLastIterationTime()
        {
            return DateTime.Now;
        }

        public void Start(IExecutableRobot robot)
        {
            _robot = robot;

            if (IsStarted) return;

            IsStarted = true;

            _testTradeItems.Clear();

            for (var i = _startIndex; i < _allCandles.Count - 1; i++)
            {
                if (IsStarted == false) break;

                var workingCandles = GetCandlesForAgent(i, _strategyOptions.NumberOfCandles);

                _marketDataProvider.InitCandles("testSymbol", workingCandles);

                /* i indicates just beggining candle, meaning index of last fully completed is i-1 */
                _notCompletedCandle = _allCandles[i];
                _operationExecutor.SetCurrentCandle(_notCompletedCandle);

                for (int j = 0; j <= 0; j++)
                {
                    /* Prevent trades on one candle because 
                       0. хотя на одной и той же свече может быть закрытие и новое открытие,
                       1. нужно исключить возможнось частичного закрытия (по более высокой цене) и нового открытия(по более низкой, которой в реальности уже не будет). 
                       2. закрытие и новое открытие тоже нужно отслеживать что цены корректны
                       3. В реальности ситуация будет немного лучше
                      */
                    _robot.Iterate();

                    _marketDataProvider.SetupOrdersAndMoveThemToCompletedState(_robotState.GetOrders(), _notCompletedCandle, false);
                }
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
            }
        }

        public bool IsExecutionStarted { get; }


        private IEnumerable<Candle> GetCandlesForAgent(int currentCandleIndex, int operationalCandlesNumber)
        {
            var startIndex = Math.Max(0, currentCandleIndex - operationalCandlesNumber);
            var count = currentCandleIndex - startIndex;

            return _allCandles.Skip(startIndex).Take(count);
        }

        public List<TestTradeItem> GetTestData()
        {
            return _testTradeItems;
        }

        void ExecutorOrderKilled(object sender, LimitOrder killedOrder)
        {
            _marketDataProvider.UpdateKilledOrder(_robotState.GetOrders(), killedOrder, true);
        }
    }

}
