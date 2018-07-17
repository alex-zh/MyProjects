using System.Windows;
using System.Windows.Input;

namespace Viewer.Windows
{    
    public partial class StockChartGoToForm
    {
        public StockChartGoToForm()
        {
            InitializeComponent();
            Loaded += StockChartGoToForm_Loaded;
            PreviewKeyDown += StockChartGoToForm_PreviewKeyDown;
        }

        void StockChartGoToForm_Loaded(object sender, RoutedEventArgs e)
        {
            tbGoToValue.Focus();
        }

        void StockChartGoToForm_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                HandleCancel();
            }
            else if(e.Key == Key.Enter)
            {
                e.Handled = true;
                HandleOK();
            }
        }

        private void HandleCancel()
        {
            DialogResult = false;
            this.Close();
        }

        public int SelectedIndex { get; private set; }

        

        private void HandleOK()
        {
            if (string.IsNullOrEmpty(tbGoToValue.Text))
            {
                MessageBox.Show("Value cannot be empty!");
                return;
            }

            int index;
            if (int.TryParse(tbGoToValue.Text, out index))
            {
                DialogResult = true;
                SelectedIndex = index;
            }
            else
            {
                MessageBox.Show("Value should be integer!");
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
           HandleCancel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            HandleOK();
        }
    }
}
