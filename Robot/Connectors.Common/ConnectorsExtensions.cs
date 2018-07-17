using System;
using System.Collections.Generic;
using Common.Classes.Robot;

namespace Connector.Common
{
    public static class ConnectorsExtensions
    {
        public static  string ToText(this Connectors connector)
        {
            switch (connector)
            {
                case Connectors.BitFinex:
                    return "Bitfinex";
                case Connectors.BitMex:
                    return "BitMex";                                
            }
            return connector.ToString();
        }

        public static IEnumerable<KeyValuePair<int, string>> ToList()
        {
            return Enum<Connectors>.ToList();                    
        }        
    }
}