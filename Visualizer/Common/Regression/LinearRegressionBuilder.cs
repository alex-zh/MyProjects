using System;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.InteropServices;
using Accord.Math;

namespace Common.Regression
{
    public class LinearRegressionBuilder
    {
        private readonly double[] _x;
        private readonly double[] _y;
        private double[,] _derivativeMatrix;
        private double[,] _lambdaMatrix;
        private double[] _bettaMatrix;

        public LinearRegressionBuilder(double[] x, double[] y)
        {
            if (x.Length != y.Length)
            {
                throw new ArgumentException("X and Y have different lengths");
            }

            if (x.Length == 0 || y.Length ==0)
            {
                throw new ArgumentException("X and Y should have at least 1 value");
            }

            _x = x;
            _y = y;
        }

        public LinearRegression Build()
        {
            _derivativeMatrix = BuildDerivativeMatrix();
            _lambdaMatrix = BuildLambdaMatrix();
            _bettaMatrix = BuildBettaMatrix();

            var result = _lambdaMatrix.Solve(_bettaMatrix, true);

            var regression = new LinearRegression { a0 = result[0], a1 = result[1] };

            var sum = _x.Select((x, i) => Math.Pow(regression.a0 + regression.a1 * x - _y[i], 2)).Sum();

            var mean = _y.Average();
            var variance = _y.Select(y => Math.Pow(y - mean, 2)).Sum();

            regression.StandardEstimateError = Math.Pow(sum / (_x.Length - 2), 0.5);

            regression.RSquared = 1 - (sum / variance);

            regression.RegressionCoefficient = CalculateCoefficient();

            return regression;
        }

        private double CalculateCoefficient()
        {
            var xMean = _x.Average();
            var yMean = _y.Average();

            var covariance = _x.Select((x, i) => (x - xMean)*(_y[i] - yMean)).Sum();
            var varianceX = Math.Sqrt(_x.Select((x, i) => Math.Pow((x - xMean), 2)).Sum());
            var varianceY = Math.Sqrt(_y.Select((y, i) => Math.Pow((y - yMean), 2)).Sum());

            return covariance / (varianceX * varianceY);
        }

        private double[,] BuildLambdaMatrix()
        {
            return _derivativeMatrix.TransposeAndMultiply(_derivativeMatrix);
        }

        private double[] BuildBettaMatrix()
        {
            return _derivativeMatrix.TransposeAndMultiply(_y);
        }

        private double[,] BuildDerivativeMatrix()
        {
            //2 for linear regression

            var result = new double[_x.Length, 2];

            for (int i = 0; i < _x.Length; i++)
            {
                result[i, 0] = 1;
                result[i, 1] = _x[i];
            }

            return result;
        }

    }
}