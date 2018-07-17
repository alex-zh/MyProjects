namespace Robot.Trans2Quik.Entities.Transaction.Order
{
    public class OrderTradeParams
    {
        public Security Security { get; set; }
        public Direction Direction { get; set; }
        public int Size { get; set; }
        public decimal Price { get; set; }

        public OrderTradeParams(){}
        
        public OrderTradeParams(Security security, Direction direction, int size, decimal price = decimal.Zero, decimal minimalPriceStep = 1)
        {
            Security = security;
            Direction = direction;
            Size = size;
            Price = price;
            MinimalPriceStep= minimalPriceStep;
        }

        public decimal MinimalPriceStep { get; set; }

        public bool IsLimited
        {
            get
            {
                return Price != decimal.Zero;
            }
        }

        public OrderTradeParams Clone()
        {
            return new OrderTradeParams()
            {
                Security = this.Security,
                Direction = this.Direction,
                Size = this.Size,
                Price = this.Price,
                MinimalPriceStep = this.MinimalPriceStep
            };
        }
    }
}
