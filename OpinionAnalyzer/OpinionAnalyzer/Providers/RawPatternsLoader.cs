using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using OpinionAnalyzer.Core.Classes;

namespace OpinionAnalyzer.Core.Providers
{
    public  class RawPatternsLoader
    {
        private readonly Trend _trend;
        protected readonly List<RawPattern> _patterns = new List<RawPattern>();

        public List<RawPattern> Patterns
        {
            get { return _patterns; }
        }
        
        public RawPatternsLoader(string path, Trend trend)
        {
            _trend = trend;

            Load(path);
        }

        private void Load(string path)
        {            
            var serializer = new XmlSerializer(typeof(OpinionPatternCollection));

            using (var textReader = new StreamReader(path))
            {
                _patterns.AddRange(((OpinionPatternCollection)serializer.Deserialize(textReader)).RawPatterns
                    .Where(x => String.IsNullOrEmpty(x.RegexText) == false));                
            }

            _patterns.ForEach(x =>
            {
                x.Trend = _trend;
                x.RegexText = Clean(x.RegexText);            
            });
        }


        private string Clean(string text)
        {
            text = Regex.Replace(text, @"^\s*[\n\r]?\s*", "");
            text = Regex.Replace(text, @"\s*[\n\r]?\s*$", "");

            return text;
        }

    }
}