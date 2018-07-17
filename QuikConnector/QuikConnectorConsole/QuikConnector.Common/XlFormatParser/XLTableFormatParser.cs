using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace QuikConnector.Common.XlFormatParser
{
    static class XlTableFormatParser
    {
        public static readonly IXlTableDataFactory<object> DefaultDataFactory = new XlTableDataFactory<object>
        {
            Blank   = null,
            Skip    = Missing.Value,
            Table   = (rows, cols) => new[] { rows, cols },
            Float   = v => v,
            String  = v => v,
            Bool    = v => v,
            Error   = v => new ErrorWrapper(v),
            Int     = v => v,
        }
            .Bind();

        public static T Read<T>(byte[] data, Func<int, int, object[], T> resultor)
        {
            using (var ms = new MemoryStream(data))
                return Read(ms, resultor);
        }

        public static T Read<T>(Stream stream, Func<int, int, object[], T> resultor)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (resultor == null) throw new ArgumentNullException("resultor");

            using (var e = Read(stream, DefaultDataFactory))
            {
                if (!e.MoveNext()) throw new FormatException();
                var size = (int[]) e.Current;
                var rows = size[0];
                var cols = size[1];
                var cells = new object[rows * cols];
                for (var i = 0; e.MoveNext(); i++)
                    cells[i] = e.Current;
                return resultor(rows, cols, cells);
            }
        }

        public static IEnumerable<object> Read(byte[] data)
        {
            return Read(data, DefaultDataFactory);
        }

        public static IEnumerable<T> Read<T>(byte[] data, IXlTableDataFactory<T> factory)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (factory == null) throw new ArgumentNullException("factory");
            return ReadImpl(data, factory);
        }
    
        static IEnumerable<T> ReadImpl<T>(byte[] data, IXlTableDataFactory<T> factory)
        {
            using (var ms = new MemoryStream(data))
            using (var e = Read(ms, factory))
                while (e.MoveNext())
                    yield return e.Current;
        }

        public static IEnumerator<object> Read(Stream stream)
        {
            return Read(stream, DefaultDataFactory);
        }

        public static IEnumerator<T> Read<T>(Stream stream, IXlTableDataFactory<T> factory)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (factory == null) throw new ArgumentNullException("factory");
            return ReadImpl(stream, factory);
        }
    
        static IEnumerator<T> ReadImpl<T>(Stream stream, IXlTableDataFactory<T> factory)
        {
            using (var reader = new BinaryReader(stream))
            {
                if (XlTableDataType.Table != (XlTableDataType) reader.ReadUInt16()) 
                    throw new FormatException();
                var size = reader.ReadUInt16();
                if (size != 4) 
                    throw new FormatException();
                var rows = reader.ReadUInt16();
                var cols = reader.ReadUInt16();
                yield return factory.Table(rows, cols);
                var cells = rows * cols;
                while (cells > 0)
                {
                    var type = (XlTableDataType) reader.ReadUInt16();
                    size = reader.ReadUInt16();
                    if (type == XlTableDataType.String)
                    {
                        while (size > 0)
                        {
                            var str = Encoding.Default.GetString(reader.ReadBytes(reader.ReadByte()));
                            yield return factory.String(str);
                            cells--;
                            size -= (ushort) (1 + checked((byte) str.Length));
                        }
                    }
                    else
                    {
                        int count;
                        Func<BinaryReader, IXlTableDataFactory<T>, T> rf;
                        switch (type)
                        {
                            case XlTableDataType.Float:
                                if (size % 8 != 0) throw new FormatException();
                                count = size / 8;
                                rf = (r, f) => f.Float(r.ReadDouble());
                                break;
                            case XlTableDataType.Skip:
                                if (size != 2) throw new FormatException();
                                count = reader.ReadUInt16();
                                rf = (r, f) => f.Skip;
                                break;
                            case XlTableDataType.Blank:
                                if (size != 2) throw new FormatException();
                                count = reader.ReadUInt16();
                                rf = (r, f) => f.Blank;
                                break;
                            case XlTableDataType.Error:
                                if (size % 2 != 0) throw new FormatException();
                                count = size / 2;
                                rf = (r, f) => f.Error(r.ReadUInt16());
                                break;
                            case XlTableDataType.Bool:
                                if (size % 2 != 0) throw new FormatException();
                                count = size / 2;
                                rf = (r, f) => f.Bool(r.ReadUInt16() != 0);
                                break;
                            case XlTableDataType.Int:
                                if (size % 2 != 0) throw new FormatException();
                                count = size / 2;
                                rf = (r, f) => f.Int(r.ReadUInt16());
                                break;
                            default: throw new FormatException();
                        }

                        for (var j = 0; j < count; j++)
                            yield return rf(reader, factory);
                    
                        cells -= count;
                    }
                }
            }
        }
    }
}