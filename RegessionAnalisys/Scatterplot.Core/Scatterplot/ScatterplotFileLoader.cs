using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.Scatterplot
{
    /// <summary>
    /// Format is the following:
    /// x y category* group* date*
    /// </summary>
    public class ScatterplotFileLoader
    {        
        public ScatterplotData Load(string fileName)
        {
            var names = new List<string>();

            var result = new ScatterplotData();

            var file = new FileInfo(fileName);

            var isFirstLine = true;

            using (StreamReader sr = file.OpenText())
            {
                string line;

                while (String.IsNullOrEmpty(line = sr.ReadLine()) == false)
                {
                    if (isFirstLine)
                    {                        
                        names = ParseFirstLine(line);

                        result.LableX = names[0];
                        result.LableY = names[1];
                        
                        isFirstLine = false;
                        continue;
                    }

                    var point = ParseLine(line, (names.Count > 2 && names[2].ToLower() == "group"), (names.Count > 3 && names[3].ToLower() == "subgroup"), (names.Count > 4 && names[4].ToLower() == "date"));
                    
                    result.Points.Add(point);
                }
            }

            return result;
        }
    
        private List<string> ParseFirstLine(string line)
        {            
            line = line.Trim();
            return line.Split(new[] { " ", "\t" }, StringSplitOptions.None).Select(x => x.ToLower()).ToList();
        }

        private ScatterplotPoint ParseLine(string line, bool parseGroup, bool parseSubGroup, bool parseDate)
        {
            int group = 0;
            int subGroup = 0;
            DateTime? date = null;
            double firstValue, secondValue;
            
            if (String.IsNullOrEmpty(line))
            {
                throw new ArgumentNullException("line");
            }

            var data = line.Split(new[] { " ", "\t" }, StringSplitOptions.None);            
                                    
            if (Double.TryParse(data[0], out firstValue) == false)
            {
                throw new ArgumentException("First parameter is not double: " + data[0]);
            }

            if (Double.TryParse(data[1], out secondValue) == false)
            {
                throw new ArgumentException("Second parameter is not double: " + data[1]);
            }

            if (parseGroup)
            {            
                if (int.TryParse(data[2], out group) == false)
                {
                    throw new ArgumentException("Group column is present but has incorrect value: " + data[2]);
                }   
            }

            if (parseSubGroup)
            {
                if (int.TryParse(data[3], out subGroup) == false)
                {
                    throw new ArgumentException("SubGroup column is present but has incorrect value: " + data[3]);
                }
            }

            if (parseDate)
            {
                DateTime parsedDate;
                if (DateTime.TryParse(data[4], out parsedDate) == false)
                {
                    throw new ArgumentException("Group column is present but has incorrect value: " + data[4]);
                }

                date = parsedDate;
            }

            return new ScatterplotPoint
            {
               X = firstValue,
               Y = secondValue,
               Group = group,
               SubGroup = subGroup,
               Date = date
            };            
        }
    }
}