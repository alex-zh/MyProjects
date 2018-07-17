using System;

namespace Scatterplot.Core.Classes
{
    public class Candle
    {             
        public double Close { get; set; }
        public double Open { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Price { get; set; }
        public DateTime Date { get; set; }
        public double Volume { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Date.Equals(((Candle)obj).Date);
        }

        public override int GetHashCode()
        {
            return Date.GetHashCode();
        }
    }
}
