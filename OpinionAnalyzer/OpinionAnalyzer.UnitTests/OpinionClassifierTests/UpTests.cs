using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpinionAnalyzer.Core;
using OpinionAnalyzer.Core.Classes;
using OpinionAnalyzer.Core.Providers;
using OpinionAnalyzer.UnitTests.Classes;

namespace OpinionAnalyzer.UnitTests.OpinionClassifierTests
{
    [TestClass]
    public class UpTests
    {
        [TestMethod]
        public void CorrectUp()
        {
            var articlesFilePath = @"TestData\Index\MiddleTerm\Index.MiddleTerm.Up.xml";

            var macroWordsLoader = new MacroWordsLoader(@"AppData\MacroWords.xml");
            var patternsLoader = new RawPatternsLoader(@"AppData\Up.xml", Trend.Up);
            

            var patternProvider = new PatternsProvider(patternsLoader.Patterns,macroWordsLoader.MacroWords);

            var classifier = new OpinionClassifier(patternProvider);

            var articles = new TestArticleLoader().LoadArticles(articlesFilePath);

            foreach (var article in articles)
            {
                if (String.IsNullOrEmpty(article.Title) && String.IsNullOrEmpty(article.Text))
                    continue;

                Trend result = classifier.Classify(article).Trend;

                Assert.AreEqual(Trend.Up, result, "Incorrect classification for: " + article.Id + " : " + article.Text);
            }
        }
    }
}
