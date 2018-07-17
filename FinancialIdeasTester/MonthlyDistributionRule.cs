using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancialIdeasTester.Rules;

namespace FinancialIdeasTester
{
    class MonthlyDistributionRule:IRule
    {
        private DataHolder _usdrubDataHolder;

        public MonthlyDistributionRule(DataHolder usdrubDataHolder)
        {
            this._usdrubDataHolder = usdrubDataHolder;
        }

        public MonthlyDistributionResult Calculate()
        {
            var collectedStats = new Dictionary<int, List< double>>();

            //var result = new MonthlyDistributionResult();

            for (int i = 0; i < _usdrubDataHolder.Changes.Count; i++)
            {
                var change = _usdrubDataHolder.Changes[i];

                var year = change.Date.Year - 2000;
                if (!(year == 13 || year == 14 || year == 12))
                {
                    continue;
                }

                if (change.Date.Year > 2010)
                {
                    //break;
                }
                
                if (!collectedStats.ContainsKey(change.Date.Day))
                {
                    collectedStats.Add(change.Date.Day, new List<double> { change.Change });    
                }
                else
                {
                    collectedStats[change.Date.Day].Add(change.Change);
                }
            }

            var result = new MonthlyDistributionResult();
            result.Days = new Dictionary<int, double>();

            foreach (int key in collectedStats.Keys)
            {

                result.Days.Add(key, collectedStats[key].Sum() / collectedStats[key].Count);
            }

            return result;
        }
    }
}
