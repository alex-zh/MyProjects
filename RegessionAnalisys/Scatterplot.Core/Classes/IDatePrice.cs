using System;

namespace Common
{
    public interface IDatePrice
    {
        DateTime Date { get; set; }
        double Price { get; set; }
    }
}