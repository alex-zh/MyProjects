using System;
using Common.Classes;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using Common.Robots.TrendFollower;
using Common.StrategiesOptions;
using Robot.Core;
using Robot.Core.Keepers;
using Robot.Core.OperationExecutors;
using Robot.Core.Providers.Quik;
using Robot.Strategies;
using Robot.Strategies.Agents;

namespace Robot.TradeApplication
{
    
    public class RobotFactory
    {
        public static IExecutableRobot CreateTrendFollower(RobotModes mode,
                                                            RobotNames robotName,
                                                            Securities security,
                                                            RobotState robotState,
                                                            ILogger logger)
        {                        
            switch (mode)
            {
                case RobotModes.Logging:
                    return CreateLoggingTrendFollower(robotName, security, robotState, logger);
                case RobotModes.Operative:
                    return CreateOperativeTrendFollower(robotName, security, robotState, logger);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

       private static IStrategyAgent GetStrategyAgent(RobotNames robotName, StrategyOptions strategyOptions, LongRangeDataProvider longRangeDataProvider, ILogger logger)
        {
            return robotName == RobotNames.TrendFollowerXbtUsd
                ? (IStrategyAgent) new CryptoTrendFollowerStrategyAgent(strategyOptions, longRangeDataProvider, logger)
                : new TrendFollowerStrategyAgent(strategyOptions, longRangeDataProvider, logger);
        }

        private static TrendFollowerRobot CreateOperativeTrendFollower(RobotNames robotName, Securities security, RobotState robotState, ILogger logger)
        {
            var strategyOptions = (StrategyOptions)StrategyOptionsFactory.GetForRobot(robotName);
            var longRangeDataProvider = new LongRangeDataProvider(strategyOptions, logger);
            var marketDataProvider = MarketDataProviderKeeper.Get(robotName, MarketDataProviderTypes.Operative);
            var marketOperationExecutor = new MarketOperationExecutor(logger);

            var strategyAgent = GetStrategyAgent(robotName, strategyOptions, longRangeDataProvider, logger);

            var robot = new TrendFollowerRobot(robotName, security, robotState, strategyOptions, strategyAgent, marketDataProvider, marketOperationExecutor, logger);
            return robot;
        }

        private static TrendFollowerRobot CreateLoggingTrendFollower(RobotNames robotName, Securities security, RobotState robotState, ILogger logger)
        {
            var strategyOptions = (StrategyOptions)StrategyOptionsFactory.GetForRobot(robotName);
            var longRangeDataProvider = new LongRangeDataProvider(strategyOptions, logger);
            var marketDataProvider = (QuikLoggingMarketDataProvider)MarketDataProviderKeeper.Get(robotName, MarketDataProviderTypes.Logging, robotState.Orders);
            var marketOperationExecutor = new MarketOperationLoggingExecutor(marketDataProvider, logger);

            var strategyAgent = GetStrategyAgent(robotName, strategyOptions, longRangeDataProvider, logger);

            var robot = new TrendFollowerRobot(robotName, security, robotState, strategyOptions, strategyAgent, marketDataProvider, marketOperationExecutor, logger);
            return robot;
        }

        //private static TrendFollowerRobot CreateTestingTrendFollower(RobotNames robotName, Securities security, RobotState robotState, ILogger logger)
        //{
        //    var strategyOptions =  (StrategyOptions)StrategyOptionsFactory.GetForRobot(robotName);
        //    var longRangeDataProvider = new LongRangeDataProvider(strategyOptions, logger);            
        //    var marketDataProvider = MarketDataProviderKeeper.Get(robotName, MarketDataProviderTypes.Testing);
        //    var marketOperationExecutor = MarketOperationExecutorKeeper.Get(MarketDataProviderTypes.Testing);

        //    var strategyAgent = GetStrategyAgent(robotName, strategyOptions, longRangeDataProvider, logger);

        //    var robot = new TrendFollowerRobot(robotName, security, robotState, strategyOptions, strategyAgent, marketDataProvider, marketOperationExecutor, logger);
        //    return robot;
        //}

        //private static NetThrowerRobot CreateTestingNetThrower(RobotNames robotName, Securities security, NetThrowerRobotState robotState, IMarketDataProvider marketDataProvider, IStrategyOptions strategyOptions, ILogger logger)
        //{
        //    var marketOperationExecutor = MarketOperationExecutorKeeper.Get(MarketDataProviderTypes.Testing);

        //    var robot = new NetThrowerRobot(robotName, security, robotState, (NetThrowerOptions)strategyOptions, marketDataProvider, marketOperationExecutor, logger);
        //    return robot;
        //}
    }

    
}