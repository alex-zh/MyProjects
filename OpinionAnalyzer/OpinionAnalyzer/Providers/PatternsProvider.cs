using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpinionAnalyzer.Core.Classes;

namespace OpinionAnalyzer.Core.Providers
{
    public class PatternsProvider : IPatternsProvider
    {
        private readonly IEnumerable<RawPattern> _rawPatterns;
        private readonly IEnumerable<MacroWord> _macroWords;

        private List<Pattern> _patterns = new List<Pattern>();

        public PatternsProvider(IEnumerable<RawPattern> rawPatterns, IEnumerable<MacroWord> macroWords)
        {
            _rawPatterns = rawPatterns;
            _macroWords = macroWords;            

            Build();
        }

        public IEnumerable<Pattern> Patterns
        {
            get
            {
                return _patterns;
            }
        }

        private void Build()
        {
            foreach (var rawPattern in _rawPatterns)
            {
                _patterns.Add(BuildPattern(rawPattern));
            }
            _patterns = _patterns.OrderByDescending(x => x.Priority).ToList();
        }

        private Pattern BuildPattern(RawPattern rawPattern)
        {
            var regexText = new PatternBuilder(_macroWords).Build(rawPattern.RegexText);

            return new Pattern
            {
                Trend = rawPattern.Trend,
                Weight = rawPattern.Weight,
                Priority =  rawPattern.Priority,
                Regex = new Regex(regexText, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase),
                RawRegexText =  rawPattern.RegexText
            };
        }     
    }
}
