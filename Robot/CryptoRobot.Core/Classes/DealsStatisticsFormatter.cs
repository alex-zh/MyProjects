using System;
using System.Text;
using Common.Classes;
using Common.Classes.StrategyTester;

namespace Robot.Core.Classes
{
    internal class DealsStatisticsFormatter
    {
        private readonly TradeDataStatistics _result;
        private readonly DealsAnalyzer _dealsAnalizer;

        public DealsStatisticsFormatter(TradeDataStatistics result)
        {
            _result = result;
            _dealsAnalizer = new DealsAnalyzer(_result.Deals);
        }

        public string Format(TradeStatisticsInfoCategories category, TradeStatisticsInfoDealTypes deal)
        {
            if (category == TradeStatisticsInfoCategories.NumberOfProfitLossDeals)
            {
                switch (deal)
                {
                    case TradeStatisticsInfoDealTypes.All:
                        return FormatNumbersAndPercents(_result.NumberOfProfitDealsAll, _result.NumberOfLossDealsAll);
                    case TradeStatisticsInfoDealTypes.Long:
                        return FormatNumbersAndPercents(_result.NumberOfProfitDealsLong, _result.NumberOfLossDealsLong);
                    case TradeStatisticsInfoDealTypes.Short:
                        return FormatNumbersAndPercents(_result.NumberOfProfitDealsShort, _result.NumberOfLossDealsShort);
                    default:
                        throw new ArgumentOutOfRangeException("deal");
                }                        
            }
            else if (category == TradeStatisticsInfoCategories.NumberOfProfitLossDealsBigger025Sigma)
            {
                return FormatNumberOfProfitLossDealsBiggerSigma(deal, 0.25);
            }
            else if (category == TradeStatisticsInfoCategories.NumberOfProfitLossDealsBigger05Sigma)
            {
                return FormatNumberOfProfitLossDealsBiggerSigma(deal, 0.5);
            }
            else if (category == TradeStatisticsInfoCategories.NumberOfProfitLossDealsBigger075Sigma)
            {
                return FormatNumberOfProfitLossDealsBiggerSigma(deal, 0.75);
            }
            else if (category == TradeStatisticsInfoCategories.NumberOfProfitLossDealsBigger1Sigma)
            {
                return FormatNumberOfProfitLossDealsBiggerSigma(deal, 1);
            }
            else if (category == TradeStatisticsInfoCategories.NumberOfProfitLossDealsBigger15Sigma)
            {
                return FormatNumberOfProfitLossDealsBiggerSigma(deal, 1.5);
            }

            return "NA";
        }

        private string FormatNumberOfProfitLossDealsBiggerSigma(TradeStatisticsInfoDealTypes deal, double sigmaFactor)
        {
            int allNumber;
            int profitableNumber;
            switch (deal)
            {
                case TradeStatisticsInfoDealTypes.All:
                    allNumber = _dealsAnalizer.GetAllDealsNumber(sigmaFactor);
                    profitableNumber = _dealsAnalizer.GetAllProfitableDealsNumber(sigmaFactor);
                    break;
                case TradeStatisticsInfoDealTypes.Long:
                    allNumber = _dealsAnalizer.GetLongDealsNumber(sigmaFactor);
                    profitableNumber = _dealsAnalizer.GetLongProfitableDealsNumber(sigmaFactor);
                    break;
                case TradeStatisticsInfoDealTypes.Short:
                    allNumber = _dealsAnalizer.GetShortDealsNumber(sigmaFactor);
                    profitableNumber = _dealsAnalizer.GetShortProfitableDealsNumber(sigmaFactor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("deal");
            }

            return FormatNumbersAndPercents(profitableNumber, allNumber - profitableNumber);
        }
        
        public string FormatNumbers(int x, int y)
        {
            return String.Format("{0} / {1}", x, y);
        }

        /// <summary>
        /// 40 / 60 % (400/600)
        /// </summary>
        public string FormatNumbersAndPercents(int x, int y)
        {
            var total = x + y;

            var builder = new StringBuilder();

            var totalNumber = Math.Max(total, 1);

            builder.AppendFormat("   {0}/{1} % ( {2}/{3} )   ", Math.Round(((double)x * 100 / totalNumber)), Math.Round(((double)y * 100 / totalNumber)), x, y);

            return builder.ToString();
        }

        public string FormatPercents(int x, int y, int total)
        {
            var builder = new StringBuilder();

            var totalNumber = Math.Max(total, 1);

            builder.AppendFormat("   {0} / {1} %   ", Math.Round(((double)x * 100 / totalNumber)), Math.Round(((double)y * 100 / totalNumber)));

            return builder.ToString();
        }
    }
}