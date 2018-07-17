using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAnna
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var list = new MyList();
            list.Add(new Node() {Data = "1"});
            list.Add(new Node() {Data = "2"});
            list.Add(new Node() {Data = "3" });

            list.Print();
        }
    }
}
    