namespace RegessionAnalisys
{
    public static class StringExtensions
    {
        public static string ToUpperFirstLetter(this string text)
        {
            return text.Substring(0, 1).ToUpper() + text.Substring(1);
        }
    }
}
