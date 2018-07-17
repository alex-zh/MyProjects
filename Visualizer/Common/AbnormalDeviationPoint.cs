using System;

namespace Common
{
    public class AbnormalDeviationPoint
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public double CandlePrice { get; set; }
        public double MaPrice { get; set; }
        public double Deviation { get; set; }
        public double Weight { get; set; }
        public double DeviationPercent { get; set; }

        public override string ToString()
        {
            return String.Format("Weight: " + Weight + "   Deviation: " + Deviation.ToString("#.000"));
        }
    }
}