using System;
using Common.Classes;
using Common.Classes.General;

namespace Common
{
    public class DivergencyPoint
    {
        public int Index { get; set; }
        public double Atr { get; set; }
        public DateTime Date { get; set; }
        public Candle Candle { get; set; }
        public Direction Direction { get; set; }
        public double FmaPrice { get; set; }
        public double SmaPrice { get; set; }
    }
}