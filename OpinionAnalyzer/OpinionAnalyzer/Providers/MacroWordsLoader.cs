using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using OpinionAnalyzer.Core.Classes;

namespace OpinionAnalyzer.Core.Providers
{
    public class MacroWordsLoader
    {
        private List<MacroWord> _macroWords = new List<MacroWord>();

        public MacroWordsLoader(string path)
        {
            Load(path);
        }

        public List<MacroWord> MacroWords
        {
            get { return _macroWords; }
        }

        private void Load(string path)
        {
            var serializer = new XmlSerializer(typeof (MacroWordsCollection));

            using (var textReader = new StreamReader(path))
            {
                _macroWords = ((MacroWordsCollection)serializer.Deserialize(textReader))
                               .MacroWords                               
                               .Where(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.Value)).ToList();
            }

            _macroWords.ForEach(x =>
            {
                x.Name = Clean(x.Name);   
                x.Value = Clean(x.Value);                
            });
        }

        private string Clean(string text)
        {
            text =  Regex.Replace(text, @"^\s*[\n\r]?\s*", "");
            text = Regex.Replace(text, @"\s*[\n\r]?\s*$", "");

            return text;
        }
    }
}
