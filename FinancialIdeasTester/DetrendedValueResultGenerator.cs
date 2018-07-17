using System;
using System.IO;
using FinancialIdeasTester.Classes;

namespace FinancialIdeasTester
{
    internal class DetrendedValueResultGenerator
    {
        internal void WriteData(DataHolder result)
        {
            var directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\R\Data";
            var resultDataPath = directoryPath + "\\" + "Result.txt";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var fileStream = new FileStream(resultDataPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.WriteLine("V DV");

                    for (int i=0; i< result.Candles.Count; i++)
                    {
                        writer.WriteLine("{0} {1}", result.Candles[i].Close, result.DetrenedValues[i].Value);
                    }
                }
            }
        }     

        private string GetRoundValue(double p)
        {
            return Math.Round(p * 100000).ToString();
        }
    }
}