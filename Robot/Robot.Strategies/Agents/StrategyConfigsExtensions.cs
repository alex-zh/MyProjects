using System.Collections.Generic;
using Common.Classes.Robot;

namespace Robot.Strategies.Agents
{
    public enum StrategyConfigs
    {
        NetThrower_63_0015,
        NetThrower_1023_00075,
        NetThrower_1023_001,
    }

    public enum StrategyConfigTypes
    {
        Test,
        Work
    }

    public static class StrategyConfigsExtensions
    {
        public static string ToText(this StrategyConfigs connector)
        {            
            return connector.ToString();
        }

        public static IEnumerable<KeyValuePair<int, string>> ToList()
        {
            return Enum<StrategyConfigs>.ToList();
        }
    }
}