using Common.Classes.General;

namespace JointAssetsViewer
{
    public class BrSiPair
    {
        public BrSiPair(Candle brCandle, Candle siCandle)
        {
            BrCandle = brCandle;
            SiCandle = siCandle;
        }

        public Candle BrCandle { get; private set; }
        public Candle SiCandle { get; private set; }
    }
}