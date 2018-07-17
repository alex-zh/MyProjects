using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Models.Regression.Linear;

namespace Core.Classes.Regression
{
   public class TestLinearRegression
    {
        public TestLinearRegression()
        {
            double[] inputs =  { 80, 60, 10, 20, 30 };
            double[] outputs = { 20, 40, 30, 50, 60 };

            var regression = new SimpleLinearRegression();
            regression.Regress(inputs, outputs);

            var slope = regression.Slope;


            var mySlope = CalculateSlope(inputs, outputs);

            if (slope == mySlope)
            {
                //You ARE GENIOUS
            }
            //
        }

        private double CalculateSlope(double[] inputs, double[] outputs)
        {
            var xy_sum = 0.0;

            for (int i = 0; i < inputs.Length; i++)
            {
                xy_sum += inputs[i]*outputs[i];
            }

            var x_sum = inputs.Sum();

            var y_sum = inputs.Sum();

            var x2_sum = 0.00;

            for (int i = 0; i < inputs.Length; i++)
            {
                x2_sum += inputs[i]*inputs[i];
            }

            var length = inputs.Length;
            return (xy_sum - x_sum * y_sum/length) / (x2_sum - x_sum * x_sum/length);

        }
    }
}
