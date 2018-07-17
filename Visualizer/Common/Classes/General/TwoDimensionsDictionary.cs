using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Classes.General
{
    public class TwoDimensionsDictionary<K1, K2, V>
    {
        private readonly Func<V> _notFoundAction;
        private readonly Dictionary<K1, Dictionary<K2, V>> _dictionary = new Dictionary<K1, Dictionary<K2, V>>();

        public TwoDimensionsDictionary(Func<V> notFoundAction)
        {                        
            _notFoundAction = notFoundAction;
        }

        public List<K1> RowKeys
        {
            get { return _dictionary.Keys.ToList(); }
        }

        public List<K2> ColumnKeys
        {
            get {
                return _dictionary.Select(x => x.Value).SelectMany(x => x.Keys).Distinct().ToList();
            }
        }

        public int RowsCount
        {
            get { return RowKeys.Count; }
        }

        public int ColumnsCount
        {
            get { return ColumnKeys.Count; }
        }

        public V this[K1 key1, K2 key2]
        {
            get
            {
                if (_dictionary.ContainsKey(key1) && _dictionary[key1].ContainsKey(key2))
                {
                    return _dictionary[key1][key2];    
                }

                return _notFoundAction();
            }

            set
            {
                if (!_dictionary.ContainsKey(key1))
                {
                    _dictionary[key1] = new Dictionary<K2, V>();
                }
                _dictionary[key1][key2] = value;
            }
        }        
    }
}
