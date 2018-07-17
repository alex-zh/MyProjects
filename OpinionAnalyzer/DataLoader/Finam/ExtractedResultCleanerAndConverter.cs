using System;
using System.Text.RegularExpressions;

namespace OpinionAnalyzer.DataLoader.Finam
{
    class ExtractedResultCleanerAndConverter
    {
        public  string Clean(string htmlContent)
        {
            var result = htmlContent;

            result = CleanGeneral(result);

            return result;
        }

        private  string CleanGeneral(string result)
        {
            result = result.Replace("\r\n", "");
            result = result.Replace("\n", "");
            result = result.Replace("&nbsp;", "");
            result = Regex.Replace(result, @"\s+", " ");

            result = Regex.Replace(result, @"\<a.+?\<\/a", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"\<img.+?\<\/img", "", RegexOptions.IgnoreCase);
            return result;
        }

        /// <summary>
        ///         <div class="sm lightgrey mb20 mt15">
        /// 24.10.2014 14:35
        /// &nbsp;&nbsp;<a href="/analysis/nslent/">Новости и комментарии</a>
        ///         </div>
        /// </summary>
        public DateTime? GetDateTime(string elementContent)
        {
            var result = CleanGeneral(elementContent);

            var dateTimeMatch = Regex.Match(result,
                @"(?<day>\d{2})\.(?<month>\d{2})\.(?<year>\d{4})\s+(?<hour>\d{2})\:(?<minute>\d{2})");

            if (dateTimeMatch.Success)
            {
                int year = int.Parse(dateTimeMatch.Groups["year"].Value);
                int month = int.Parse(dateTimeMatch.Groups["month"].Value);
                int day = int.Parse(dateTimeMatch.Groups["day"].Value);

                int hour = int.Parse(dateTimeMatch.Groups["hour"].Value);
                int minute = int.Parse(dateTimeMatch.Groups["minute"].Value);

                return new DateTime(year, month, day, hour, minute, 0);
            }

            return null;
        }
    }
}
