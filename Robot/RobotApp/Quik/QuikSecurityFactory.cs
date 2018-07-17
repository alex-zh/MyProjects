using System;
using Common.Classes.Robot;
using Robot.Core.Classes;
using Robot.Trans2Quik.Entities.Transaction.Order;

namespace Robot.Core
{
    public class QuikSecurityFactory
    {
        public static Security Get(Securities security)
        {
            switch (security)
            {
                case Securities.Si:
                    return new Security(RobotSettings.FuturesClassCode, RobotSettings.SecurityCodeSi);
                case Securities.Ri:
                    return new Security(RobotSettings.FuturesClassCode, RobotSettings.SecurityCodeRi);
                default:
                    throw new ArgumentOutOfRangeException(security.ToString());
            }
        }
    }
}