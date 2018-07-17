using System;
using OpinionAnalyzer.Core.Classes;
using OpinionAnalyzer.Core.Providers;

namespace OpinionAnalyzer.Core
{
    public class OpinionClassifier : IClassify
    {
        private readonly IPatternsProvider _provider;

        public OpinionClassifier(IPatternsProvider provider)
        {
            _provider = provider;
        }

        public ClassificationResult Classify(IArticle article)
        {
            var articleText = TextCleaner.Clean(article.Text);

            return Classify(articleText);
        }

        public ClassificationResult Classify(String text)
        {
            var result = new ClassificationResult() { Trend = Trend.Unknown };

            foreach (var pattern in _provider.Patterns)
            {
                var articleText = TextCleaner.Clean(text);

                if (pattern.Regex.IsMatch(articleText))
                {
                    return new ClassificationResult() { Trend = pattern.Trend, RegexText = pattern.RawRegexText};
                }
            }

            return result;
        }
    }
}
