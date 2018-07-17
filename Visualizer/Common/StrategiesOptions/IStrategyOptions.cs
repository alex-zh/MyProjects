namespace Common.StrategiesOptions
{
    public interface IStrategyOptions
    {
        int NumberOfCandles { get; }
        int MaxPositionSize { get; }
        int MinPositionSize { get; }
    }
}