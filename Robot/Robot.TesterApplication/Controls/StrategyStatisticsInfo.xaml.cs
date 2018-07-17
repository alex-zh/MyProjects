using Common;
using Common.Classes.StrategyTester;
using Common.StrategiesOptions;
using Robot.Core.Classes;
using Robot.TesterApplication.Classes;

namespace Robot.TesterApplication.Controls
{
    public partial class StrategyStatisticsInfo
    {
        private TradeDataStatisticsToGridConverter _statisticsToGridConverter;
        private readonly DynamicGridExporter _dynamicGridExporter = new DynamicGridExporter();
        private ObjectExporter<IStrategyOptions> _objectExporter = new ObjectExporter<IStrategyOptions>();

        public StrategyStatisticsInfo()
        {
            InitializeComponent();
        }

        public void BindData(TradeDataStatistics result, IStrategyOptions strategyOptions)
        {
            _statisticsToGridConverter = new TradeDataStatisticsToGridConverter(result, strategyOptions.MaxPositionSize);

            var strategyParamsInfo = _objectExporter.Export(strategyOptions);

            var profitLossInfo = _dynamicGridExporter.Export(_statisticsToGridConverter.TesterProfitLossDictionary,
                                                                _statisticsToGridConverter.FormatValue,
                                                                _statisticsToGridConverter.FormatRowHeader);

            var dealsNumbersInfo = _dynamicGridExporter.Export(_statisticsToGridConverter.TesterDealsNumbersDictionary,
                                                                  (c, s) => s,
                                                                  _statisticsToGridConverter.FormatRowHeader);

            tbSummary.Text = "\n" +  
                            "Config: " +"\n\n" +
                            strategyParamsInfo + "\n\n" +
                             "PnL: " + "\n\n" +
                            profitLossInfo + "\n\n" +
                            "Deals Numbers: " + "\n\n" +
                            dealsNumbersInfo;
        }
    }
}
