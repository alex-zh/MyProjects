using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomProcessEmulation
{
    //Y(t) = Y(t-1) + E(t), E(t) ~ iid(mu, sigma^2)
    public class RandomWalkModel
    {
        public List<double> Generate(double startValue, int count)
        {
            var result = new List<double>();

            NormalDistribution nd = new NormalDistribution();
            
            var value = startValue;

            for (int i = 0; i < count; i++)
            {
                value = value + nd.NextGaussian(0, 1);

                result.Add(value);
            }

            return result;            
        }
    }
}
