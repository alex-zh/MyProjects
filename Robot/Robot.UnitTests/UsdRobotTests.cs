using System;
using System.Collections.Generic;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using Common.StrategiesOptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Core;
using Robot.UnitTests.Stubs;
using Robot.UnitTests.UsdStrategyAgetStubs;

namespace Robot.UnitTests
{
    [TestClass]
    public class UsdRobotTests
    {
        private IMarketOperationExecutor _marketOperationExecutor;
        private readonly MarketDataProviderStub _marketDataProviderStub = new MarketDataProviderStub() ;
        private StrategyOptions _strategyOptions = new StrategyOptions() {
            PositionSize = 2,
            MinPositionSize = -2,
            MaxPositionSize = 2,
            HalfPositionSize = 1,
            UseTakeProfit = true, 
            UseStopLoss = true
        };

        private readonly ILogger _logger = new LoggerStub();

        [TestInitialize]
        public void TestInit()
        {            
            _marketOperationExecutor = new MarketOperationExecutorStub(_marketDataProviderStub);  
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            _robot = null;
            _agent = null;
        }

        private void Init(string xmlStrategyScript)
        {
            _agent = new StrategyAgentStub(xmlStrategyScript);

            _robot = new TrendFollowerRobot(RobotNames.TrendFollowerSi,
                                  Securities.Si,
                                  new RobotStateOld(0, new Dictionary<RobotOrderTypes, LimitOrderOld>()),
                                  _strategyOptions,
                                  _agent,
                                  _marketDataProviderStub,
                                  _marketOperationExecutor,                                  
                                  new LoggerStub() 
                                  );            
        }

        private void Iterate(bool moveAllToCompleted = false)
        {
            _robot.Iterate();
            _agent.Iteration++;

            if (moveAllToCompleted)
            {
                _marketDataProviderStub.MoveAllTo(OrderStatuses.Completed);                
            }

            _marketDataProviderStub.TriggerChangedEvent();            
        }

        [TestMethod]
        public void OpenClose_Success()
        {
            Init(UsdRobotTestStrategies.OpenClose);

            var robotState = _robot.StateManager.State;

            Assert.AreEqual(robotState.Size, 0);

            Iterate(true);
            
            Assert.AreEqual(robotState.Size, Math.Abs(robotState.Size)>0);
            Assert.AreEqual(_strategyOptions.PositionSize, robotState.Size);

            Iterate(true); // setup close position order

            Assert.AreEqual(robotState.Size, 0);
            Assert.AreEqual(0, robotState.Size);
        }

        [TestMethod]
        public void OpenIncorrectClose_Success()
        {
            Init(UsdRobotTestStrategies.OpenIncorrectCloseDirection);

            var robotState = _robot.StateManager.State;
            
            Assert.AreEqual(robotState.Size, 0);

            Iterate(true);
            
            Assert.AreNotEqual(robotState.Size, 0);
            Assert.AreEqual(_strategyOptions.PositionSize, robotState.Size);

            Iterate(true);
            /* setup close position order, but position is up and close direction is down 
               so position should remain open */

            Assert.AreNotEqual(robotState.Size, 0);
            Assert.AreEqual(_strategyOptions.PositionSize, robotState.Size);
        }

        [TestMethod]
        public void SeveralOpportunitiesToOpenUpPosition_PositionOpenedOnlyOnce_Success()
        {
            Init(UsdRobotTestStrategies.SeveralOpportunitiesToOpenPositionUpStrategy);

            var robotState = _robot.StateManager.State;

            Assert.AreEqual(robotState.Size, 0);

            Iterate(true);

            Assert.AreNotEqual(0, robotState.Size);
            Assert.AreEqual(_strategyOptions.PositionSize, robotState.Size);

            Iterate(true);

            Assert.AreNotEqual(0, robotState.Size);
            Assert.AreEqual(_strategyOptions.PositionSize, robotState.Size);            
        }


        [TestMethod]
        public void SeveralOpportunitiesToOpenDownPosition_PositionOpenedOnlyOnce_Success()
        {
            Init(UsdRobotTestStrategies.SeveralOpportunitiesToOpenPositionDownStrategy);

            var robotState = _robot.StateManager.State;

            Assert.AreEqual(robotState.Size, 0);

            Iterate(true);

            Assert.AreNotEqual(0, robotState.Size);
            Assert.AreEqual(_strategyOptions.PositionSize * (-1), robotState.Size);

            Iterate(true);

            Assert.AreNotEqual(0, robotState.Size);
            Assert.AreEqual(_strategyOptions.PositionSize * (-1), robotState.Size);
        }

        [TestMethod]
        public void OpenUpThenTakeProfitThenAddToPosition_Success()
        {
            Init(UsdRobotTestStrategies.OpenUpThenTakeProfitThenAddToPosition);

            var robotState = _robot.StateManager.State;

            Assert.AreEqual(robotState.Size, 0);

            Iterate(true);

            Assert.AreEqual(_strategyOptions.PositionSize, robotState.Size);

            Iterate(true); // should register TAKE PROFIT order

            Assert.AreNotEqual(0, robotState.Size);           
            Assert.AreEqual(_strategyOptions.HalfPositionSize, robotState.Size);

            Iterate(true); // should register ADDING TO POSITION order

            Assert.AreNotEqual(0, robotState.Size);
            Assert.AreEqual(_strategyOptions.PositionSize, robotState.Size);

            Iterate(true); // should register SELL position order            

            Assert.AreEqual(robotState.Size, 0);
            Assert.AreEqual(0, robotState.Size);                        
        }

        [TestMethod]
        public void OpenDownThenTakeProfitThenAddToPosition_Success()
        {            
            Init(UsdRobotTestStrategies.OpenDownThenTakeProfitThenAddToPosition);

            var robotState = _robot.StateManager.State;

            Assert.AreEqual(robotState.Size, 0);

            Iterate(true);

            Assert.AreEqual(_strategyOptions.PositionSize * -1, robotState.Size);

            Iterate(true); // should register TAKE PROFIT order

            Assert.AreNotEqual(0, robotState.Size);
            Assert.AreEqual(_strategyOptions.HalfPositionSize*-1, robotState.Size);

            Iterate(true); // should register ADDING TO POSITION order

            Assert.AreNotEqual(0, robotState.Size);
            Assert.AreEqual(_strategyOptions.PositionSize*-1, robotState.Size);

            Iterate(true); // should register close position order            

            Assert.AreEqual(robotState.Size, 0);
            Assert.AreEqual(0, robotState.Size);
        }

        [TestMethod]
        public void OpenThenTakeProfitThenTakeProfitWasWithdrawnThenNewTakeProfit()
        {
            Init(UsdRobotTestStrategies.OpenThenTakeProfitThenTakeProfitWasWithdrawnThenNewTakeProfit);

            var robotState = _robot.StateManager.State;

            Assert.AreEqual(robotState.Size, 0);

            Iterate(true);

            Assert.AreEqual(_strategyOptions.PositionSize, robotState.Size);

            _marketDataProviderStub.ClearAll();

            Iterate(false); // should register TAKE PROFIT order

            _marketDataProviderStub.MoveToStatus(0, OrderStatuses.Withdrawn);
            _marketDataProviderStub.TriggerChangedEvent();            

            Assert.AreNotEqual(0, robotState.Size);
            Assert.AreEqual(_strategyOptions.PositionSize, robotState.Size);

            Iterate(true); // should register TAKE PROFIT order

            Assert.AreNotEqual(0, robotState.Size);
            Assert.AreEqual(_strategyOptions.HalfPositionSize, robotState.Size);
        }

        [TestMethod]
        public void OpenDown_OrderNotExecuted_TakeProfit_TakeProfitExecuted()
        {
            Init(UsdRobotTestStrategies.OpenDownOrderNotExecutedTakeProfitNotExecuted);

            var robotState = _robot.StateManager.State;

            Assert.AreEqual(robotState.Size, 0);

            Iterate(true); // open position down

            Assert.AreEqual(_strategyOptions.PositionSize * -1, robotState.Size);

            Iterate(); // register position up

            Assert.AreEqual(_strategyOptions.PositionSize * -1, robotState.Size); // order is not executed yet

            Iterate(); // register TP order, open order should be withdrawn because it hasn't been repeated 

            Assert.AreEqual(_strategyOptions.PositionSize * -1, robotState.Size); // nothing changed

            Iterate(true); // one TP order should be executed

            Assert.AreEqual(_strategyOptions.HalfPositionSize * -1, robotState.Size); // orders are not executed yet                       
        }

        [TestMethod]
        public void Open_SetupTakeProfit_Close_TakeProfitShouldBeWithdrawn()
        {
            Init(UsdRobotTestStrategies.OpenSetupTakeProfitCloseTakeProfitShouldBeWithdrawn);

            var robotState = _robot.StateManager.State;

            Assert.AreEqual(robotState.Size, 0);

            Iterate(true); // open position down

            Assert.AreEqual(_strategyOptions.PositionSize * -1, robotState.Size);

            Iterate(); // register up take profit

            Iterate(); // register close order

            _marketDataProviderStub.TriggerChangedEvent(); // synchronize killed (withdrawn) orders

            Assert.AreEqual(1, robotState.GetOrders().Count()); // take profit order should be removed                        

            Assert.AreEqual(_strategyOptions.PositionSize * -1, robotState.Size);

            _marketDataProviderStub.MoveAllTo(OrderStatuses.Completed); 
            _marketDataProviderStub.TriggerChangedEvent(); 
            
            Assert.AreEqual(0, robotState.Size); // order is not executed yet
            Assert.AreEqual(robotState.Size, 0); // order is not executed yet
        }

        [TestMethod]
        public void OpenThatHasNotBeenExecutedShouldBeWithdrawn_Success()
        {
            Init(UsdRobotTestStrategies.OpenThenTakeProfitThenTakeProfitWasWithdrawnThenNewTakeProfit);

            var robotState = _robot.StateManager.State;

            Assert.AreEqual(robotState.Size, 0);

            Iterate();

            Assert.AreEqual(robotState.Size, 0);
            Assert.AreEqual(true, robotState.GetOrders().First().IsActive);
            Assert.AreEqual(RobotOrderTypes.OpenPosition, robotState.GetOrders().First().RobotOrderType);

            Iterate();

            Assert.AreEqual(robotState.Size,0);
            Assert.AreEqual(0, robotState.GetOrders().Count());
        }
    }
}
