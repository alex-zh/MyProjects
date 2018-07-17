namespace Common.Classes.Builders
{
  
    public class TrendLine
    {
        public TrendPoint Start { get; set; }
        public TrendPoint End { get; set; }

        /// <summary>
        /// Number of candles composing the trend
        /// </summary>
        public int Length { get; set; }
        public TrendTypes TrendType { get; set; }
        
    }
}