using System;

namespace Visualizer.VisualControls.Controls
{
    public class BivariateChartCanvasCalculator
    {
        private readonly double _minX;
        private readonly double _minY;
        private readonly double _maxX;
        private readonly double _maxY;
        private readonly double _canvasHeight;
        private readonly double _canvasWidth;
        private const double TopOffset = 3;
        private const double BottomOffset = 30;
        private const double WidthOffset = 60;

        public BivariateChartCanvasCalculator(double canvasHeight, double canvasWidth, double minX, double minY, double maxX, double maxY)
        {
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;
            _canvasHeight = canvasHeight - TopOffset - BottomOffset;
            _canvasWidth = canvasWidth - 2 * WidthOffset;
        }

        public double CanvasWidth
        {
            get { return _canvasWidth; }
        }

        public double GetY(double value)
        {
            if (Math.Abs(_minY - _maxY) < 0.000000001) return 0;

            return TopOffset + _canvasHeight - (value - _minY) * _canvasHeight / (_maxY - _minY);
        }

        public double GetX(double value)
        {
            if (Math.Abs(_minX - _maxX) < 0.000000001) return 0;

            return WidthOffset + (value - _minX) * _canvasWidth / (_maxX - _minX);
        }
    }
}