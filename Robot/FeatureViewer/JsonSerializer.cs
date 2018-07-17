using System.IO;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

namespace Viewer
{
    public class JsonSerializer<T> where T: new()
    {
        private readonly string _filePath;

        public JsonSerializer(string filePath)
        {
            _filePath = filePath;
        }

        public void Write(T data)
        {
            string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
            });

            using (var stream = new StreamWriter(_filePath))
            {
                stream.Write(json);
            }
        }

        public T Read()
        {
            if (File.Exists(_filePath) == false)
            {
                return new T();
            }

            using (var reader = new StreamReader(_filePath))
            {
                var jsonText = reader.ReadToEnd();

                return JsonConvert.DeserializeObject<T>(jsonText);
            }
        }
    }
}