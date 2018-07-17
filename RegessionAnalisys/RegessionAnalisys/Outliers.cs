using System.Collections.Generic;

namespace RegessionAnalisys
{
    public class Outliers
    {
        public Outliers(IEnumerable<double> x, IEnumerable<double> y)
        {
            X = new List<double>(x);
            Y = new List<double>(y);
        }

        public List<double> X { get; private set; }
        public List<double> Y { get; private set; }        
    }
}