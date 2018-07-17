using System.Collections.Generic;
using Common.Classes.General;

namespace Robot.Strategies.Agents
{
    public class CandlesSimpleAggregator
    {
        private readonly int _aggregationFactor;

        /// <summary>
        /// will create candles squeezing passing candles in aggregationFactor times
        /// </summary>
        public CandlesSimpleAggregator(int aggregationFactor = 12)
        {
            _aggregationFactor = aggregationFactor;
        }

        public List<Candle> Aggregate(List<Candle> candles)
        {
            var index = 0;
            var result = new List<Candle>();
            int rangeEndIndex = 0;
            double rangeMaxValue = 0.0, rangeMinValue = 0.0;

            for (int i = candles.Count - 1; i >= 0; i--)
            {

                if (rangeEndIndex == 0)
                {
                    rangeEndIndex = i;
                    rangeMaxValue = candles[i].High;
                    rangeMinValue = candles[i].Low;
                }
                else
                {
                    var rangeLength = rangeEndIndex - i + 1;

                    if (rangeLength < _aggregationFactor)
                    {
                        if (candles[i].High > rangeMaxValue)
                            rangeMaxValue = candles[i].High;
                        if (candles[i].Low < rangeMinValue)
                            rangeMinValue = candles[i].Low;
                    }
                    else if (rangeLength == _aggregationFactor)
                    {
                        result.Insert(0, new Candle
                        {
                            Date = candles[i].Date,
                            Open = candles[i].Open,
                            Low = rangeMinValue,
                            High = rangeMaxValue,
                            Close = candles[rangeEndIndex].Close
                        });

                        rangeEndIndex = 0;
                        rangeMaxValue = 0.0;
                        rangeMinValue = 0.0;

                    }
                }
            }

            return result;
        }
    }
}