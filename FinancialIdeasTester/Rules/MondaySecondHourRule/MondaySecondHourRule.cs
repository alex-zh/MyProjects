using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinancialIdeasTester.Classes;

namespace FinancialIdeasTester.Rules
{
    public class MondaySecondHourRule : IRule
    {
        public List<MondayResult> Calculate(List<CandleChange> candleChanges)
        {            
            DateTime veryFirstMondayDate = DateTime.Now;

            var result = new List<MondayResult>();

            var mondaysTable = new List<int>();
            var previousMondayDate = DateTime.MinValue;

            for (var i = 0; i < candleChanges.Count; i++)
            {
                var change = candleChanges[i];

                if (change.Date.DayOfWeek == DayOfWeek.Monday && change.Date.Date != previousMondayDate)
                {
                    mondaysTable.Add(i);
                    previousMondayDate = change.Date.Date;
                }
            }

            for (var i = 1; i < mondaysTable.Count; i++)
            {
                var mondayIndex = mondaysTable[i];
                var previousMondayIndex = mondaysTable[i - 1];

                if (mondayIndex + 9 > candleChanges.Count)
                {
                    break;
                }
                if (mondayIndex - previousMondayIndex > 6*9)
                {
                    Logger.Warn("Difference beetween monday indexes is more than 6 days");
                    continue;
                }

                var mondayResult = new MondayResult();

                var mondayFirstCandle = candleChanges[mondayIndex];

                if (mondayFirstCandle.Date.Hour == 10)
                {
                    mondayResult.FirstHourChange = mondayFirstCandle.Change;
                }
                else
                {
                    Logger.Warn("Monday first hour is not 10 o'clock!");
                    continue;
                }

                var mondaySecondCandle = candleChanges[mondayIndex + 1];

                if (mondaySecondCandle.Date.Hour == 11)
                {
                    mondayResult.SecondHourChange = mondaySecondCandle.Change;
                }
                else
                {
                    Logger.Warn("Monday second hour is not 11 o'clock!");
                    continue;
                }

                var mondayLastCandle = candleChanges[mondayIndex + 8];

                if (mondayLastCandle.Date.Hour == 18)
                {
                    for (var dayIndex = mondayIndex; dayIndex <= mondayIndex + 8; dayIndex++)
                    {
                        mondayResult.MondayChange += candleChanges[dayIndex].Change;
                    }
                }
                else
                {
                    Logger.Warn("Monday Last hour is not 18 o'clock!");
                    continue;
                }
            
                for (var j = previousMondayIndex; j < mondayIndex; j++)
                {
                    mondayResult.PreviousWeekChange += candleChanges[j].Change;
                }

                result.Add(mondayResult);
            }

            return result;
        }
    }
}
