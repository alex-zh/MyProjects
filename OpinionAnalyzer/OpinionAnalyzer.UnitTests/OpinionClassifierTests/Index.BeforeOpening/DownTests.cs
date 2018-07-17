using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpinionAnalyzer.Core;
using OpinionAnalyzer.Core.Classes;
using OpinionAnalyzer.Core.Providers;
using OpinionAnalyzer.UnitTests.Classes;

namespace OpinionAnalyzer.UnitTests.OpinionClassifierTests.Index.BeforeOpening
{
    [TestClass]
    public class DownTests
    {       
        [TestMethod]
        public void Down_Test1()
        {
            CorrectDown(@"TestData\Index\BeforeOpening\Index.BeforeOpening.Down.xml");
        }

        private static void CorrectDown(string testArticlesFilePath)
        {
            var macroWordsLoader = new MacroWordsLoader(@"AppData\MacroWords.xml");
            var patternsLoader = new RawPatternsLoader(@"AppData\BeforeOpening\Down.xml", Trend.Down);

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

                Assert.AreEqual(Trend.Down, result.Trend, "Incorrect classification for: "  + article.Text +"\n\r + Pattern: " + result.RegexText);

                if (String.IsNullOrEmpty(article.NegativeText) == false)
                {
                    var negativeResult = classifier.Classify(article.NegativeText);

                    Assert.AreNotEqual(Trend.Down, negativeResult.Trend, "Incorrect classification for negative text: " + article.Id + " : " + article.Text);
                }
            }
        }
    }
}
