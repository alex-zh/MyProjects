using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpinionAnalyzer.Core;
using OpinionAnalyzer.Core.Classes;
using OpinionAnalyzer.Core.Providers;
using OpinionAnalyzer.UnitTests.Classes;

namespace OpinionAnalyzer.UnitTests.OpinionClassifierTests.Index.BeforeOpening
{
    [TestClass]
    public class UnknownTests
    {       
        [TestMethod]
        public void UnknownForUp()
        {
            Check(@"TestData\Index\BeforeOpening\Index.BeforeOpening.Unknown.xml",
                @"AppData\BeforeOpening\Up.xml", Trend.Up);
        }

        [TestMethod]
        public void UnknownForDown()
        {
            Check(@"TestData\Index\BeforeOpening\Index.BeforeOpening.Unknown.xml",
                @"AppData\BeforeOpening\Down.xml", Trend.Down);
        }

        [TestMethod]
        public void UnknownForNeutral()
        {
            Check(@"TestData\Index\BeforeOpening\Index.BeforeOpening.Unknown.xml",
                @"AppData\BeforeOpening\Neutral.xml", Trend.Neutral);
        }

        private static void Check(string testArticlesFilePath, string rawPatternsPath, Trend trend)
        {
            var macroWordsLoader = new MacroWordsLoader(@"AppData\MacroWords.xml");
            var patternsLoader = new RawPatternsLoader(rawPatternsPath, trend);

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

                Assert.AreEqual(Trend.Unknown, result.Trend, "Incorrect classification: Regex: " + result.RegexText + "\n\r : " + article.Text + " \n\r " + "Regex: " + result.RegexText);

                if (String.IsNullOrEmpty(article.NegativeText) == false)
                {
                    var negativeResult = classifier.Classify(article.NegativeText);

                    Assert.AreNotEqual(Trend.Unknown, negativeResult, "Incorrect classification for negative text: " + article.Id + " : " + article.Text);
                }
            }
        }
    }
}
