using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMexConnector;

namespace TestBitMexWebSocket
{
    class Program
    {
        static void Main(string[] args)
        {
            var connnector = new Connector();
            connnector.Connect();
        }
    }
}
