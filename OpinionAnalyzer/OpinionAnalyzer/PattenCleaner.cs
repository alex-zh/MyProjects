using System.Text.RegularExpressions;

namespace OpinionAnalyzer.Core
{
    public class PattenCleaner
    {
        public static string Clean(string result)
        {
            result = ReplaceDotStar(result);
            result = ReplaceCarretWrapup(result);
            result = DuplicateSimilarLetters(result);
            result = ReplaceWhiteSpaces(result);

            return result;
        }

        public  static string ReplaceDotStar(string result)
        {            
            result = result.Replace(" .* ", " [^.]* ");
            result = result.Replace(" .{", " [^.]{");

            return result;
        }

        public static string ReplaceCarretWrapup(string result)
        {
            result = result.Replace("\n\r", " ");
            result = result.Replace("\n", " ");

            return result;
        }

        public static string DuplicateSimilarLetters(string result)
        {
            result = Regex.Replace(result, "[е]", "[её]");

            return result;
        }

        public static string ReplaceWhiteSpaces(string result)
        {
            result = Regex.Replace(result, "\\s+", "\\s*");
            return result;
        }
    }
}