namespace Trans2Quik.Core.Entities.Transaction.Order
{
    public class OrderTradeParams
    {
        public Security Security { get; private set; }
        public Direction Direction { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }

        public bool IsLimited
        {
            get
            {
                return Price != decimal.Zero;
            }
        }

        public bool SupportDecimals { get; set; }

        public OrderTradeParams(Security security, Direction direction, int quantity, decimal price = decimal.Zero, bool supportDecimals = true)
        {
            Security = security;
            Direction = direction;
            Quantity = quantity;
            Price = price;
            SupportDecimals = supportDecimals;
        }
        public OrderTradeParams(string classCode, string secCode, Direction direction, int quantity, decimal price = decimal.Zero, bool supportDecimals = true)
            : this(new Security(classCode, secCode), direction, quantity, price, supportDecimals)
        {
        }
    }
}
