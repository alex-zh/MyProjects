using System.Collections.Generic;
using Common.Classes.General;

namespace Common.Classes.Builders
{
    public class TrendPart
    {
        public TrendPart()
        {
            Parts = new List<TrendPart>();       
        }

        public TrendTypes TrendType { get; set; }

        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        
        public List<TrendPart> Parts { get; set; }        
    }

}
