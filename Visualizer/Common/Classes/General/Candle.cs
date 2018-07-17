using System;

namespace Common.Classes.General
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

        protected bool Equals(Candle other)
        {
            return Close.Equals(other.Close) && Date.Equals(other.Date);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Candle)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Close.GetHashCode() * 397) ^ Date.GetHashCode();
            }
        }

        public Candle Clone()
        {
            return new Candle()
            {
                Close = this.Close,
                Open = this.Open,
                Low = this.Low,
                High = this.High,
                Price = this.Price,
                Date = this.Date,
                Volume = this.Volume
            };
        }
    }
}
