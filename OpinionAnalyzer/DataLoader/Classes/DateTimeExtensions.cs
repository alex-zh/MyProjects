using System;

namespace OpinionAnalyzer.DataLoader.Classes
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Возращает hex значение для Day-Month-Year
        /// Например 
        /// для 19/09/2014 будет возвращено 13097DE 
        /// </summary>        
        public static string GetHexValue(this DateTime dateTime)
        {         
            return string.Format("{0}{1}{2}", dateTime.Day.ToString("X2"), dateTime.Month.ToString("X2"), dateTime.Year.ToString("X3"));
        }        
    }
}
