using System;
using System.Collections.Generic;
using System.IO;
using RegessionAnalisys.Properties;

namespace RegessionAnalisys
{
    public class DataFilesProvider
    {
        public List<KeyValuePair<string, string>> GetList()
        {
            var result= new List<KeyValuePair<string, string>> ();
            var filesPaths = Directory.GetFiles(Settings.Default.PathToFiles);

            foreach (var path in filesPaths)
            {
                var name = Path.GetFileNameWithoutExtension(path);
                result.Add(new KeyValuePair<string, string>(name, path));                
            }

            result.Sort(new FileNamesComparer());

            return result;
        }
    }

    public class FileNamesComparer : IComparer<KeyValuePair<string, string>>
    {
        public int Compare(KeyValuePair<string,string> x, KeyValuePair<string,string> y)
        {
            var xDate = ParseStringToDateTime(x.Key);
            var yDate = ParseStringToDateTime(y.Key);

            return xDate.CompareTo(yDate);
        }

        private DateTime ParseStringToDateTime(string value)
        {
            var parts = value.Split('_');
            var day = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            var year = int.Parse(parts[2]);

            return new DateTime(year,month, day);
        }
    }
}