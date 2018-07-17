using Common.Robots.Common;

namespace Common.Classes.Logger
{
    public static class EnumExtentions
    {
        public static string GetFirstLetter(this LogMessageTypes messageType)
        {
            return messageType.ToString().Substring(0, 1).ToUpper();
        }

        public static Direction GetOpposite(this Direction direction)
        {
            return direction == Direction.Buy ? Direction.Sell : Direction.Buy;
        }

        public static int ToInt(this Direction direction)
        {
            return direction == Direction.Buy ? 1: -1;
        }

        public static int ToInt(this OrderSides side)
        {
            return side == OrderSides.Buy ? 1 : -1;
        }

    }
}