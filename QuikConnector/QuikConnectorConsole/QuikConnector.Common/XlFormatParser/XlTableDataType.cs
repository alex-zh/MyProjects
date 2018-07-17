using System;

namespace QuikConnector.Common.XlFormatParser
{
    [Serializable]
    internal enum XlTableDataType
    {
        Table  = 16,
        Float  = 1,
        String = 2,
        Bool   = 3,
        Error  = 4,
        Blank  = 5,
        Int    = 6,
        Skip   = 7,
    }
}