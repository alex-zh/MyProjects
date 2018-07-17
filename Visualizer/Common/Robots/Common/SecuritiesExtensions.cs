using System;
using System.Collections.Generic;
using Common.Robots.Common;

namespace Common.Classes.Robot
{
    public static class SecuritiesExtensions
    {
        public static  string ToText(this Securities security)
        {
            switch (security)
            {
                case Securities.Si:
                    return "Si";
                case Securities.Ri:
                    return "Ri";
                case Securities.Br:
                    return "Brent";                
            }
            return security.ToString();
        }

        public static IEnumerable<KeyValuePair<int, string>> ToList()
        {
            foreach(var v in Enum.GetValues(typeof (Securities)))
            {
                yield return new KeyValuePair<int, string>((int)v, Enum.GetName(typeof(Securities), v));
            }          
        }        
    }
}