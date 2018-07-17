using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Shapes;
using Common;
using Common.Classes;
using Common.Classes.General;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Visualizer
{   
    public partial class MainWindow
    {        
        public MainWindow()
        {
            InitializeComponent();            
        }           

        private void LoadMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Multiselect = false, Filter = "Text|*.txt|All|*.*" };
            dialog.InitialDirectory = @"C:\Users\aaz\Documents\My Projects\TradeIdeasTester\Visualizer\Visualizer\AppData";

            if (dialog.ShowDialog() == true)
            {                
                var candles = new CandlesLoader().FromFile(dialog.FileName);

                chart.Init(candles);               
            }            
        }

        private void LoadSpreadMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Multiselect = false, Filter = "Text|*.txt|All|*.*" };

            dialog.InitialDirectory = @"C:\MyProjects\Visualizer\Visualizer\AppData\";

            IEnumerable<Candle> firstSeries = new List<Candle>(), 
                                secondSeries = new List<Candle>();

            if (dialog.ShowDialog() == true)
            {
                firstSeries = new CandlesLoader().FromFile(dialog.FileName);

                if (dialog.ShowDialog() == true)
                {
                    secondSeries = new CandlesLoader().FromFile(dialog.FileName);
                }

            }

            var spreads = new SpreadCalculator().GetSpread(firstSeries, secondSeries);

            var dumper = new Dumper<object>(@"C:\Result\result.txt");
            foreach (var datePrice in spreads)
            {
                var d = datePrice.Date;
                dumper.WriteLine(int.Parse(d.Month.ToString("d2") + d.Day.ToString("d2") + d.Hour.ToString("d2")) + " " + datePrice.Price + " " + datePrice.Percent);
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var workingRectangle = Screen.PrimaryScreen.WorkingArea;

            this.Width = workingRectangle.Width * 0.75;
            this.Height = workingRectangle.Height * 0.75;
        }



    }

  
}
