using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpinionAnalyzer.Core;
using OpinionAnalyzer.Core.Classes;
using OpinionAnalyzer.Core.Providers;
using OpinionAnalyzer.UnitTests.Classes;

namespace OpinionAnalyzer.UnitTests.OpinionClassifierTests.Index.BeforeOpening
{
    [TestClass]
    public class NeutralTests
    {       
        [TestMethod]
        public void Neutral_Test1()
        {
            CorrectDown(@"TestData\Index\BeforeOpening\Index.BeforeOpening.Neutral.xml");
        }

        private static void CorrectDown(string testArticlesFilePath)
        {
            var macroWordsLoader = new MacroWordsLoader(@"AppData\MacroWords.xml");
            var patternsLoader = new RawPatternsLoader(@"AppData\BeforeOpening\Neutral.xml", Trend.Neutral);

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

                Assert.AreEqual(Trend.Neutral, result.Trend, "Incorrect classification for: " + article.Id + " : " + article.Text);

                if (String.IsNullOrEmpty(article.NegativeText) == false)
                {
                    var negativeResult = classifier.Classify(article.NegativeText);

                    Assert.AreNotEqual(Trend.Neutral, negativeResult.Trend, "Incorrect classification for negative text: " + article.Id + " : " + article.Text);
                }
            }
        }
    }
}
