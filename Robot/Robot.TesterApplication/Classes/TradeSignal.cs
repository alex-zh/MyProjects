using Common.Classes.General;
using Robot.TesterApplication.Enums;

namespace Robot.TesterApplication.Classes
{
    public class TradeSignal
    {
        public Candle Candle { get; set; }
        public double Price { get; set; }
        public TradeSignalTypes TradeSignalType { get; set; }

        /// <summary>
        /// Парный сингнал, если текущий это открытие позиции, тогда PairSignal это закрытие
        /// </summary>
        public TradeSignal PairSignal { get; set; }
    }
}