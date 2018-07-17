using System.Windows;
using Common;
using Common.Classes.StrategyTester;
using Common.StrategiesOptions;

namespace Robot.TesterApplication.Windows
{    
    public partial class ExportResultWindow
    {
        public ExportResultWindow()
        {
            InitializeComponent();
        }

        public void BindData(TradeDataStatistics result, IStrategyOptions options)
        {
            StatisticsInfo.BindData(result, options);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
