using System;

namespace Common.Classes
{
    public class IntersectionSegment
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Movement { get; set; }

        public int TimeSpan
        {
            get { return (int)(EndDate - StartDate).TotalMinutes; }
        }

        public Direction Direction
        {
            get { return Movement > 0 ? Direction.Buy : Direction.Sell; }
        }
    }
}