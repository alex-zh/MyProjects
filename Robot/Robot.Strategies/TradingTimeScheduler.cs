using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.Strategies
{
    public class TradingTimeScheduler
    {
        public static bool IsTimeToExit(DateTime dateTime)
        {
            if (dateTime.Hour == 23 && dateTime.Minute > 30)
            {
                return true;
            }

            if (dateTime.Hour == 18 && dateTime.Minute > 42)
            {
                return true;
            }

            return false;
        }


        public static bool IsLateToOpenPostion(DateTime dateTime)
        {
            if (IsEndOfDay(dateTime))
            {
                return true;
            }

            if (IsTimeBeforeEveningClearing(dateTime))
            {
                return true;
            }
                       
            if (IsTimeBeforeRigsDataPublication(dateTime))
            {
                return true;
            }

            if (IsTimeBeforeEIAInvetoriesPublication(dateTime))
            {
               // return true;
            }
            return false;
        }



        private static  bool IsTimeBeforeEIAInvetoriesPublication(DateTime dateTime)
        {
            var datesInString =
                "15.10.2015;07.10.2015;30.09.2015;23.09.2015;16.09.2015;10.09.2015;02.09.2015;26.08.2015;19.08.2015;12.08.2015;05.08.2015;29.07.2015;22.07.2015;15.07.2015;08.07.2015;01.07.2015;24.06.2015;17.06.2015;10.06.2015;03.06.2015;28.05.2015;20.05.2015;13.05.2015;06.05.2015;29.04.2015;22.04.2015;15.04.2015;08.04.2015;01.04.2015;25.03.2015;18.03.2015;11.03.2015;04.03.2015;19.02.2015;11.02.2015;04.02.2015;28.01.2015;22.01.2015;14.01.2015;07.01.2015;31.12.2015;24.12.2015;17.12.2015;10.12.2015;03.12.2015;26.11.2015;19.11.2015;13.11.2015;05.11.2015;29.10.2015;22.10.2015;16.10.2015;08.10.2015;01.10.2015;24.09.2015;17.09.2015;10.09.2015;04.09.2015;27.08.2015;20.08.2015;13.08.2015;06.08.2015;30.07.2015;23.07.2015;16.07.2015;09.07.2015;02.07.2015;25.06.2015;18.06.2015;11.06.2015;04.06.2015;29.05.2015;21.05.2015;14.05.2015;07.05.2015;30.04.2015;23.04.2015;16.04.2015;09.04.2015;02.04.2015;26.03.2015;19.03.2015;12.03.2015;05.03.2015;26.02.2015;20.02.2015;12.02.2015;05.02.2015;29.01.2015;23.01.2015;15.01.2015;08.01.2015;03.01.2015";

            var dates = datesInString.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).Select(x =>
            {
                var parts = x.Split(new[] {'.'});
                return new DateTime(int.Parse(parts[2]), int.Parse(parts[1]), int.Parse(parts[0]));
            });

            if (dates.Any(x => x.Year == dateTime.Year && x.Month == dateTime.Month && x.Day == dateTime.Day))
            {
                if (dateTime.Hour >= 16 && dateTime.Hour < 18)
                {
                    return true;
                }
            }

            return false;
        }
        

        /// <summary>
        /// Baker Huges publishes rig count data at noon center dateTime on each friday
        /// People are waiting this data and market is sleeping.
        /// </summary>
        private static bool IsTimeBeforeRigsDataPublication(DateTime dateTime)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Friday && (dateTime.Hour >= 18 && dateTime.Hour < 20))
            {
                return true;
            }
            return false;
        }

        private static bool IsTimeBeforeEveningClearing(DateTime time)
        {
            if (time.Hour == 18 && time.Minute > 42)
            {
                return true;
            }

            return false;
        }

        private static bool IsEndOfDay(DateTime time)
        {
            if (time.Hour >= 23)
            {
                return true;
            }

            return false;
        }
    }
}
