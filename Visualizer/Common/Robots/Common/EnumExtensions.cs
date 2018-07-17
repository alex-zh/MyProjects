using System;
using System.Collections.Generic;

namespace Common.Classes.Robot
{
    public class Enum<T> where T : struct, IConvertible
    {
        public static IEnumerable<KeyValuePair<int, string>> ToList()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            foreach (var v in Enum.GetValues(typeof(T)))
            {
                yield return new KeyValuePair<int, string>((int)v, Enum.GetName(typeof(T), v));
            }
        }        
    }


}