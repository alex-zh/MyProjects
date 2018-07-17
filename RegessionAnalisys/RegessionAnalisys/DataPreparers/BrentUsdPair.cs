using System;

namespace RegessionAnalisys.DataPreparers
{
    public class BrentUsdPair
    {
        public double UsdPrice { get; set; }
        public double BrentPrice { get; set; }       
        public int Group { get; set; }
        public DateTime Date { get; set; }
    }
}