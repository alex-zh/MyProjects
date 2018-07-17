using System;

namespace QuikConnector.Common.Classes
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

            return Date.Equals(((Candle)obj).Date) && ((Candle)obj).Close == Close
                                                   && ((Candle)obj).Open == Open
                                                   && ((Candle)obj).Low == Low 
                                                   && ((Candle)obj).High == High
                                                   && ((Candle)obj).Price == Price;
        }

        public override int GetHashCode()
        {
            return new { Date, Open, High, Low, Close, Price }.GetHashCode();
        }
    }
}
