using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Classes;
using Common.Classes.General;

namespace DistributionViewer.HeatMapProviders
{
    /// <summary>
    /// For specific number of days builds for each hour shows heat value as % of change
    /// </summary>
    public class TimeToDayHeatMapProvider
    {
        private const int _numberOfDays = 61;
        private List<Candle> _candles;

        public TimeToDayHeatMapProvider(List<Candle> candles)
        {
            if (candles.Count < 2)
                throw new ArgumentException("Number of candles must be meaningfull");

            if (( candles.Skip(1).First().Date - candles.First().Date).TotalHours != 1)
                throw new ArgumentException("Candles must be 1 hour candles");

            _candles = candles;
        }

        public HeatMap<DateTime, int> Get()
        {
            var heatMapList = new List<HeatMapChartElement<DateTime, int>>();

            var groupedByDay = _candles.OrderByDescending(x => x.Date.Date)
                                       .Take(_numberOfDays * 12)
                                       .GroupBy(x => x.Date.Date).OrderBy(x => x.Key);

            foreach (var dayGroup in groupedByDay)
            {                                
                var candles = dayGroup.OrderByDescending(x => x.Date.Hour).ToList();

                foreach (var candle in candles)
                {

                    var element = new HeatMapChartElement<DateTime, int>
                    {
                        X = dayGroup.Key,
                        Y = candle.Date.Hour,
                        Value = (candle.Close - candle.Open) * 100 / candle.Open,
                        Text = ((candle.Close - candle.Open) * 100 / candle.Open).ToString("00")
                    };

                    heatMapList.Add(element);

                }
            }
            
            var heatMap = new HeatMap<DateTime, int>(heatMapList);

            return heatMap;
        }
    }
}
