namespace Common
{
    public class DistributionBin
    {
        public int Count{ get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }

        public override string ToString()
        {
            return MinValue + " ~ " + MaxValue + "; " + Count;
        }
    }
}