using System;
using Common;
using Common.Classes;
using Common.Classes.Robot;
using Common.Robots.Common;
using Common.StrategiesOptions;
using Robot.Strategies.Agents.Configs;

namespace Robot.Strategies.Agents
{
    public class StrategyOptionsFactory
    {
        public static IStrategyOptions GetForRobot(string robotId)
        {
            if (robotId == RobotIdentifier.NetThrower_1min_075)
            {
                return GetByName(StrategyConfigs.NetThrower_1023_00075, StrategyConfigTypes.Work);
            }

            throw new NotSupportedException("Strategy options are not supported for {robotId}");
        }

        public static IStrategyOptions GetForRobot(RobotNames name)
        {
            if (name == RobotNames.NetThrower)
            {
                return GetByName(StrategyConfigs.NetThrower_63_0015, StrategyConfigTypes.Test);
            }

            throw new NotSupportedException($"No strategy options are specified for robot {name}");

        }

        public static IStrategyOptions GetByName(StrategyConfigs name, StrategyConfigTypes configType)
        {
            if (configType == StrategyConfigTypes.Test)
            {
                switch (name)
                {
                    case StrategyConfigs.NetThrower_63_0015:
                        return StrategyConfigsKeeper.NetThrower_63_0015;
                    case StrategyConfigs.NetThrower_1023_00075:
                        return StrategyConfigsKeeper.NetThrower_1023_00075;
                    case StrategyConfigs.NetThrower_1023_001:
                        return StrategyConfigsKeeper.NetThrower_1023_001;

                }
            }
            else
            {
                switch (name)
                {
                    case StrategyConfigs.NetThrower_63_0015:
                        return StrategyConfigsKeeper.NetThrower_63_0015;
                    case StrategyConfigs.NetThrower_1023_00075:
                        return StrategyConfigsKeeper.NetThrower_1023_00075;
                    case StrategyConfigs.NetThrower_1023_001:
                        return StrategyConfigsKeeper.NetThrower_1023_001;
                }
            }

            throw new NotSupportedException(name.ToString());
        }

        
    }
}
