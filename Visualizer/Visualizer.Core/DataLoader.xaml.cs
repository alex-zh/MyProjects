using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using Common;
using GraphicsLib.Classes;
using Microsoft.Win32;

namespace GraphicsLib
{
    /// <summary>
    /// Interaction logic for DataLoader.xaml
    /// </summary>
    public partial class DataLoader : UserControl
    {
        public DataLoader()
        {
            InitializeComponent();
            Init();
            tbSelectedFile.MouseDoubleClick += tbSelectedFile_MouseDoubleClick;
        }

        public event EventHandler DataLoaded;

        void OnDataLoaded()
        {
            if (DataLoaded != null)
            {
                DataLoaded(this, EventArgs.Empty);
            }
        }

        private List<Candle> _candles = new List<Candle>();

        public IEnumerable<Candle> GetLoadedCandles()
        {
            return _candles;
        }

        void tbSelectedFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectDataFile();
        }

        protected void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectDataFile();
        }

        public void Init()
        {
            var dataFormats = new List<KeyValuePair<string, int>>();

            foreach (int itemValue in Enum.GetValues(typeof(LoadedDataFormats)))
            {
                string name = GetFormatName(itemValue);
                dataFormats.Add(new KeyValuePair<string, int>(name, itemValue));
            }

            cbSelectFormat.ItemsSource = dataFormats;
            cbSelectFormat.DisplayMemberPath = "Key";
            cbSelectFormat.SelectedValuePath = "Value";
            cbSelectFormat.SelectedIndex = 0;
        }

        private LoadedDataFormats GetSelectedDataFormat()
        {
            return (LoadedDataFormats)cbSelectFormat.SelectedValue;            
        }

        private string GetFormatName(int itemValue)
        {
            if (itemValue == (int)LoadedDataFormats.DATE_TIME_OPEN_HIGH_LOW_CLOSE_VOL)
            {
                return "<DATE>;<TIME>;<OPEN>;<HIGH>;<LOW>;<CLOSE>;<VOL>";
            }

            return "not supported";
        }

        protected void btnLoadData_Click(object sender, RoutedEventArgs e)
        {
            _candles.Clear();

            var skipFirstRow = true;

            var file = new FileInfo(tbSelectedFile.Text);

            using (StreamReader sr = file.OpenText())
            {
                string line;
                while (String.IsNullOrEmpty(line = sr.ReadLine()) == false)
                {
                    if (skipFirstRow) { skipFirstRow = false; continue; }
                    _candles.Add(ParseCandleLine(line));                    
                }
            }

            OnDataLoaded();
        }

        private Candle ParseCandleLine(string line)
        {
            if (String.IsNullOrEmpty(line))
            {
                throw new ArgumentNullException("line");
            }

            var dataFormat = GetSelectedDataFormat();

            var data = line.Split(new[] { ";" }, StringSplitOptions.None);

            if (dataFormat == LoadedDataFormats.DATE_TIME_OPEN_HIGH_LOW_CLOSE_VOL)
            {
                var date = DateTime.ParseExact(data[0], "yyyyMMdd", CultureInfo.InvariantCulture);
                var time = DateTime.ParseExact(data[1], "HHmmss", CultureInfo.InvariantCulture);
                                
                return new Candle()
                {
                    Date = date.Date + time.TimeOfDay,
                    Open = double.Parse(data[2]),
                    High = double.Parse(data[3]),
                    Low = double.Parse(data[4]),
                    Close = double.Parse(data[5]),
                    Volume = double.Parse(data[6])
                };
            }

            return null;
        }


        private void SelectDataFile()
        {
            OpenFileDialog open = new OpenFileDialog();

            open.Multiselect = false;

            open.Filter = "AllFiles|*.*";

            if ((bool)open.ShowDialog())
            {
                tbSelectedFile.Text = open.FileName;
                btnLoadData.IsEnabled = true;
            }
        }
    }
}
