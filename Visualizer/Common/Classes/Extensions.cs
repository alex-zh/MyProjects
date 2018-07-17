using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Common.Classes.General;
using Common.Classes.Robot;
using Common.Robots.Common;

namespace Common.Classes
{
    public static class Extensions
    {
        public static string ToUpperFirstLetter(this string text)
        {
            return text.Substring(0, 1).ToUpper() + text.Substring(1);
        }

        public static string ToSplitText(this string camelCaseString)
        {
            return Regex.Replace(camelCaseString, "(?!^)([A-Z])", " $1");            
        }

        public static DateTime ToSecondsPrecision(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);    
        }

        public static DateTime ToMinutesPrecision(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        }       

        public static int IndexOfMax<T, TV>(this List<T> collection, Func<T, TV> valueSelector) where TV : IComparable<TV>
        {
            int maxIndex = -1;
            TV maxValue = default(TV); // Immediately overwritten anyway

            int index = 0;
            foreach (T item in collection)
            {
                var value = valueSelector(item);

                if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }

        public static int IndexOfMin<T, TV>(this List<T> collection, Func<T, TV> valueSelector) where TV : IComparable<TV>
        {
            int minIndex = -1;
            TV minValue = default(TV); // Immediately overwritten anyway

            int index = 0;
            foreach (T item in collection)
            {
                var value = valueSelector(item);

                if (value.CompareTo(minValue) < 0 || minIndex == -1)
                {
                    minIndex = index;
                    minValue = value;
                }
                index++;
            }
            return minIndex;
        }
       
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> collection, int n)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (n < 0)
                throw new ArgumentOutOfRangeException("n", "n must be 0 or greater");

            var temp = new LinkedList<T>();

            foreach (var value in collection)
            {
                temp.AddLast(value);
                if (temp.Count > n)
                    temp.RemoveFirst();
            }

            return temp;
        }

        public static IEnumerable<T> TakeRange<T>(this IEnumerable<T> collection, int startIndex, int endIndex)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (endIndex< startIndex)
                throw new ArgumentNullException("startIndex is bigger than endIndex");

            var temp = new List<T>();

            var i =0 ;

            foreach (var value in collection)
            {
                if (i++ < startIndex) continue;
                
                temp.Add(value);

                if (i > endIndex) break;
            }

            return temp;
        }

        public static RobotTradeOld ToRobotTrade(this LimitOrderOld orderOld)
        {
            return new RobotTradeOld
            {
                DateTime = orderOld.DateTime,                
                Price =  orderOld.Params.Price,
                OrderNumber = orderOld.OrderNumber,
                Security = orderOld.Security.ToString(),
                Size = (int )(orderOld.Params.Side == OrderSides.Buy ? orderOld.Params.Size : orderOld.Params.Size * -1),
            };
        }

        public static bool IsActive(this LimitOrder order)
        {
            if (order.Status == OrderStatuses.Active)
                return true;

            return false;
            ;
        }
    }
}
