using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using OpinionAnalyzer.DataLoader;

namespace OpinionAnalyzer.Viewer
{
    internal class ArticleFromFileLoader
    {
        private readonly string _dataPath;

        public ArticleFromFileLoader(string dataPath)
        {
            _dataPath = dataPath;
        }

        public IEnumerable<LoadedArticle> LoadForDate(DateTime selectedDate)
        {
            var result = new List<LoadedArticle>();

            var folderPath = GetFolderPath(selectedDate);

            if (Directory.Exists(folderPath) == false)
            {
                return result;
            }

            var files = Directory.GetFiles(folderPath, "*.xml", SearchOption.TopDirectoryOnly);

            var serializer = new XmlSerializer(typeof (LoadedArticle));

            foreach (var file in files)
            {
                using (Stream stream = File.OpenRead(file))
                {
                    var article = (LoadedArticle) serializer.Deserialize(stream);
                    result.Add(article);
                }
            }

            return result;
        }

        private string GetFolderPath(DateTime selectedDate)
        {
            return String.Format(@"{0}\{1}\{2}\{3}", _dataPath, selectedDate.Year, selectedDate.Month, selectedDate.Day);
        }   
    }
}