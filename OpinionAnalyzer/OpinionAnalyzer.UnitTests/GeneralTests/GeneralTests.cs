using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpinionAnalyzer.Core;
using OpinionAnalyzer.Core.Classes;
using OpinionAnalyzer.Core.Providers;

namespace OpinionAnalyzer.UnitTests.GeneralTests
{
    [TestClass]
    public class GeneralTests
    {                
        [TestMethod]
        public void Initial()
        {
            var item = new Article("", "");

            var classifier = new OpinionClassifier(new PatternsProvider(new RawPattern[0], new MacroWord[0]));

            Trend result = classifier.Classify(item).Trend;

            Assert.AreEqual(Trend.Unknown, result);
        }
      
    }
}
