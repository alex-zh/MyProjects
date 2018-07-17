using System;

namespace BitMexConnector.Tables.Classes
{
    public class OrderBookEntry
    {
        public OrderBookEntry(long id, double size, double price)
        {
            Id = id;
            Size = size;
            Price = price;
        }

        public long Id { get; }
        public double Size { get; }
        public double Price { get; }

        public override int GetHashCode()
        {
            unchecked 
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + Size.GetHashCode();
                hash = hash * 23 + Price.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var objectToCompare = (OrderBookEntry)obj;
            return (Id == objectToCompare.Id) &&
                (Math.Abs(Size - objectToCompare.Size) < double.Epsilon) &&
                (Math.Abs(Price - objectToCompare.Price) < double.Epsilon);
        }
    }
}