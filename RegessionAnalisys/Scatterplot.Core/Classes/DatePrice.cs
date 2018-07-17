using System;

namespace Common
{
    public class DatePrice : IDatePrice
    {
        public DateTime Date{ get; set; }
        public double Price { get; set; }
    }
}