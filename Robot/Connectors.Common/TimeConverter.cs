using System;

namespace Connector.Common
{
    public class TimeConverter
    {
        public static DateTime FromMilliseconds(double milliseconds)
        {
            var time = TimeSpan.FromMilliseconds(milliseconds);
            return new DateTime(1970, 1, 1) + time;
        }

        public static double FromDate(DateTime dateTime)
        {
            return (long) (dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

    }
}