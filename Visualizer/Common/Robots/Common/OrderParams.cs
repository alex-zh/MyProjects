namespace Common.Robots.Common
{
    public class OrderParams
    {        
        public double Size { get; set; }                
        public double Price { get; set; }
        public OrderSides Side { get; set; }
        
        public OrderParams()
        {
            
        }

        public OrderParams(OrderSides side, double size,  double price)
        {            
            Size = size;            
            Price = price;     
            Side = side;
        }

        public OrderParams Clone()
        {
            return new OrderParams
            {                
                Size = this.Size,                
                Price = this.Price,
                Side = this.Side,
            };
        }

        public override string ToString()
        {
            return $"{Size}, {Price}, {Side}";
        }
    }
}