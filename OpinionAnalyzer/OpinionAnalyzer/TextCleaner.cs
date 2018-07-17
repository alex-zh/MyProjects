using System.Text.RegularExpressions;

namespace OpinionAnalyzer.Core
{
    public class TextCleaner
    {
        public static string Clean(string text)
        {
            var result = text;

            //заменяем . для цифр 0.5 иначе такие предложения не находятся
            result = Regex.Replace(result, @"(?<=\d)\.(?=\d)", " ");

            result = Regex.Replace(result, "[\"':*<>,]", " ");
            result = Regex.Replace(result, "\\s+", " ");

            return result;
        }
    }
}