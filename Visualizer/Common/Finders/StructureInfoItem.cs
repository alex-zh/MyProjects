namespace Common.Finders
{
    public class StructureInfoItem
    {        
        public double Atr { get; set; }
        public int Period { get; set; }
        public double Change { get; set; }

        public StructureInfoItem Clone()
        {
            return new StructureInfoItem()
            {
                Atr = this.Atr,
                Change = this.Change,
                Period = this.Period
            };
        }
    }
}