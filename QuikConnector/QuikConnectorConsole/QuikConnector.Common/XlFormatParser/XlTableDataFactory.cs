using System;

namespace QuikConnector.Common.XlFormatParser
{
    public sealed class XlTableDataFactory<T>
    {
        public T Blank { get; set; }
        public T Skip { get; set; }
        public Func<int, int, T> Table { get; set; }
        public Func<double, T> Float { get; set; }
        public Func<string, T> String { get; set; }
        public Func<bool, T> Bool { get; set; }
        public Func<int, T> Error { get; set; }
        public Func<int, T> Int { get; set; }

        public IXlTableDataFactory<T> Bind() { return new Closure(this); }

        sealed class Closure : IXlTableDataFactory<T>
        {
            readonly T _blank;
            readonly T _skip;
            readonly Func<int, int, T> _table;
            readonly Func<double, T> _float;
            readonly Func<string, T> _string;
            readonly Func<bool, T> _bool;
            readonly Func<int, T> _error;
            readonly Func<int, T> _int;

            public Closure(XlTableDataFactory<T> factory)
            {
                _blank  = factory.Blank;
                _skip   = factory.Skip;
                _table  = factory.Table  ?? delegate { return default(T); };
                _float  = factory.Float  ?? delegate { return default(T); };
                _string = factory.String ?? delegate { return default(T); };
                _bool   = factory.Bool   ?? delegate { return default(T); };
                _error  = factory.Error  ?? delegate { return default(T); };
                _int    = factory.Int    ?? delegate { return default(T); };
            }

            T IXlTableDataFactory<T>.Blank                     { get { return _blank; }     }
            T IXlTableDataFactory<T>.Skip                      { get { return _skip;  }     }
            T IXlTableDataFactory<T>.Table(int rows, int cols) { return _table(rows, cols); }
            T IXlTableDataFactory<T>.Float(double value)       { return _float(value);      }
            T IXlTableDataFactory<T>.String(string value)      { return _string(value);     }
            T IXlTableDataFactory<T>.Bool(bool value)          { return _bool(value);       }
            T IXlTableDataFactory<T>.Error(int value)          { return _error(value);      }
            T IXlTableDataFactory<T>.Int(int value)            { return _int(value);        }
        }

    }
}