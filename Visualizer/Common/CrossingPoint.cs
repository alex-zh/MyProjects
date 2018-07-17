using System;

namespace Common
{
    public class CrossingPoint
    {        
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public Direction Direction { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CrossingPoint)obj);
        }

        protected bool Equals(CrossingPoint other)
        {
            return Index == other.Index && Date.Equals(other.Date) && Direction == other.Direction;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Index;
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Direction;
                return hashCode;
            }
        }

        
    }
}