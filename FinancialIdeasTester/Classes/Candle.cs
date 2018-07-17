using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialIdeasTester.Classes
{
    public class Candle
    {
        public double Close { get; set; }
        public double Open { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Price { get; set; }
        public DateTime Date { get; set; }
    }
}
