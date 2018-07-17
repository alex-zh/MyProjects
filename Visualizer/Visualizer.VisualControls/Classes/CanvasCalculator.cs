using System;

namespace Visualizer.VisualControls.Classes
{
    public class CanvasCalculator
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

        public CanvasCalculator(double canvasHeight, double canvasWidth, 
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
            return 2 * CanvasHorizontalOffset + _barOffset + binsCount * (_barWidth + _barOffset);
        }
        
        public double GetY(double value)
        {
            if (Math.Abs(_minY - _maxY) < 0.000000001) return 0;

            return CanvasVerticalOffset + _canvasHeight - (value - _minY) * _canvasHeight / (_maxY - _minY);
        }        

        public double GetX(double value)
        {
            if (Math.Abs(_minX - _maxX) < 0.000000001) return 0;

            return CanvasHorizontalOffset + (value - _minX) * _canvasWidth / (_maxX - _minX);
        }
    }
}