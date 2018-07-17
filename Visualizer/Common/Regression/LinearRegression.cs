namespace Common.Regression
{
    public class LinearRegression
    {
        public double a0 { get; set; }
        public double a1 { get; set; }

        public double RegressionCoefficient { get; set; }

        /// <summary>
        /// SEE = Standard Error of Estimate
        /// </summary>
        public double StandardEstimateError { get; set; }
        /// <summary>
        /// determination coefficient
        /// See http://habrahabr.ru/post/195146/
        /// </summary>
        public double RSquared { get; set; }
    }
}