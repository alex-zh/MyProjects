using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinancialIdeasTester.Classes;

namespace FinancialIdeasTester.Rules
{
    public class FirstHourRule : IRule
    {
        public List<FirstHourRuleResult> Calculate(List<CandleChange> candleChanges)
        {            
            DateTime veryFirstMondayDate = DateTime.Now;

            var resultList = new List<FirstHourRuleResult>();

            var daysTable = new List<int>();
            var previousDayDate = DateTime.MinValue;

            for (var i = 0; i < candleChanges.Count; i++)
            {
                var change = candleChanges[i];

                if ( change.Date.Date != previousDayDate)
                {
                    daysTable.Add(i);
                    previousDayDate = change.Date.Date;
                }
            }

            for (var i = 0; i < daysTable.Count; i++)
            {
                var dayIndex = daysTable[i];

                if (i < daysTable.Count - 1 && 
                    (daysTable[i + 1] - dayIndex > 9)) //difference between current day and next day is greater than 9 day
                {
                    Logger.Warn("Difference beetween days is more than 9");
                    continue;
                }

                var dayCandle = candleChanges[dayIndex];

                var dayResult = new FirstHourRuleResult();
                dayResult.DayOfWeek = dayCandle.Date.DayOfWeek;
                
                if (dayCandle.Date.Hour == 10)
                {
                    dayResult.FirstHourChange = dayCandle.ChangeFromPreviousClose;
                }
                else
                {
                    Logger.Warn("First hour is not 10 o'clock!");
                    continue;
                }

                if (candleChanges.Count <= dayIndex + 8)
                {
                    Logger.Warn("Last hour is not 18 o'clock!");
                    continue;
                }

                var dayLastCandle = candleChanges[dayIndex + 8];

                if (dayLastCandle.Date.Hour == 18)
                {
                    for (var hourIndex = dayIndex + 1; hourIndex <= dayIndex + 8; hourIndex++)
                    {
                        dayResult.DayChange += candleChanges[hourIndex].Change;
                    }
                }
                else
                {
                    Logger.Warn("Last hour is not 18 o'clock!");
                    continue;
                }

                resultList.Add(dayResult);
            }

            return resultList;
        }
    }
}
