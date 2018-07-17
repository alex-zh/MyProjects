using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHex
{
    class Program
    {
        static void Main(string[] args)
        {
            ulong IV0 = 0x6A09E667F3BCC908UL;
            var res = ConvertToBinary(IV0);
            Console.WriteLine(res);
        }

        public static string ConvertToBinary(ulong value)
        {
            if (value == 0) return "0";
            System.Text.StringBuilder b = new System.Text.StringBuilder();
            while (value != 0)
            {
                b.Insert(0, ((value & 1) == 1) ? '1' : '0');
                value>>= 1;
            }
            return b.ToString();
        }
    }


}
