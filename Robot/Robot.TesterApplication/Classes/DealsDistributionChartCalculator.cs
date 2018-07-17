using System;

namespace Robot.TesterApplication.Classes
{
    public class DealsDistributionChartCalculator
    {
        private readonly double _canvasWidth;
        private readonly int _barWidth;
        private readonly int _barOffset;
        private readonly double _minY;
        private readonly double _maxY;
        private double _canvasHeight;
        private double _canvasHeightHalf;
        private int CanvasVerticalOffset = 40;
        private int CanvasHorizontalOffset = 40;
        private double _minX;
        private double _maxX;

        public DealsDistributionChartCalculator(double canvasHeight, double canvasWidth, 
                                           int barWidth, int barOffset, 
                                           double minX, double maxX, double minY, double maxY)
        {            
            _barWidth = barWidth;
            _barOffset = barOffset;
         
            _minY = minY;
            _maxY = maxY;

            _minX = minX;
            _maxX = maxX;

            _canvasWidth = canvasWidth - 2 * CanvasHorizontalOffset;
            _canvasHeight = canvasHeight - 2 * CanvasVerticalOffset;
            _canvasHeightHalf = canvasHeight / 2 - CanvasVerticalOffset;
        }

        public int GetCanvasWidth(int binsCount)
        {
            return _barOffset + binsCount * (_barWidth + _barOffset) + 2 * CanvasHorizontalOffset;
        }
        
        public double GetX(int index)
        {
            return CanvasHorizontalOffset + _barWidth + index * (_barWidth + _barOffset);
        }

        public double GetY(double value)
        {
            var maxDeviation = Math.Max(Math.Abs(_maxY), Math.Abs(_minY));

            return _canvasHeightHalf - value * _canvasHeightHalf / maxDeviation;
        }
    }
}