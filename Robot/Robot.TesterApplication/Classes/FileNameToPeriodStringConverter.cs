using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Robot.TesterApplication.Classes
{
    public class FileNameToPeriodStringConverter
    {
        public string Convert(string fileName)
        {
            string result ="", startDateString = null, endDateString = null;

            if (string.IsNullOrEmpty(fileName) == false)
            {
                var match = Regex.Match(fileName, "(?<startDate>\\d{6})_(?<endDate>\\d{6})");
                if (match.Success)
                {
                    startDateString = match.Groups["startDate"].Value;
                    endDateString = match.Groups["endDate"].Value;
                }
            }

            if (String.IsNullOrEmpty(startDateString) == false && String.IsNullOrEmpty(endDateString) == false)
            {
                var startMonth = int.Parse(startDateString.Substring(2, 2));
                var endMonth = int.Parse((endDateString.Substring(2, 2)));
                var startYear = int.Parse(startDateString.Substring(0, 2)) + 2000;
                var endYear = int.Parse((endDateString.Substring(0, 2))) + 2000;

                var startMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(startMonth);
                var endMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(endMonth);

                result = startDateString + " - " + endDateString;

                if (startMonthName != endMonthName || startYear != endYear)
                {
                    if (startYear == endYear)
                    {
                        result += " (" + startMonthName + " - " + endMonthName + " " + startYear + ")";
                    }
                    else
                    {
                        result += " (" + startMonthName + " " + startYear + " - " + endMonthName + " " + endYear + ")";
                    }

                }
                else
                {
                    result += " (" + startMonthName + " " + startYear + ")";
                }
            }

            return result;
        }
    }
}