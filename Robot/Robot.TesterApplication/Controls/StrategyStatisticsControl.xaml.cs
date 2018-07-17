using Common.Classes.StrategyTester;
using Robot.Core.Classes;

namespace Robot.TesterApplication.Controls
{
    public partial class StrategyStatisticsControl
    {
        private TradeDataStatisticsToGridConverter _statisticsToGridConverter;
        
        public StrategyStatisticsControl()
        {
            InitializeComponent();
            
            BindData(new TradeDataStatistics(), 0);
        }
              
        public void BindData(TradeDataStatistics result, int maxPositionSize)
        {
            _statisticsToGridConverter = new TradeDataStatisticsToGridConverter(result, maxPositionSize);

            BindDealsNumbersGrid();
            BindProfitLossGrid();
        }

        private void BindDealsNumbersGrid()
        {
            DealsGrid.Build(_statisticsToGridConverter.TesterDealsNumbersDictionary, 
                            (categories, s) => s, 
                            (categories, s, args) => { }, 
                            _statisticsToGridConverter.FormatRowHeader);
        }
        
        private void BindProfitLossGrid()
        {                       
            ProfitLossGrid.Build(_statisticsToGridConverter.TesterProfitLossDictionary,
                                 _statisticsToGridConverter.FormatValue,
                                 _statisticsToGridConverter.FormatElement,
                                 _statisticsToGridConverter.FormatRowHeader);
        }              
    }
}
