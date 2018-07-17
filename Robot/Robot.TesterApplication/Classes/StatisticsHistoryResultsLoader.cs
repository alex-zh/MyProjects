using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Robot.TesterApplication.Classes
{
    public class StatisticsHistoryResultsLoader
    {
        public StrategyTestResult Load(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                return null;
            }
            var serializer = new XmlSerializer(typeof(StrategyTestResult));

            using (Stream stream = File.OpenRead(filePath))
            {
                return (StrategyTestResult)serializer.Deserialize(stream);
            }
        }

        public IEnumerable<KeyValuePair<string, string>> LoadPaths(string folderPath)
        {
            var result = new List<KeyValuePair<string, string>>();

            if (Directory.Exists(folderPath) == false)
            {
                return result;
            }

            var files = Directory.GetFiles(folderPath, "*.xml", SearchOption.TopDirectoryOnly);

            result.AddRange(files.Select(file => new KeyValuePair<string, string>(Path.GetFileNameWithoutExtension(file), Path.Combine(folderPath, file))));

            return result;
        }
    }
}
