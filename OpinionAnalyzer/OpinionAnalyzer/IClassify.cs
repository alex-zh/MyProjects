using OpinionAnalyzer.Core.Classes;

namespace OpinionAnalyzer.Core
{
    public interface IClassify
    {
        ClassificationResult Classify(IArticle item);
    }

    public class ClassificationResult
    {
        public Trend Trend
        {
            get;
            set;
        }

        public string RegexText
        {
            get;
            set;
        }
    }
}
