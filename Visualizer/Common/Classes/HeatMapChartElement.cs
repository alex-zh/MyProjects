namespace Common.Classes
{
    public class HeatMapChartElement<TX, TY>
    {
        public TX X { get; set; }
        public TY Y { get; set; }
        public double Value { get; set; }

        public string Text { get; set; }
    }
}