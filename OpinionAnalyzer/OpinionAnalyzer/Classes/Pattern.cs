using System.Text.RegularExpressions;

namespace OpinionAnalyzer.Core.Classes
{
    public class Pattern
    {        
        public Trend Trend { get; set; }
     
        public int Weight { get; set; }

        public Regex Regex { get; set; }

        public string RawRegexText { get; set; }

        public int Priority { get; set; }
    }
}