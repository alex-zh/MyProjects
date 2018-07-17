using System;

namespace RegessionAnalisys
{
    public class CanvasCalculator
    {
        private readonly double _minX;
        private readonly double _minY;
        private readonly double _maxX;
        private readonly double _maxY;
        private readonly double _canvasHeight;
        private readonly double _canvasWidth;
        private const double Offset = 20;

        public CanvasCalculator(double canvasHeight, double canvasWidth, double minX, double minY, double maxX, double maxY)
        {
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;
            _canvasHeight = canvasHeight - 2 * Offset;
            _canvasWidth = canvasWidth - 2 * Offset;
        }

        public double GetY(double value)
        {
            if (Math.Abs(_minY - _maxY) < 0.000000001) return 0;

            return Offset + _canvasHeight - (value - _minY) * _canvasHeight / (_maxY - _minY);
        }

        public double GetX(double value)
        {
            if (Math.Abs(_minX - _maxX) < 0.000000001) return 0;

            return Offset + (value - _minX) * _canvasWidth / (_maxX - _minX);
        }
    }
}
