using System;

namespace FinancialIdeasTester.Classes
{
    public class DetrendedValue
    {
        public double Value { get; set; }        
        public DateTime Date { get; set; }
    }

    public class CandleChange
    {
        public double Change { get; set; }
        public double ChangeFromPreviousClose { get; set; }
        public DateTime Date { get; set; }
    }
}