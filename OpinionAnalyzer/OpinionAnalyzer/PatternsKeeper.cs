using System.Collections.Generic;
using System.Linq;
using OpinionAnalyzer.Core.Classes;
using OpinionAnalyzer.Core.Providers;

namespace OpinionAnalyzer.Core
{
    public static class BeforeOpeningPatternsKeeper
    {
        public static IEnumerable<Pattern> _patterns;
        private static string _market;
        private static string _expect;

        public static void LoadPatterns()
        {
            var macroWordsLoader = new MacroWordsLoader(@"AppData\MacroWords.xml");
            var upPatternsLoader = new RawPatternsLoader(@"AppData\BeforeOpening\Up.xml", Trend.Up);
            var downPatternsLoader = new RawPatternsLoader(@"AppData\BeforeOpening\Down.xml", Trend.Down);
            var neutralPatternsLoader = new RawPatternsLoader(@"AppData\BeforeOpening\Neutral.xml", Trend.Neutral);

            var patterns = new List<RawPattern>();

            patterns.AddRange(upPatternsLoader.Patterns);
            patterns.AddRange(downPatternsLoader.Patterns);
            patterns.AddRange(neutralPatternsLoader.Patterns);

            var patternProvider = new PatternsProvider(patterns, macroWordsLoader.MacroWords);

            _patterns = patternProvider.Patterns;
            _market = FringeWithSeparators(PattenCleaner.Clean(macroWordsLoader.MacroWords.Where(x => x.Name == "Market").Select(x => x.Value).Single()));
            _expect = FringeWithSeparators(PattenCleaner.Clean(macroWordsLoader.MacroWords.Where(x => x.Name == "Expect").Select(x => x.Value).Single()));
        }

        private static string FringeWithSeparators(string pattern)
        {
            return "(^|(?<=\\W))" + pattern + "($|(?=\\W))";
        }

        public static string Market
        {
            get
            {
                return _market;

            }
        }

        public static string Expect
        {
            get
            {
                return _expect;

            }
        }

        public static IEnumerable<Pattern> Patterns
        {
            get
            {
                return _patterns; 
                
            }
        }        
    }
}
