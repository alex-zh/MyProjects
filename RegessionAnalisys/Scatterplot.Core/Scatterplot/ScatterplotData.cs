using System.Collections.Generic;

namespace Common.Scatterplot
{
    public class ScatterplotData
    {
        public ScatterplotData()
        {
            Points = new List<ScatterplotPoint>();
        }

        public string LableX { get; set; }
        public string LableY { get; set; }

        public List<ScatterplotPoint> Points
        {
            get;
            private set;            
        }
    }
}
