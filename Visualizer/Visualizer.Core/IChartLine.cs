using System.Windows.Media;
using Common;

namespace Visualizer.Core
{
    public interface IChartLine
    {     
        Brush Color { get;  }        
        double LineThickness { get; }
    }

    public interface IChartMovingLine: IChartLine
    {
        int Period { get; }
    }

    public interface IChartMovingAverageLine : IChartMovingLine
    {
        DatePrice this[int index] { get; }
    }
}