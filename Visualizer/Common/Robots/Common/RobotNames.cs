using Common.Classes;

namespace Common.Robots.Common
{
    public enum RobotTypes
    {
        TrendFollower,
        NetThrower
    }

    public class RobotIdentifier
    {
        public static string Get(RobotTypes robotType, TimeFrames timeFrame, string specifier)
        {
            return $"{robotType}_{timeFrame}_{specifier}";
        }

        public static string NetThrower_1min_075 => "NetThrower_1min_075";
    }

    public enum RobotNames
    {
        TrendFollowerXbtUsd = 0,
        TrendFollowerSi = 1,
        NetThrower = 2,
    }
}