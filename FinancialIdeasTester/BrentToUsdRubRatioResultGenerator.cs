using System;
using System.Collections.Generic;
using System.IO;
using FinancialIdeasTester.Rules.BrentToUsdRubRatio;

namespace FinancialIdeasTester
{
    public class BrentToUsdRubRatioResultGenerator
    {
        internal void WriteData(List<BrentToUsdRubRatioRuleResult> result)
        {
            var directoryPath = @"C:\Users\aaz\Documents\R\Data";
            var resultDataPath =  directoryPath+"\\" + "BrentToUsdRubRatio.txt";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var fileStream = new FileStream(resultDataPath, FileMode.Create))
            {                
                using (var writer = new StreamWriter(fileStream))
                {
                      writer.WriteLine("RatioChange NextRatioChange Col");

                    foreach (var record in result)
                    {
                        writer.WriteLine(GetFormatedRecord(record));
                    }
                }
            }
        }

        private string GetFormatedRecord(BrentToUsdRubRatioRuleResult record)
        {
            return string.Format("{0} {1} {2}",
                                  GetRoundValue(record.RatioChange),
                                  GetRoundValue(record.NextRatioChange),
                                  record.DateTime.Year - 2008);
        }

        private string GetRoundValue(double p)
        {
            return Math.Round(p * 100000).ToString();
        }
    }
}