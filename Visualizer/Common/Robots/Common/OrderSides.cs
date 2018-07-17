namespace Common.Robots.Common
{
    public enum OrderSides
    {
        Buy = 0,
        Sell = 1,
    }

    public enum ExecutionTypes
    {
        New,
        Trade,
        Filled,
        Canceled,
        Rejected
    }
}