using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Common.Classes.StrategyTester;
using Robot.TesterApplication.Classes;
using Robot.TesterApplication.Properties;

namespace Robot.TesterApplication.Controls
{    
    public partial class HistoryStatisticsViewer
    {
        private List<KeyValuePair<string, string>> _paths = new List<KeyValuePair<string, string>>();
        private TradeDataStatistics _currentStatisticsData;
        public event EventHandler DataChanged;

        public HistoryStatisticsViewer()
        {
            InitializeComponent();

            BindHistoryListData();
        }

        private void BindHistoryListData()
        {
            var selectedValue = cbHistoryList.SelectedValue;

            _paths = new StatisticsHistoryResultsLoader().LoadPaths(Settings.Default.TestResultsFolderPath).ToList();

            cbHistoryList.ItemsSource = _paths;

            cbHistoryList.SelectedValuePath = "Value";
            cbHistoryList.DisplayMemberPath = "Key";

            cbHistoryList.SelectedValue = selectedValue;
        }

        public TradeDataStatistics CurrentStatistics
        {
            get { return _currentStatisticsData ?? (_currentStatisticsData = new TradeDataStatistics()); }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var fileName = cbHistoryList.Text;

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Enter the file name");
            }

            if (_paths.Any(x=>x.Key == fileName) &&
                MessageBox.Show("File exists, it will be overwritten. Is it OK?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
            {
                return;                
            }

            var data = new StrategyTestResult()
            {
                Description = tbDescription.Text,
                Statistics = _currentStatisticsData
            };

            new StatisticsHistoryResultsSaver(Settings.Default.TestResultsFolderPath).Save(fileName, data);            

            BindHistoryListData();
        }

        private void HistoryList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = (string) cbHistoryList.SelectedValue;

            if (string.IsNullOrEmpty(selectedValue) == false)
            {
                LoadAndBindData(selectedValue);    
            }            
        }

        private void LoadAndBindData(string filePath)
        {
            var data = new StatisticsHistoryResultsLoader().Load(filePath);

            _currentStatisticsData = data.Statistics;
            tbDescription.Text = data.Description;

            if (DataChanged != null)
            {
                DataChanged(this, EventArgs.Empty);
            }
        }

        public void BindStatisticsData(TradeDataStatistics result)
        {
            cbHistoryList.SelectedIndex = -1;
            tbDescription.Text = String.Empty;
            
            _currentStatisticsData = result;            
        }
    }
}
