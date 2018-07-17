using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialIdeasTester.Rules.BrentToUsdRubRatio
{
    public class BrentToUsdRubRatioRuleResult
    {
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Brent/(Usd/Rub) ration change during n {2, 4, 6, 8} days after DateTime
        /// </summary>
        public double RatioChange { get; set; }

        public double NextRatioChange { get; set; }

        public double BrentAverageChange { get; set; }

        /// <summary>
        /// Usd/Rub change during k {1, 2, 3, 4} days ofter n days
        /// </summary>
        public double UsdRubChange { get; set; }
     }
}
