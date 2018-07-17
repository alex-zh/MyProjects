using System;
using System.Collections.Generic;
using FinancialIdeasTester.Classes;
using FinancialIdeasTester.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class MondaySecondHourRuleTests
    {
        [TestMethod]
        public void OneFullWeek()
        {
            var data = new List<CandleChange>();

            for (int day = 7; day <= 14; day++)
            {
                var date = new DateTime(2014, 4, day);

                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue;
                
                for (int hour = 10; hour <= 18; hour++)
                {                                      
                    data.Add(new CandleChange() { Date = new DateTime(date.Year, date.Month, date.Day, hour,0,0 ), Change = 1 });
                }
            }

            MondaySecondHourRule rule = new MondaySecondHourRule();
            var result = rule.Calculate(data);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].FirstHourChange);
            Assert.AreEqual(1, result[0].SecondHourChange);
            Assert.AreEqual(9, result[0].MondayChange);
            Assert.AreEqual(9*5, result[0].PreviousWeekChange);            
        }

        [TestMethod]
        public void TwoFullWeeks()
        {
            var data = new List<CandleChange>();

            for (int day = 6; day <= 22; day++)
            {
                var date = new DateTime(2014, 4, day);

                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                var change = 1;

                for (int hour = 10; hour <= 18; hour++)
                {
                    if (day > 6 && day <= 14)
                        change = 2;

                    if (day > 14)
                        change = 4;

                    if (hour == 11)
                        change += 2;

                    data.Add(new CandleChange() { Date = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0), Change = change });
                }
            }

            MondaySecondHourRule rule = new MondaySecondHourRule();
            var result = rule.Calculate(data);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[0].FirstHourChange);
            Assert.AreEqual(4, result[0].SecondHourChange);
            Assert.AreEqual(2 * 9 + 2, result[0].MondayChange);
            Assert.AreEqual((9*2 +2)* 5, result[0].PreviousWeekChange);

            Assert.AreEqual(4, result[1].FirstHourChange);
            Assert.AreEqual(6, result[1].SecondHourChange);
            Assert.AreEqual(9 * 4 + 2, result[1].MondayChange);
            Assert.AreEqual((9 * 2 + 2) + (9 * 4 + 2) * 4, result[1].PreviousWeekChange);
        }
    }
}
