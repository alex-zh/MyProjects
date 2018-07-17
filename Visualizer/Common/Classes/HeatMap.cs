using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Classes
{
    public class HeatMap<TX, TY> where TX : IComparable<TX>
                                 where TY : IComparable<TY>
    {
        private readonly List<TX> _xValues;
        private readonly List<TY> _yValues;
        private readonly Dictionary<Tuple<TX, TY>, HeatMapChartElement<TX, TY>> _dictionary = new Dictionary<Tuple<TX, TY>, HeatMapChartElement<TX, TY>>();

        private readonly List<HeatMapChartElement<TX, TY>> _list;

        public HeatMap(List<HeatMapChartElement<TX, TY>> list)
        {
            _list = list;
            _xValues = _list.Select(x => x.X).Distinct().OrderBy(x => x).ToList();
            _yValues = _list.Select(x => x.Y).Distinct().OrderBy(x => x).ToList();

            foreach (var item in list)
            {
                var key = new Tuple<TX, TY>(item.X, item.Y);

                if (_dictionary.ContainsKey(key) == false)
                    _dictionary.Add(key, item);
            }
        }

        public int MaxX => _xValues.Count - 1;

        public int MaxY => _yValues.Count - 1;

        public double MaxValue => _list.Max(x => x.Value);

        public double MinValue => _list.Min(x => x.Value);

        public TX GetX(int index)
        {
            return _xValues[index];
        }

        public TY GetY(int index)
        {
            return _yValues[index];
        }

        public double GetValue(int x, int y)
        {
            var key = new Tuple<TX, TY>(GetX(x), GetY(y));

            return _dictionary.ContainsKey(key) ? _dictionary[key].Value : 0;
        }


        public bool Any()
        {
            return _list.Any();
        }
    }
}
