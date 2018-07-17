using System;
using System.Globalization;
using System.Threading;
using Visualizer.Core.Classes;

namespace Visualizer.VisualControls.Classes
{
    public static class DateTimeExtensions
    {
        public static DateTime GetGroupingDateTime(this DateTime date, GroupingInterval groupingInterval)
        {
            switch (groupingInterval)
            {
                case GroupingInterval.Raw:
                    return date;

                case GroupingInterval.Day:
                    return new DateTime(date.Year, date.Month, date.Day);

                case GroupingInterval.Week:
                    return date.GetFirstDayOfWeekDate();

                case GroupingInterval.Month:
                    return new DateTime(date.Year, date.Month, 1);

            }
            return date;
        }

        public static DateTime GetFirstDayOfMonth(this DateTime date)
        {            
            return new DateTime(date.Year, date.Month, 1);
        }        

        public static DateTime GetFirstDayOfWeekDate(this DateTime date)
        {
            CultureInfo currentCulture;
            if (Thread.CurrentThread.CurrentCulture.Calendar is GregorianCalendar)
            {
                currentCulture = Thread.CurrentThread.CurrentCulture;
            }
            else
            {
                currentCulture = new CultureInfo("en-US");
            }
            return date.GetFirstDayOfWeekDate(currentCulture);
        }

        public static DateTime GetFirstDayOfWeekDate(this DateTime date, CultureInfo culture)
        {
            int firstDayOfWeek = (int)culture.DateTimeFormat.FirstDayOfWeek;
            int num2 = ((int)date.DayOfWeek) - firstDayOfWeek;
            if (num2 < 0)
            {
                num2 += 7;
            }
            return date.AddDays((double)-num2).Date;
        }

        public static int WeekOfMonth(this DateTime date)
        {
            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            int firstDay = (int)firstDayOfMonth.DayOfWeek;
            if (firstDay == 0)
            {
                firstDay = 7;
            }
            double d = (firstDay + date.Day - 1) / 7.0;
            return (int)Math.Ceiling(d);
        }

    }
}
