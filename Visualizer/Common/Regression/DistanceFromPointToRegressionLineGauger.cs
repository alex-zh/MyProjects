using System;

namespace Common.Regression
{
    public class DistanceFromPointToRegressionLineGauger
    {
        private readonly LinearRegression _regression;

        public DistanceFromPointToRegressionLineGauger(LinearRegression regression)
        {
            _regression = regression;
        }

        public double GetDistance(double x, double y)
        {
            var a1 = _regression.a1;
            var regressionY = _regression.a0 + a1 * x;
            var difference = y - regressionY;
            
            var cosAlpha = 1 / Math.Sqrt(1 + a1 * a1);

            return difference * cosAlpha;
        }
    }
}