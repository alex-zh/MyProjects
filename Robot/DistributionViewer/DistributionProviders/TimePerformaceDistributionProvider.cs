using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Common;
using Common.Classes;
using Common.Classes.General;

namespace DistributionViewer.DistributionProviders
{
    public class TimePerformaceDistributionProvider
    {
        private List<Candle> _candles;

        public TimePerformaceDistributionProvider(List<Candle> candles)
        {
            _candles = candles;
        }

        public List<DistributionBin> Get()
        {            
            var values = new List<double>();

            for (int candleIndex = 0; candleIndex < _candles.Count - 6; candleIndex++)
            {
                var candle = _candles[candleIndex];
                var candleNext = _candles[candleIndex + 6];

                if (candle.Date.Hour == 18 && candle.Date.Minute == 30 && candle.Date.DayOfWeek == DayOfWeek.Wednesday)
                {
                    values.Add((candleNext.Close - candle.Open) * 100 * 100 / candle.Open);
                }                
            }

            var bins = new List<DistributionBin>();

            for(int i=0; i<values.Count; i++)
            {
                var bin = new DistributionBin();
                bin.MinValue = i;
                bin.MaxValue = i+1;
                bin.Count = (int)values[i];
                bins.Add(bin);
            }

            return bins;

        }
    }
}
