using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancialIdeasTester.Classes;

namespace FinancialIdeasTester
{
    public class LogScaleDetrendedChangesValuator
    {
        //1. Find average candle trend change
        public List<CandleChange> Valuate(List<Candle> candles)
        {
            var result = new List<CandleChange>();
            double totalLogChange = 0.0;

            for(var i=0; i< candles.Count; i++)
            {
                var candle = candles[i];
                var previousCandle = (i > 0) ? candles[i - 1] : null;

                var logChange = Math.Log(candle.Close) - Math.Log(candle.Open);
                var logChangeFromPreviousCandle = (previousCandle != null) ? Math.Log(candle.Open) - Math.Log(candle.Close) : logChange;

                result.Add(new CandleChange() { Date = candle.Date, Change = logChange, ChangeFromPreviousClose = logChangeFromPreviousCandle });

                totalLogChange += logChange;
            }

            var averageLogChange = totalLogChange / result.Count;

            foreach (var candleChange in result)
            {
                candleChange.Change -= averageLogChange;
                candleChange.ChangeFromPreviousClose -= averageLogChange;
            }

            return result;
        }
    }
}
