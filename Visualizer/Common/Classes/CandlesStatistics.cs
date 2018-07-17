namespace Common.Classes
{
    public class CandlesStatistics
    {
        public int Count { get;  set; }

        public DistributionParameters TailsUp { get;  set; }

        public DistributionParameters TailsDown{ get;  set; }

        public DistributionParameters Body { get;  set; }

        public DistributionParameters Swing { get;  set; }

        public DistributionParameters Volume { get; set; }
    }
}