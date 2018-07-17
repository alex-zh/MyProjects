using System.Collections;

namespace BitMex.TradeApplication.Controls
{
    public partial class OrdersGrid
    {
        public OrdersGrid()
        {
            InitializeComponent();
        }

        public IEnumerable ItemsSource
        {
            get { return DataGrid.ItemsSource; }
            set { DataGrid.ItemsSource = value; }
        } 
    }
}
