using System.Linq;
using Common.Classes.StrategyTester;

namespace Robot.TesterApplication.Controls
{
    public partial class DealInfoControl
    {
        public DealInfoControl()
        {
            InitializeComponent();
        }

        public void BindData(Deal clickedDeal, int candleIndex)
        {
            if (clickedDeal.TradeItems.Any() == false) return;

            tbIndex.Text = candleIndex.ToString();
            tbStartDate.Text = clickedDeal.TradeItems.First().DateTime.ToString("dd/MM HH:mm:ss");
            tbEndDate.Text = clickedDeal.TradeItems.Last().DateTime.ToString("dd/MM HH:mm:ss");

            tbProfitInMoney.Text = clickedDeal.GetProfitInMoney().ToString("F2");
            tbProfitInPercent.Text = clickedDeal.GetAggregatedPercentProfit(1).ToString("F2");

            TradeItemsGrid.ItemsSource = clickedDeal.TradeItems;            
        }
    }
}
