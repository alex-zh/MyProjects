using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Visualizer.VisualControls.Classes;
using Visualizer.VisualControls.Controls;
using MessageBox = System.Windows.MessageBox;

namespace FortsPositionsViewer
{    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            InitCombos();
        }

        private void InitCombos()
        {
            comboSelectRange.DisplayMemberPath = "Value";
            comboSelectRange.SelectedValuePath = "Key";
            comboSelectRange.Items.Add(new KeyValuePair<int, string>(7, "Last 7 days"));
            comboSelectRange.Items.Add(new KeyValuePair<int, string>(14, "Last 14 days"));
            comboSelectRange.Items.Add(new KeyValuePair<int, string>(30, "Last 30 days"));
            comboSelectRange.Items.Add(new KeyValuePair<int, string>(150, "Last 150 days"));
            comboSelectRange.Items.Add(new KeyValuePair<int, string>(300, "Last 300 days"));

            comboDataType.DisplayMemberPath = "Value";
            comboDataType.SelectedValuePath = "Key";
            comboDataType.Items.Add(new KeyValuePair<PositionsDataTypes, string>(PositionsDataTypes.NumberOfPeople, "Number of people"));
            comboDataType.Items.Add(new KeyValuePair<PositionsDataTypes, string>(PositionsDataTypes.NumberOfPositions, "Number of positions"));

            comboDeriative.DisplayMemberPath = "Value";
            comboDeriative.SelectedValuePath = "Key";

            foreach (DeriativeNames value in Enum.GetValues(typeof(DeriativeNames)))
            {
                comboDeriative.Items.Add(new KeyValuePair<DeriativeNames, string>(value, value.GetText()));
            }

            comboDeriative.SelectedIndex = 0;
            comboSelectRange.SelectedIndex = 0;
            comboDataType.SelectedIndex = 0;
        }

        private void ButtonShow_OnClick(object sender, RoutedEventArgs e)
        {                        
            ShowData();
        }

        private void ShowData()
        {
            try
            {
                var selectedDeriative = SelectedDeriative;
                var dataProvider = new FortsPositionsDataProvider(selectedDeriative);
                var data = dataProvider.Read();

                var individualsData = data.Individuals.OrderByDescending(x => x.Key)
                                                      .Take(DaysToShow).ToDictionary(x => x.Key, y => y.Value);

                Func< OpenInterestDataItem, double> longSelector = x =>
                                                    PositionsDataType == PositionsDataTypes.NumberOfPeople ? x.ClientsInLong : x.LongPosition;

                Func<OpenInterestDataItem, double> shortSelector = x =>
                                                   PositionsDataType == PositionsDataTypes.NumberOfPeople ? x.ClientsInShort : x.ShortPosition;

                var graph1 = GetGraphFor("Number In Long", individualsData, longSelector , Colors.Green);
                var graph2 = GetGraphFor("Number In Short", individualsData, shortSelector, Colors.Red);

                ChartIndividuals.WidthOffset = (PositionsDataType == PositionsDataTypes.NumberOfPositions) ? 80 : 60;

                ChartIndividuals.Init(new List<TimeGraphLine> { graph1, graph2 });
                ChartIndividuals.Draw();

                var legalsData = data.Legals.OrderByDescending(x => x.Key)
                                            .Take(DaysToShow).ToDictionary(x => x.Key, y => y.Value);

                var graph3 = GetGraphFor("Number In Long", legalsData, longSelector, Colors.Green);
                var graph4 = GetGraphFor("Number In Short", legalsData, shortSelector, Colors.Red);

                ChartLegals.WidthOffset = (PositionsDataType == PositionsDataTypes.NumberOfPositions) ? 80 : 60;

                ChartLegals.Init(new List<TimeGraphLine> { graph3, graph4 });
                ChartLegals.Draw();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private DeriativeNames SelectedDeriative
        {
            get
            {
                return ((KeyValuePair<DeriativeNames, string>)(comboDeriative.SelectedItem ?? comboDeriative.Items[0])).Key;                
                
            }
        }
        private int DaysToShow
        {
            get
            {
                return ((KeyValuePair<int, string>)(comboSelectRange.SelectedItem ?? comboSelectRange.Items[0])).Key;
            }
        }

        private PositionsDataTypes PositionsDataType
        {
            get
            {
                return ((KeyValuePair<PositionsDataTypes, string>)(comboDataType.SelectedItem?? comboDataType.Items[0])).Key;
            }
        }

        public TimeGraphLine GetGraphFor(string legend, 
                                         Dictionary<DateTime, OpenInterestDataItem> data, 
                                         Func<OpenInterestDataItem, double> valueSelector, 
                                         Color color)
        {
            var points = new List<PlainChartPoint>();
            foreach (var date in data.Keys)
            {
                var point = new PlainChartPoint() { DateTime = date, Value = valueSelector(data[date])};
                points.Add(point);
            }

            return new TimeGraphLine(points, AxisSides.Left, color, legend);
        }

        private async void Sync_OnClick(object sender, RoutedEventArgs e)
        {
            var previousContent = btnSync.Content;
            btnSync.Content = "In progress ...";

            var synchronizer = new FortsPositionsDataSynchronizer();
            await synchronizer.Synchronize(SelectedDeriative);           
           
            btnSync.Content = previousContent;
            ShowData();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var workingRectangle = Screen.PrimaryScreen.WorkingArea;

            Width = workingRectangle.Width * 0.85;
            Height = workingRectangle.Height * 0.85;
        }

        private void ComboSelectRange_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowData();                        
        }

        private void ComboDataType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowData();
        }

        private void comboDeriative_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowData();
        }
    }
}
