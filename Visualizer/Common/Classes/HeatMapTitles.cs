using System.Collections.Generic;

namespace Common.Classes
{
    public class HeatMapTitles
    {
        private Dictionary<int, string> _dictionary = new Dictionary<int, string>();

        public string this[int index]
        {
            get
            {
                if(_dictionary.ContainsKey(index))
                    return _dictionary[index];

                return "NA";
            }
            set
            {
                _dictionary[index] = value;
            }
        }

        public int Count { get { return _dictionary.Keys.Count; } }
    }
}