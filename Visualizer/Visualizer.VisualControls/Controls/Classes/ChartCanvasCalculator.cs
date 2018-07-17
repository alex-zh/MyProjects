using System;

namespace Visualizer.VisualControls.Controls.Classes
{
    public class ChartCanvasCalculator
    {
        private readonly double _minX;
        private readonly double _minY;
        private readonly double _maxX;
        private readonly double _maxY;
        private readonly double _canvasHeight;
        private readonly double _canvasWidth;
        private const double TopOffset = 3;
        private const double BottomOffset = 30;
        private double _widthOffset = 60;

        public ChartCanvasCalculator(ChartCanvasCalculatorSettings settings) : 
            this(settings.CanvasHeight, settings.CanvasWidth, settings.MinX, settings.MinY, settings.MaxX, settings.MaxY)
        {
            _widthOffset = settings.WidthOffset;
            _canvasWidth = settings.CanvasWidth - 2 * _widthOffset;
        }

        public ChartCanvasCalculator(double canvasHeight, double canvasWidth, double minX, double minY, double maxX, double maxY)
        {
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;
            _canvasHeight = canvasHeight - TopOffset - BottomOffset;
            _canvasWidth = canvasWidth - 2 * _widthOffset;
        }

        public double CanvasHeight
        {
            get { return _canvasHeight; }
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

            return _widthOffset + (value - _minX) * _canvasWidth / (_maxX - _minX);
        }
    }
}