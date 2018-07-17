using System;

namespace Common
{
    public class DatePrice : IDatePrice
    {
        public int Index { get; set; }
        public DateTime Date{ get; set; }
        public double Value { get; set; }
    }
}