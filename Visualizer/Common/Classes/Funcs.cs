using System;

namespace Common.Classes
{
    public static class Funcs
    {
        public static readonly Func<DateTime, DateTime, bool> AreDatesOfOneDay = (firstDate, secondDate) =>
              (firstDate.Day == secondDate.Day) && (firstDate.Month == secondDate.Month);

    }
}