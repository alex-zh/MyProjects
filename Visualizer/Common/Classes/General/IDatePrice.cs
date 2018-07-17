using System;

namespace Common
{
    public interface IDatePrice
    {
        DateTime Date { get; set; }
        double Value { get; set; }
    }
}