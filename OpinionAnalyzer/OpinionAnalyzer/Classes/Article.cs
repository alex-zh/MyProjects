using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpinionAnalyzer.Core.Classes
{
    public class Article : IArticle
    {
        private const string SentencePattern = @"(?<=[\.!\?])(\s+|$)";

        private List<Sentence> _sentences;

        public Article(string title, string text)
        {
            Title= title;
            Text = text;
        }

        public string Title { get; private set; }
        public string Text{ get; private set; }

        public List<Sentence> Sentences
        {
            get
            {
                if (_sentences != null) return _sentences;


                _sentences = new List<Sentence>();

                if (string.IsNullOrEmpty(Text) == false)
                {
                    _sentences = Regex.Split(Text, SentencePattern).Select(x => new Sentence(x)).ToList();   
                }

                return _sentences;
            }
        }
    }
}