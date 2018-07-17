using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Documents;
using System.Xml;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace FortsPositionsViewer
{
    internal class FortsPositionsDataProvider
    {
        private readonly DeriativeNames _deriativeName;
        private readonly string _fortsOpenPositionsDataFilePath;

        public FortsPositionsDataProvider(DeriativeNames deriativeName)
        {
            _deriativeName = deriativeName;
            _fortsOpenPositionsDataFilePath = Config.GetFortsOpenPositionsDataFilePath(_deriativeName);
        }

        public void Write(OpenInterestData data)
        {
            string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
            });

            using (var stream = new StreamWriter(_fortsOpenPositionsDataFilePath))
            {
                stream.Write(json);
            }            
        }

        public OpenInterestData Read()
        {
            if (File.Exists(_fortsOpenPositionsDataFilePath) == false)
            {
                return new OpenInterestData();
            }

            using (var reader = new StreamReader(_fortsOpenPositionsDataFilePath))
            {
                var jsonText = reader.ReadToEnd();

                return JsonConvert.DeserializeObject<OpenInterestData>(jsonText);
            }
        }
    }
}