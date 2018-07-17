using System;
using System.Globalization;
using System.Windows.Media;
using Common.Classes.Robot;
using Robot.Core.Providers;

namespace JointAssetsViewer.Controls
{
    public partial class QuikConnectionSummary
    {
        private const string ConnectedString = "Connected";
        private const string NotConnectedString = "Not Connected";

        private QuikDataProvider _quikDataProvider;

        public QuikConnectionSummary()
        {
            InitializeComponent();
        }

        public QuikDataProvider QuikDataProvider
        {
            get { return _quikDataProvider; }
            set
            {
                _quikDataProvider = value;
                _quikDataProvider.TimeChanged += _quikDataProvider_TimeChanged;
                _quikDataProvider.DataChanged += _quikDataProvider_DataChanged;
            }
        }

        void _quikDataProvider_DataChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {

                    var lastBrentCandle = _quikDataProvider.GetLastCandle(Securities.Br);
                    var lastSiCandle = _quikDataProvider.GetLastCandle(Securities.Br);

                    if (lastBrentCandle != null && lastSiCandle != null)
                    {
                        tbFirstAssertLastUpdateTime.Text = lastBrentCandle.Date.ToString("hh:mm:ss", CultureInfo.InvariantCulture);
                        tbSecondAssertLastUpdateTime.Text = lastSiCandle.Date.ToString("hh:mm:ss", CultureInfo.InvariantCulture);
                    }
                }
                catch (Exception)
                {

                }
            });
        }


        private void _quikDataProvider_TimeChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var quikTime = _quikDataProvider.GetServerTime();
                tbQuikTime.Text = quikTime.ToString("hh:mm:ss", CultureInfo.InvariantCulture);

                tbRobotTime.Text = DateTime.Now.ToString("hh:mm:ss", CultureInfo.InvariantCulture);

                if ((quikTime - DateTime.Now).Seconds > 1)
                {
                    tbQuikDdeStatus.Text = NotConnectedString;
                    tbQuikDdeStatus.Foreground = Brushes.DarkBlue;
                }
                else
                {
                    tbQuikDdeStatus.Text = ConnectedString;
                    tbQuikDdeStatus.Foreground = Brushes.Green;
                }
            });
        }

    }
}