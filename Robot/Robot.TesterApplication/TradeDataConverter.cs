using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;
using Common.Classes.StrategyTester;
using Robot.TesterApplication.Classes;
using Robot.TesterApplication.Controls;
using Robot.TesterApplication.Enums;

namespace Robot.TesterApplication
{
    public class TradeDataConverter
    {
        public static IEnumerable<TradeSignal> ConvertToTradeSignals(List<Candle> candles, List<ITradeItem> rawData, bool useBinarySignals = false)
        {
            var result = new List<TradeSignal>();
            var currentSize = 0.0;

            foreach (var tradeItem in rawData)
            {
                var tradeSignal = new TradeSignal
                    {
                        Candle = candles.First(x => x.Date == tradeItem.DateTime),
                        Price = tradeItem.Price,  
                        TradeSignalType = GeTradeSignalType(tradeItem.Size, currentSize, useBinarySignals)
                    };

                currentSize += tradeItem.Size;

                result.Add(tradeSignal);
            }

            return result;
        }

        public static TradeSignalTypes GeTradeSignalType(double tradeSize, double currentSize, bool useBinarySignals)
        {
            if (useBinarySignals == false)
            {
                if (tradeSize > 0 && (currentSize + tradeSize) <= 0)
                {
                    return TradeSignalTypes.Close;
                }

                if (tradeSize < 0 && (currentSize + tradeSize) >= 0)
                {
                    return TradeSignalTypes.Close;
                }
            }

            if (tradeSize > 0)
            {
                return TradeSignalTypes.Buy;
            }
            else
            {
                return TradeSignalTypes.Sell;
            }
 
        }
    }
}