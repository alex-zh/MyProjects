using System;
using System.Collections.Generic;
using FinancialIdeasTester.Classes;
using FinancialIdeasTester.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class FirstHourRuleTests
    {
        [TestMethod]
        public void FourDays()
        {
            var data = new List<CandleChange>();

            var daysCount = 2;

            for (int day = 7; day < 7 + daysCount; day++)
            {
                var date = new DateTime(2014, 4, day);

                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue;
                
                for (int hour = 10; hour <= 18; hour++)
                {                                      
                    data.Add(new CandleChange() { Date = new DateTime(date.Year, date.Month, date.Day, hour,0,0 ), Change = 1, ChangeFromPreviousClose = 1 });
                }
            }

            var rule = new FirstHourRule();
            var result = rule.Calculate(data);

            Assert.AreEqual(daysCount, result.Count);
            Assert.AreEqual(1, result[0].FirstHourChange);
            Assert.AreEqual(8, result[0].DayChange);
            Assert.AreEqual(DayOfWeek.Monday, result[0].DayOfWeek);

            Assert.AreEqual(1, result[1].FirstHourChange);
            Assert.AreEqual(8, result[1].DayChange);
            Assert.AreEqual(DayOfWeek.Tuesday, result[1].DayOfWeek);            
        }     
    }
}
