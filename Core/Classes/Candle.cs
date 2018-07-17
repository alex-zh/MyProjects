using System;

namespace Core.Classes
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

    public class CandleChange
    {
        public double Change { get; set; }
        public double ChangeFromPreviousClose { get; set; }
        public DateTime Date { get; set; }
    }
}
