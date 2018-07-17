using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FinancialIdeasTester.Rules;

namespace FinancialIdeasTester
{
    public class FirstHourResultGenerator
    {
        //internal void WriteData(List<FirstHourRuleResult> result)
        //{
        //    var resultDataPath = @"C:\Trade.Results\" + "FirstHourResult.txt";
            
        //    using (var writer = new StreamWriter(resultDataPath))
        //    {
        //        writer.WriteLine("FirstHourChange DayChange DayName");

        //        foreach(var record in result)
        //        {
        //            writer.WriteLine(GetFormatedRecord(record));
        //        }                
        //    }
        //}

        //private string GetFormatedRecord(FirstHourRuleResult record)
        //{
        //    return string.Format("{0} {1} {2}",
        //                          GetRoundValue(record.FirstHourChange),                                  
        //                          GetRoundValue(record.DayChange),
        //                          record.DayOfWeek.ToString());
        //}

        //private string GetRoundValue(double p)
        //{
        //    return Math.Round(p * 100000).ToString();
        //}
    }
}
