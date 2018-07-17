using System;
using Common;
using Scatterplot.Core.Classes;

namespace RegessionAnalisys.DataPreparers
{
    public class BrentUsdDataPoint
    {
        public Candle BrentCandle{ get; set; }
        public Candle UsdCandle { get; set; }
        public bool IsGrouped { get; set; }
        public DateTime Date { get; set; }
    }
}