using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Common;
using Common.Classes.General;

namespace Viewer
{
    public class TrendFeature
    {
        public TrendFeature()
        {
            Weight = 1;
            Values = new List<DateRangeValue>();
        }

        public int Weight { get; set; }

        public DateTime StartDate
        {
            get
            {
                if (Values.Any())
                {
                    return Values[0].StartDate;
                }

                return DateTime.MaxValue;
            }
        }

        public DateTime EndDate
        {
            get
            {
                if (Values.Any())
                {
                    return Values[Values.Count - 1].EndDate;
                }
               
                return DateTime.MaxValue;
            }
        }

        public List<DateRangeValue> Values { get; private set; }

        public List<double> GetValues()
        {
            return Values.OrderBy(x => x.StartDate).Select(x => x.Value).ToList();
        }
        
    }
}