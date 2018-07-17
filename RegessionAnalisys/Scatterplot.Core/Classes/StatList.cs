using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class StatList: List<double>
    {
        private double? _median = null;
        private double? _medianSigma = null;
        
        public double Median
        {
            get
            {
                if (this.Any() == false)
                {
                    return 0;
                }

                if (_median.HasValue == false)
                {
                    var middle = this.Count() / 2;
                    _median = this.OrderBy(x => x).ToArray()[middle];
                }

                return _median.Value;
            }
        }

        public double MedianSigma
        {
            get
            {
                if (this.Any() == false)
                {
                    return 0;
                }

                if (_medianSigma.HasValue == false)
                {
                    var deviations = new StatList();

                    ForEach(x => deviations.Add(Math.Abs(x - Median)));

                    _medianSigma = deviations.Median;
                }

                return _medianSigma.Value;
            }
        }
    }
}