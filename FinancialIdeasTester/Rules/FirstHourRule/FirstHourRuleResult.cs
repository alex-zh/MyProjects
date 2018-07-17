using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialIdeasTester.Rules
{
    public class FirstHourRuleResult
    {
        public double FirstHourChange { get; set; }
        public double DayChange { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}
