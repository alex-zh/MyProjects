using System.Collections.Generic;

namespace Common.Classes.Builders
{
    public class TrendBuilderOptions
    {
        public bool IsForTest { get; set; }
     
        public double LevelUpThreshold { get; set; }
        public double AverageOneDayMove { get; set; }

        public List<double> TestTrendCoefficients { get; set; }
        public List<double> TestLookupWindowCoefficients { get; set; }
    }
}