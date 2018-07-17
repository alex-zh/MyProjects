using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Common.Classes;
using Common.Classes.General;
using Common.Classes.StrategyTester;
using Brushes = System.Windows.Media.Brushes;
using Deals = Common.Classes.TradeStatisticsInfoDealTypes;
using Categories = Common.Classes.TradeStatisticsInfoCategories;

namespace Robot.Core.Classes
{
    public class TradeDataStatisticsToGridConverter
    {
        private readonly TradeDataStatistics _result;
        private readonly int _maxPositionSize;
        private Dictionary<Categories, Func<double, string>> _formatValuesDictionary;
        private Dictionary<Categories, Action<double, TextBlock>> _formatElementsDictionary;
        private TwoDimensionsDictionary<TradeStatisticsInfoCategories, TradeStatisticsInfoDealTypes, string> _testerDealsNumbersDictionary;
        private TwoDimensionsDictionary<TradeStatisticsInfoCategories, TradeStatisticsInfoDealTypes, double> _testerProfitLossDictionary;
        private TwoDimensionsDictionary<TradeStatisticsInfoCategories, TradeStatisticsInfoDealTypes, double> _profitLossDictionary;

        public TradeDataStatisticsToGridConverter(TradeDataStatistics result, int maxPositionSize)
        {
            _result = result;
            _maxPositionSize = maxPositionSize;

            SetupFormats();
            SetupProfitLossGrid();
            SetupTesterProfitLossGrid();
            SetupDealsNumbersGrid();
        }

        public TwoDimensionsDictionary<Categories, Deals, double> ProfitLossDictionary
        {
            get { return _profitLossDictionary; }
        }

        public TwoDimensionsDictionary<Categories, Deals, double> TesterProfitLossDictionary
        {
            get { return _testerProfitLossDictionary; }
        }

        public TwoDimensionsDictionary<Categories, Deals, string> TesterDealsNumbersDictionary
        {
            get { return _testerDealsNumbersDictionary; }
        }

        private void SetupFormats()
        {
            _formatValuesDictionary = new Dictionary<Categories, Func<double, string>>();

            _formatElementsDictionary = new Dictionary<Categories, Action<double, TextBlock>>();

            _formatElementsDictionary.Add(Categories.PercentProfit, (value, textBlock) =>
            {
                textBlock.FontWeight = FontWeights.Bold;

                if (value > 0) textBlock.Foreground = Brushes.Green;
                else if (value < 0) textBlock.Foreground = Brushes.Red;
            });

            _formatValuesDictionary.Add(Categories.MoneyProfit, x => x.ToString("N2"));
            _formatValuesDictionary.Add(Categories.MoneyDrawdown, x => x.ToString("N2"));
            _formatValuesDictionary.Add(Categories.PercentProfit, x => x.ToString("N2") + " %");
            _formatValuesDictionary.Add(Categories.PercentDrawdown, x => x.ToString("N2") + " %");
            _formatValuesDictionary.Add(Categories.PercentAggregatedProfit, x => x.ToString("N2") + " %");
        }

        private void SetupDealsNumbersGrid()
        {
            var dealsFormatter = new DealsStatisticsFormatter(_result, _maxPositionSize);

            _testerDealsNumbersDictionary = new TwoDimensionsDictionary<Categories, Deals, string>(() => "NA");

            _testerDealsNumbersDictionary[Categories.NumberOfDeals, Deals.All] = _result.NumberOfDealsAll.ToString();
            _testerDealsNumbersDictionary[Categories.NumberOfDeals, Deals.Long] = _result.NumberOfDealsLong.ToString();
            _testerDealsNumbersDictionary[Categories.NumberOfDeals, Deals.Short] = _result.NumberOfDealsShort.ToString();

            _testerDealsNumbersDictionary[Categories.NumberOfTradedContracts, Deals.All] = _result.NumberOfTradedContractsAll.ToString();
            _testerDealsNumbersDictionary[Categories.NumberOfTradedContracts, Deals.Long] = _result.NumberOfTradedContractsLong.ToString();
            _testerDealsNumbersDictionary[Categories.NumberOfTradedContracts, Deals.Short] = _result.NumberOfTradedContractsShort.ToString();
            
            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDeals, Deals.All] = dealsFormatter.Format(Categories.NumberOfProfitLossDeals, Deals.All);
            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDeals, Deals.Long] = dealsFormatter.Format(Categories.NumberOfProfitLossDeals, Deals.Long);
            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDeals, Deals.Short] = dealsFormatter.Format(Categories.NumberOfProfitLossDeals, Deals.Short);

            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDealsBigger025Sigma, Deals.All] = dealsFormatter.Format(Categories.NumberOfProfitLossDealsBigger025Sigma, Deals.All);
            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDealsBigger025Sigma, Deals.Long] = dealsFormatter.Format(Categories.NumberOfProfitLossDealsBigger025Sigma, Deals.Long);
            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDealsBigger025Sigma, Deals.Short] = dealsFormatter.Format(Categories.NumberOfProfitLossDealsBigger025Sigma, Deals.Short);

            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDealsBigger05Sigma, Deals.All] = dealsFormatter.Format(Categories.NumberOfProfitLossDealsBigger05Sigma, Deals.All);
            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDealsBigger05Sigma, Deals.Long] = dealsFormatter.Format(Categories.NumberOfProfitLossDealsBigger05Sigma, Deals.Long);
            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDealsBigger05Sigma, Deals.Short] = dealsFormatter.Format(Categories.NumberOfProfitLossDealsBigger05Sigma, Deals.Short);

            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDealsBigger075Sigma, Deals.All] = dealsFormatter.Format(Categories.NumberOfProfitLossDealsBigger075Sigma, Deals.All);
            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDealsBigger075Sigma, Deals.Long] = dealsFormatter.Format(Categories.NumberOfProfitLossDealsBigger075Sigma, Deals.Long);
            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDealsBigger075Sigma, Deals.Short] = dealsFormatter.Format(Categories.NumberOfProfitLossDealsBigger075Sigma, Deals.Short);

            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDealsBigger1Sigma, Deals.All] = dealsFormatter.Format(Categories.NumberOfProfitLossDealsBigger1Sigma, Deals.All);
            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDealsBigger1Sigma, Deals.Long] = dealsFormatter.Format(Categories.NumberOfProfitLossDealsBigger1Sigma, Deals.Long);
            _testerDealsNumbersDictionary[Categories.NumberOfProfitLossDealsBigger1Sigma, Deals.Short] = dealsFormatter.Format(Categories.NumberOfProfitLossDealsBigger1Sigma, Deals.Short);
        }

        private void SetupProfitLossGrid()
        {
            _profitLossDictionary = new TwoDimensionsDictionary<Categories, Deals, double>(() => 0.0);

            _profitLossDictionary[Categories.PercentProfit, Deals.All] = _result.PercentProfitAll;
            _profitLossDictionary[Categories.PercentProfit, Deals.Long] = _result.PercentProfitLong;
            _profitLossDictionary[Categories.PercentProfit, Deals.Short] = _result.PercentProfitShort;
            _profitLossDictionary[Categories.PercentProfit, Deals.Market] = _result.PercentProfitMarket;
            
            _profitLossDictionary[Categories.MoneyProfit, Deals.All] = _result.MoneyProfitAll;
            _profitLossDictionary[Categories.MoneyProfit, Deals.Long] = _result.MoneyProfitLong;
            _profitLossDictionary[Categories.MoneyProfit, Deals.Short] = _result.MoneyProfitShort;
            _profitLossDictionary[Categories.MoneyProfit, Deals.Market] = _result.MoneyProfitMarket;
            
            _profitLossDictionary[Categories.PercentAggregatedProfit, Deals.All] = _result.PercentAggregatedProfitAll;
            _profitLossDictionary[Categories.PercentAggregatedProfit, Deals.Long] = _result.PercentAggregatedProfitLong;
            _profitLossDictionary[Categories.PercentAggregatedProfit, Deals.Short] = _result.PercentAggregatedProfitShort;            
        }

        private void SetupTesterProfitLossGrid()
        {
            _testerProfitLossDictionary = new TwoDimensionsDictionary<Categories, Deals, double>(() => 0.0);

            _testerProfitLossDictionary[Categories.PercentProfit, Deals.All] = _result.PercentProfitAll;
            _testerProfitLossDictionary[Categories.PercentProfit, Deals.Long] = _result.PercentProfitLong;
            _testerProfitLossDictionary[Categories.PercentProfit, Deals.Short] = _result.PercentProfitShort;
            _testerProfitLossDictionary[Categories.PercentProfit, Deals.Market] = _result.PercentProfitMarket;

            _testerProfitLossDictionary[Categories.MoneyProfit, Deals.All] = _result.MoneyProfitAll;
            _testerProfitLossDictionary[Categories.MoneyProfit, Deals.Long] = _result.MoneyProfitLong;
            _testerProfitLossDictionary[Categories.MoneyProfit, Deals.Short] = _result.MoneyProfitShort;
            _testerProfitLossDictionary[Categories.MoneyProfit, Deals.Market] = _result.MoneyProfitMarket;

            _testerProfitLossDictionary[Categories.PercentAggregatedProfit, Deals.All] = _result.PercentAggregatedProfitAll;
            _testerProfitLossDictionary[Categories.PercentAggregatedProfit, Deals.Long] = _result.PercentAggregatedProfitLong;
            _testerProfitLossDictionary[Categories.PercentAggregatedProfit, Deals.Short] = _result.PercentAggregatedProfitShort;

            _testerProfitLossDictionary[Categories.PercentDrawdown, Deals.All] = _result.PercentDrawdownAll;
            _testerProfitLossDictionary[Categories.PercentDrawdown, Deals.Long] = _result.PercentDrawdownLong;
            _testerProfitLossDictionary[Categories.PercentDrawdown, Deals.Short] = _result.PercentDrawdownShort;

            _testerProfitLossDictionary[Categories.MoneyDrawdown, Deals.All] = _result.MoneyDrawdownAll;
            _testerProfitLossDictionary[Categories.MoneyDrawdown, Deals.Long] = _result.MoneyDrawdownLong;
            _testerProfitLossDictionary[Categories.MoneyDrawdown, Deals.Short] = _result.MoneyDrawdownShort;
        }
        public string FormatValue(Categories rowKey, double value)
        {
            if (_formatValuesDictionary.ContainsKey(rowKey))
            {
                return _formatValuesDictionary[rowKey](value);
            }

            return value.ToString();
        }

        public string FormatValueEmpty(Categories rowKey, double value)
        {
            if (_formatValuesDictionary.ContainsKey(rowKey))
            {
                return _formatValuesDictionary[rowKey](value);
            }

            return value.ToString();
        }

        public void FormatElement(Categories rowKey, double value, TextBlock element)
        {
            if (_formatElementsDictionary.ContainsKey(rowKey))
            {
                _formatElementsDictionary[rowKey](value, element);
            }
        }

        public string FormatRowHeader(Categories rowKey)
        {
            switch (rowKey)
            {
                case Categories.PercentAggregatedProfit:
                    return "Percent Profit (Aggregated)";
                case Categories.NumberOfDeals:
                    return "Total";
                case Categories.NumberOfTradedContracts:
                    return "Traded Contracts";
                case Categories.NumberOfProfitLossDeals:
                    return "Profit/Loss";                
                case Categories.NumberOfProfitLossDealsBigger025Sigma:
                    return "Profit/Loss > 0.25";
                case Categories.NumberOfProfitLossDealsBigger05Sigma:
                    return "Profit/Loss > 0.5";
                case Categories.NumberOfProfitLossDealsBigger075Sigma:
                    return "Profit/Loss > 0.75";
                case Categories.NumberOfProfitLossDealsBigger1Sigma:
                    return "Profit/Loss > 1";
                default:
                    return rowKey.ToString().ToSplitText();
            }
        }

    }
}
