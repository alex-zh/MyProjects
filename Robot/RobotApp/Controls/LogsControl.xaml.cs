using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Common.Classes.Logger;
using Robot.DataLayer.Repositories;

namespace Robot.TradeApplication.Controls
{    
    public partial class LogsControl
    {
        private Task _showLogsTask;
        private readonly LogsRepository _logsRepository = new LogsRepository();
        private readonly LogsFilterExpressionBuilder _filterExpressionBuilder = new LogsFilterExpressionBuilder();

        public LogsControl()
        {
            InitializeComponent();
            Loaded += LogsControl_Loaded;
        }      

        void LogsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_showLogsTask == null)
            {
                _showLogsTask = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        UpdateLogs();
                        Thread.Sleep(1000);
                    }
                });
            }
        }

        private void UpdateLogs()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    var logs = _logsRepository.GetAll(_filterExpressionBuilder.Build(tbErrors.IsChecked.GetValueOrDefault(),
                                                                                     tbWarnings.IsChecked.GetValueOrDefault(),
                                                                                     tbInfo.IsChecked.GetValueOrDefault())).ToList();
                    if (LogsGrid.Items.Count != logs.Count)
                    {
                        LogsGrid.ItemsSource = logs;

                        if (logs.Any())
                        {
                            LogsGrid.ScrollIntoView(logs.Last());
                        }
                    }
                });
            }
            catch (TaskCanceledException)
            {

            }
        }

        private void ErrorsFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateLogs();
        }

        private void WarningsFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateLogs();
        }

        private void InfoFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateLogs();
        }

        private void CleanLog_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clean the log?", "", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _logsRepository.ClearAll();
            }
        }
    }
}
