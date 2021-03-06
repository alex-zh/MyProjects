using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpinionAnalyzer.Core.Classes;

namespace OpinionAnalyzer.Core
{
    public class PatternBuilder
    {
        private readonly IEnumerable<MacroWord> _macroWords;

        public PatternBuilder(IEnumerable<MacroWord> macroWords)
        {
            _macroWords = macroWords;
        }

        public string Build(string regexText)
        {
            var result = regexText;
            
            result = PattenCleaner.ReplaceCarretWrapup(result);

            foreach (var macroWord in _macroWords)
            {
                var name = GetMacroWordReplacementPattern(macroWord.Name);
                var value = " (" + macroWord.Value + ") ";

                result = Regex.Replace(result, name, value, RegexOptions.IgnoreCase);
            }

            //should be go after macro replacement because macro words can contains .*
            result = PattenCleaner.ReplaceDotStar(result);            

            result = PattenCleaner.DuplicateSimilarLetters(result);
            result = PattenCleaner.ReplaceWhiteSpaces(result);  

            return result;
        }

        
        private string GetMacroWordReplacementPattern(string macroWord)
        {
            const string prefix = "^|\\s|(?<=[(])";
            const string postfix = "$|\\s|(?=[)])";

            return "(" + prefix + ")" + macroWord + "(" + postfix + ")";
        }
    }
}