using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpinionAnalyzer.Core;
using OpinionAnalyzer.Core.Classes;
using OpinionAnalyzer.Core.Providers;
using OpinionAnalyzer.UnitTests.Classes;

namespace OpinionAnalyzer.UnitTests.OpinionClassifierTests.Index.BeforeOpening
{
    [TestClass]
    public class UpTests
    {       
        [TestMethod]
        public void Up_Test1()
        {
            CorrectUp(@"TestData\Index\BeforeOpening\Index.BeforeOpening.Up.xml");
        }

        private static void CorrectUp(string testArticlesFilePath)
        {
            var macroWordsLoader = new MacroWordsLoader(@"AppData\MacroWords.xml");
            var patternsLoader = new RawPatternsLoader(@"AppData\BeforeOpening\Up.xml", Trend.Up);

            var patternProvider = new PatternsProvider(patternsLoader.Patterns, macroWordsLoader.MacroWords);

            var classifier = new OpinionClassifier(patternProvider);

            var articles = new TestArticleLoader().LoadArticles(testArticlesFilePath);

            foreach (var article in articles)
            {
                if (String.IsNullOrEmpty(article.Title) && String.IsNullOrEmpty(article.Text))
                {
                    continue;
                }

                var result = classifier.Classify(article);

                Assert.AreEqual(Trend.Up, result.Trend, "Incorrect classification for: " + article.Text +"\n\r + Regex: " +result.RegexText);

                if (String.IsNullOrEmpty(article.NegativeText) == false)
                {
                    var negativeResult = classifier.Classify(article.NegativeText);

                    Assert.AreNotEqual(Trend.Up, negativeResult.Trend, "Incorrect classification for negative text: " + article.Id + " : " + article.Text);
                }
            }
        }
    }
}
