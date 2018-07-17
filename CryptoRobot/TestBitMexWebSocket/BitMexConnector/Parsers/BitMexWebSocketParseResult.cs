using System.Collections.Generic;

namespace BitMexConnector.Parsers
{
    public class BitMexWebSocketParseResult<T>
    {
        public BitMexWebSocketActionNames ActionName
        {
            get; set;            
        }

        public List<T> Data { get; set; }
    }
}