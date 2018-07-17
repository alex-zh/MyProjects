using System;
using System.Collections.Generic;

namespace FortsPositionsViewer
{
    public class OpenInterestData
    {
        public Dictionary<DateTime, OpenInterestDataItem> Legals { get; set; }     
        public Dictionary<DateTime, OpenInterestDataItem> Individuals { get; set; }

        public OpenInterestData()
        {
            Legals = new Dictionary<DateTime, OpenInterestDataItem>();
            Individuals = new Dictionary<DateTime, OpenInterestDataItem>();
        }
    }
}