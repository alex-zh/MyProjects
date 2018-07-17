using Common;
using Common.Classes;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Robot.Core;
using Robot.Core.Keepers;
using Robot.Strategies;
using Robot.Strategies.Agents;

namespace Robot.TradeApplication
{
    public class RobotFactory
    {
        public static TrendFollowerRobot CreateTrendFollower(RobotModes mode,
                                                            RobotNames robotName,
                                                            Securities security,
                                                            RobotState robotState,
                                                            RobotStateSynchronizer stateSynchronizer,
                                                            ILogger logger)
        {
            return mode == RobotModes.Operative ?
                           CreateTrendFollower(robotName, security, robotState, logger) :
                           CreateSibrRobotForLogging(robotName, security, robotState, stateSynchronizer, logger);
        }

        private static TrendFollowerRobot CreateTrendFollower(RobotNames robotName,
                                                   Securities security,
                                                   RobotState robotState,
                                                   ILogger logger)
        {

            var strategyOptions = StrategyConfigFactory.GetForRobot(robotName);
            var agent = new TrendFollowerStrategyAgent(
                strategyOptions,
                new LongTermParametersProvider(strategyOptions, logger),
                logger);

            var robot = new TrendFollowerRobot(robotName,
                                    security,
                                    robotState,
                                    strategyOptions,
                                    agent,
                                    new QuikTradeExecutor(logger),
                                    QuikDataProviderKeeper.GetOperationalOne(),
                                    new RobotOperationTimeScheduler(),
                                    logger);
            return robot;
        }

        private static TrendFollowerRobot CreateSibrRobotForLogging(RobotNames robotName,
                                                         Securities security,
                                                         RobotState robotState,
                                                         ILogger logger)
        {
            var loggingDataProvider = QuikDataProviderKeeper.GetLoggingOne(robotState.Orders);

            var robot = new TrendFollowerRobot(robotName,
                                    security,
                                    robotState,
                                    new TrendFollowerStrategyAgent(new StrategyOptions
                                    {
                                        FmaPeriod = executionOptions.FmaPeriod,
                                        SmaPeriod = executionOptions.SmaPeriod,
                                        TakeProfitFactor = executionOptions.TakeProfitFactor
                                    },
                                    new LongTermParametersProvider(executionOptions.FmaPeriod, executionOptions.SmaPeriod, logger),
                                    logger),
                                    new QuikTradeExecutorForLogging(loggingDataProvider, logger),
                                    loggingDataProvider,
                                    new RobotOperationTimeScheduler(),
                                    stateSynchronizer,
                                    logger,
                                    executionOptions);
            return robot;
        }
    }
}