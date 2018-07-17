using System;

namespace Common.Scatterplot
{
    public class ScatterplotPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public DateTime? Date { get; set; }
        public int Group { get; set; }
        
        public int SubGroup { get; set; }        
    }
}
