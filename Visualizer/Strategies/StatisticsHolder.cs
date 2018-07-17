using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategies
{
    public class StatisticsHolder
    {
        private List<StrategyRealization> _list = new List<StrategyRealization>();

        public StatisticsHolder()
        {         
        }

        public int LookingForwardDaysNumber
        {
            get { return _list[0].Offsets.Count; }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public List<StrategyRealization> Realizations
        {
            get {
                return _list;
            }
        }

        public void Add(StrategyRealization realization)
        {
            _list.Add(realization);
        }
    }
}
