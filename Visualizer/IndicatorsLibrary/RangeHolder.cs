using Common.Classes;
using Common.Classes.General;

namespace Core
{
    class RangeChecker
    {
        private Candle _currentCandle;
        private double _atr;
        private double _middle;

        public RangeChecker(Candle currentCandle, double atr)
        {
            _currentCandle = currentCandle;
            _atr = atr;
            _middle = GetMiddle(currentCandle);
        }

        public bool IsInsideBaseRange(Candle candle)
        {
            var middle = GetMiddle(candle);

            if (middle <= _middle + 0.3*_atr && middle >= _middle - 0.5 * _atr)
            {
                return true;
            }

            return false;
        }

        public bool IsInsideTransitionRange(Candle candle)
        {
            var middle = GetMiddle(candle);

            if (middle >= _middle + 0.3*_atr )//&& middle <= _middle + 2 * _atr)
            {
                return true;
            }

            return false;
        }

        private double GetMiddle(Candle candle)
        {
            return (candle.Open + candle.Close) / 2;
        }
    }
}
