using System;
using System.IO;
using System.Xml.Serialization;

namespace Robot.TesterApplication.Classes
{
    public class StatisticsHistoryResultsSaver
    {
        private readonly string _testResultsFolderPath;

        public StatisticsHistoryResultsSaver(string testResultsFolderPath)
        {
            if (Directory.Exists(testResultsFolderPath) == false)
            {
                throw new ArgumentException(testResultsFolderPath +" doesn't exist");
            }

            _testResultsFolderPath = testResultsFolderPath;
        }

        public void Save(string name, StrategyTestResult data)
        {            
            var serializer = new XmlSerializer(typeof(StrategyTestResult));            

            using (Stream stream = File.Create(GetFileName(name)))
            {
                serializer.Serialize(stream, data);
            }
        }

        private string GetFileName(string name)
        {
            return Path.Combine(_testResultsFolderPath, name + ".xml");
        }
    }
}