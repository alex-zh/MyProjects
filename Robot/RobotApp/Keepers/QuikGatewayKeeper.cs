using Robot.Trans2Quik;

namespace Robot.Core.Keepers
{
    public class QuikGatewayKeeper
    {
        private static Gateway _gateway;

        public static Gateway Get()
        {
            return _gateway ?? (_gateway = new Gateway(RobotSettings.PathToQuik, RobotSettings.Account));
        }
    }
}