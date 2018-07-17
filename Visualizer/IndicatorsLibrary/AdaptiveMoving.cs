using Common.Classes;
using Common.Classes.General;

namespace Core
{
    /// <summary>
    /// Представляет из себя адаптивную среднею
    /// </summary>
    public class AdaptiveMoving
    {
        private int _period;
        private Candle[] _candles;        

        public AdaptiveMoving(Candle[] candles, CustomVolatilityCalculator customVolatilityCalculator, int period)
        {
            _period = period;
            _candles = candles;            
        }
    }
}
