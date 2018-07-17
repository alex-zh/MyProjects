using System;
using System.Collections.Generic;
using System.Linq;
using FinancialIdeasTester.Classes;

namespace FinancialIdeasTester.Rules.BrentToUsdRubRatio
{
    public class BrentToUsdRubRatioRule
    {
        private readonly int _averagePeriod = 7;//days
        private readonly int _ratioChangePeriod = 2;//days
        private const int UsdRubNextChangePeriod = 1;//days

        private readonly List<Candle> _brentCandles = new List<Candle>();
        private readonly List<Candle> _usdrubCandles = new List<Candle>();

        public BrentToUsdRubRatioRule(DataHolder brentDataHolder, DataHolder usdrubDataHolder) : 
                                      this(brentDataHolder, usdrubDataHolder, 7, 2){}

        public BrentToUsdRubRatioRule(DataHolder brentDataHolder, 
                                      DataHolder usdrubDataHolder, 
                                      int averagePeriod,
                                      int ratioChangePeriod)
        {
            if (brentDataHolder.Candles.Count != usdrubDataHolder.Candles.Count)
            {
                //Logger.Warn("brentDataHolder and  usdrubCandles have different lenght!");                
            }

            _brentCandles = brentDataHolder.Candles.Intersect(usdrubDataHolder.Candles, new CandleByDateComparer()).ToList();
            _usdrubCandles = usdrubDataHolder.Candles.Intersect(brentDataHolder.Candles, new CandleByDateComparer()).ToList();

            _brentCandles.Sort((a, b) => DateTime.Compare(a.Date, b.Date));
            _usdrubCandles.Sort((a, b) => DateTime.Compare(a.Date, b.Date));

          _averagePeriod = averagePeriod;
            _ratioChangePeriod = ratioChangePeriod;
        }

        public List<BrentToUsdRubRatioRuleResult> Calculate()
        {
            var result = new List<BrentToUsdRubRatioRuleResult>();

            for (int i = _averagePeriod; i < _usdrubCandles.Count - _ratioChangePeriod - 40; i++)
            {
                double averageRatio = CalculateAverageRatio(i);

                double ratioChange = CalculateBrentRatioChange(i, i + 4);

                if (ratioChange * 100 < 3)
                    continue;
                
                //if (Math.Abs((_brentCandles[i + 17].Close - brentAveragePrice) * 100 / brentAveragePrice )< 1)
                //    continue;


                double nextAverageRatio = CalculateAverageRatio(i+2);

                double nextRationChange = CalculateUsdRatioChange( i + 4, i+6);

                result.Add(new BrentToUsdRubRatioRuleResult
                {
                    DateTime = _usdrubCandles[i].Date,
                    RatioChange = ratioChange,                     
                    NextRatioChange = nextRationChange
                });
            }
            return result;
        }

        private double CalculateUsdRubChange(int startIndex, int endIndex)
        {
            var startValue = _usdrubCandles[startIndex].Close;
            var endValue = _usdrubCandles[endIndex].Close;

            return (endValue - startValue) / startValue;
        }

        private double CalculateBrentRatioChange(int startIndex, int endIndex)
        {
            var startValue = _brentCandles[startIndex].Close;
            var endValue = _brentCandles[endIndex].Close;

            return (endValue - startValue)/startValue;
            //var currentRatio = _brentCandles[endIndex].Close / _usdrubCandles[endIndex].Close;

            //return currentRatio - averageRatio;
        }

        private double CalculateUsdRatioChange(int startIndex, int endIndex)
        {
            var startValue = _usdrubCandles[startIndex].Close;
            var endValue = _usdrubCandles[endIndex].Close;

            return (endValue - startValue) / startValue;
            //var currentRatio = _brentCandles[endIndex].Close / _usdrubCandles[endIndex].Close;

            //return currentRatio - averageRatio;
        }

        private double CalculateAverageRatio(int currentIndex)
        {
            double total = 0.0;

            for (int i = currentIndex - _averagePeriod; i < currentIndex; i++)
            {
                total += _brentCandles[i].Close/_usdrubCandles[i].Close;
            }

            return total / _averagePeriod;
        }
    }
}
