using System.IO;
using System.Xml.Serialization;

namespace Common
{
    public class Dumper<T>
    {
        private readonly string _path;

        public Dumper(string path)
        {
            _path = path;
        }

        public void Dump(T objectToDump)
        {
            var serializer = new XmlSerializer(typeof (T));

            using (TextWriter writer = new StreamWriter(_path, false))
            {
                serializer.Serialize(writer, objectToDump);    
            }                        
        }

        public void WriteLine(string line)
        {
            using (TextWriter writer = new StreamWriter(_path, true))
            {
                writer.WriteLine(line);
            }      
        }
    }
}
