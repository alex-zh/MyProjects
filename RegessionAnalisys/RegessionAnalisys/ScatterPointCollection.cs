using System;
using System.Collections.Generic;
using System.Linq;
using Common.Scatterplot;

namespace RegessionAnalisys
{
    public class ScatterPointCollection : List<ScatterplotPoint>
    {
        private List<int> _groups;

        public ScatterPointCollection(IEnumerable<ScatterplotPoint> points)
        {
            AddRange(points);
        }

        public List<int> Groups
        {
            get
            {
                if (_groups == null)
                {
                    _groups = this.Select(x => Math.Abs(x.Group)).Distinct().ToList();
                }

                return _groups;
            }
        }

        public int GroupsCount
        {
            get { return this.Select(x => Math.Abs(x.Group)).Distinct().Count(); }
        }
    }
}