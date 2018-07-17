using System;
using System.Collections.Generic;

namespace Core
{
    public interface IVolatilityCalculator
    {
        double Get(int index);
        SortedList<DateTime, double> GetAll();
    }
}