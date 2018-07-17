namespace Visualizer.VisualControls.Classes
{
    public interface IChartCalculator
    {
        double GetCoordinateByValue(double value);
        double GetValueByCoordinate(double coordinate);
    }
}