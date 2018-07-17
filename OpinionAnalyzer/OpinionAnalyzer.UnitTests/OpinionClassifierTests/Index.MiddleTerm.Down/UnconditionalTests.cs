using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpinionAnalyzer.Core;
using OpinionAnalyzer.Core.Classes;
using OpinionAnalyzer.Core.Providers;
using OpinionAnalyzer.UnitTests.Classes;

namespace OpinionAnalyzer.UnitTests.OpinionClassifierTests.Index.MiddleTerm.Down
{
    [TestClass]
    public class UnconditionalTests
    {
        [TestMethod]
        public void GeneralCases()
        {
            CorrectDown(@"TestData\Index\MiddleTerm\Down\Index.MiddleTerm.Unconditional.Down.xml");
        }

        [TestMethod]
        public void TechnicalAnalysisCases()
        {
            CorrectDown(@"TestData\Index\MiddleTerm\Down\Index.MiddleTerm.TechnicalAnalysis.Down.xml");
        }

        private static void CorrectDown(string testArticlesFilePath)
        {
            var patternsLoader = new RawPatternsLoader(@"AppData\TrendPatterns\Down.Unconditional.xml", Trend.Down);
            var macroWordsLoader = new MacroWordsLoader(@"AppData\TrendPatterns\MacroWords.xml");

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

                Assert.AreEqual(Trend.Down, result, "Incorrect classification for: " + article.Id + " : " + article.Text);

                if (String.IsNullOrEmpty(article.NegativeText) == false)
                {
                    var negativeResult = classifier.Classify(article.NegativeText);

                    Assert.AreNotEqual(Trend.Down, negativeResult, "Incorrect classification for negative text: " + article.Id + " : " + article.Text);
                }
            }
        }
    }
}
