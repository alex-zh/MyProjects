using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace OpinionAnalyzer.UnitTests.Classes
{
    public class TestArticleLoader
    {
        public IEnumerable<TestArticle> LoadArticles(string pathToArticles)
        {
            var serializer = new XmlSerializer(typeof(TestArticleCollection));

            using (var textReader = new StreamReader(pathToArticles))
            {
                return ((TestArticleCollection)serializer.Deserialize(textReader)).Articles.ToList();
            }
        }
    }
}