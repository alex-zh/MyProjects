using System;

namespace RegessionAnalisys.DataPreparers
{
    public class BrentUsdDataPercent
    {
        public double BrentPercent { get; set; }
        public double UsdPercent { get; set; }
        public int Category { get; set; }
        public DateTime Date { get; set; }
        public bool IsGrouped { get; set; }
    }
}