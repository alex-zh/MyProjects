using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpinionAnalyzer.DataLoader.Finam;

namespace OpinionAnalyzer.DataLoader.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FinamDataLoader _finamDataLoader = new FinamDataLoader();
        private FinamDataSaver _finamDataSaver;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var startDate = dpStartDate.SelectedDate;
            var endDate = dpEndDate.SelectedDate;

            if (startDate.HasValue == false || endDate.HasValue == false)
            {
                MessageBox.Show("Period is not selected!");
                return;
            }

            var path = tbPath.Text;
            var sd = startDate.Value;
            var ed = endDate.Value;

            var task = new Task(() => LoadData(path, sd, ed));

            task.Start();
        }

        private void LoadData(string path, DateTime startDate, DateTime endDate)
        {

            _finamDataSaver = new FinamDataSaver(path);

            for (DateTime d = startDate; d <= endDate; d = d.AddDays(1))
            {
                if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                var list = _finamDataLoader.GetList(d);

                foreach (var item in list)
                {
                   var article = _finamDataLoader.LoadArticle(item, d);
                    _finamDataSaver.Save(article);
                    Log(article);
                }
            }
        }

        private void Log(LoadedArticle article)
        {
            this.Dispatcher.Invoke(() =>
            {
                tbLog.AppendText(String.Format("{0}: {1}", article.PublishDate, article.Title));
                tbLog.AppendText("\r\n");
                tbLog.ScrollToEnd();
            });
        }
    }
}
