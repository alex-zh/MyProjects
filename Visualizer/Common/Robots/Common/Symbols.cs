using System.Collections.Generic;

namespace Common.Robots.Common
{
    public class BitMex
    {
        public string XBTUSD => "XBTUSD";
        public string XRPU18 => "XRPU18";

        public IEnumerable<string> GetAll()
        {
            yield return XBTUSD;
            yield return XRPU18;
        }
    }

    public class Symbols
    {
        static Symbols()
        {
            BitMex = new BitMex();    
        }

        public static BitMex BitMex { get; }
    }
}