using System.Collections.Generic;
using System.IO;
using Common.Classes.General;

namespace Common
{
    public class CandlesWriter
    {
        private readonly string _fileName;
        public string FirstLine = "<DATE>;<TIME>;<OPEN>;<HIGH>;<LOW>;<CLOSE>;<VOL>";

        public CandlesWriter(string fileName)
        {
            _fileName = fileName;
        }

        public void Write(IEnumerable<Candle> candles)
        {
            var directoryPath = Path.GetDirectoryName(_fileName);

            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }


            using (StreamWriter sw = new StreamWriter(_fileName))
            {
                sw.WriteLine(FirstLine);
                foreach (var candle in candles)
                {
                    sw.WriteLine(CreateCandleLine(candle));    
                }                
                }
            }

        private string CreateCandleLine(Candle candle)
        {
            string date = candle.Date.ToString("yyyyMMdd");
            string time = candle.Date.ToString("HHmmss");

            return $"{date}; {time}; {candle.Open}; {candle.High}; {candle.Low}; {candle.Close}; {candle.Volume}";
        }
    }       
}
