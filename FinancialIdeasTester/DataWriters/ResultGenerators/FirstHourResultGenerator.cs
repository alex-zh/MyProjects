using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FinancialIdeasTester.Rules;

namespace FinancialIdeasTester
{
    class DataWriter
    {
        internal void WriteData(string fileName, List<MondayResult> result)
        {
            var resultDataPath = @"C:\Trade.Results\" + fileName;
            
            using (var writer = new StreamWriter(resultDataPath))
            {
                writer.WriteLine("FirstHourChange SecondHourChange MondayChange PreviousWeekChange");

                foreach(var record in result)
                {
                    writer.WriteLine(GetFormatedRecord(record));
                }
                
            }
        }

        private string GetFormatedRecord(Rules.MondayResult record)
        {
            return string.Format("{0} {1} {2} {3}",
                                  GetRoundValue(record.FirstHourChange),
                                  GetRoundValue(record.SecondHourChange),
                                  GetRoundValue(record.MondayChange),
                                  GetRoundValue(record.PreviousWeekChange));
        }

        private string GetRoundValue(double p)
        {
            return Math.Round(p * 100000).ToString();
        }
    }
}
