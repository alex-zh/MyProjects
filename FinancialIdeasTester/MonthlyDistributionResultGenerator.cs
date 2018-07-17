using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FinancialIdeasTester.Rules.BrentToUsdRubRatio;

namespace FinancialIdeasTester
{
 
    public class MonthlyDistributionResultGenerator
    {
        internal void WriteData(MonthlyDistributionResult result)
        {
            var directoryPath = @"C:\Users\aaz\Documents\R\Data";
            var resultDataPath = directoryPath + "\\" + "MonthlyDistribution.txt";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var fileStream = new FileStream(resultDataPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.WriteLine("Day Return");

                    foreach (var record in result.Days)
                    {
                        writer.WriteLine(GetFormatedRecord(record));
                    }
                }
            }
        }

        private string GetFormatedRecord(KeyValuePair<int, double> record)
        {
            return string.Format("{0} {1}",
                                record.Key,
                                GetRoundValue(record.Value));
        }

      
        private string GetRoundValue(double p)
        {
            return Math.Round(p * 100000).ToString();
        }
    }
}
