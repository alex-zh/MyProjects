namespace Visualizer.VisualControls.Controls
{
    /// <summary>
    /// Interaction logic for CandleInfoBox.xaml
    /// </summary>
    public partial class CandleInfoBox
    {
        public CandleInfoBox()
        {
            InitializeComponent();
        }

        //public bool IsVisible
        //{
        //    get { return infoBox.Visibility == System.Windows.Visibility.Visible; }
        //    set
        //    {
        //        if (value)
        //        {
        //            infoBox.Visibility = System.Windows.Visibility.Visible;
        //        }
        //        else
        //        {
        //            infoBox.Visibility = System.Windows.Visibility.Hidden;
        //        }
        //    }
        //}

        public string CandleDate
        {
            set
            {
                tbCandleDate.Text = value;
            }
        }

        public string CandleClose
        {
            get
            {
                return tbCandleClose.Text;
            }
            set
            {
                tbCandleClose.Text = value;
            }
        }

        public string CandleOpen
        {
            get
            {
                return tbCandleOpen.Text;
            }
            set
            {
                tbCandleOpen.Text = value;
            }
        }

        public string CandleMin
        {
            get
            {
                return tbCandleMin.Text;
            }
            set
            {
                tbCandleMin.Text = value;
            }
        }   

        public string CandleMax
        {
            get
            {
                return tbCandleMax.Text;
            }
            set
            {
                tbCandleMax.Text = value;
            }
        }

        public string DeviationText
        {
            get
            {
                return tbDeviationInfo.Text;
            }
            set
            {
                tbDeviationInfo.Text = value;
            }
        }

        public string CandleIndex
        {
            get
            {
                return tbCandleIndex.Text;
            }
            set
            {
                tbCandleIndex.Text = value;
            }
        }

        public string CandleVolume
        {
            get
            {
                return tbCandleVolume.Text;
            }
            set
            {
                tbCandleVolume.Text = value;
            }
        }

        public string VolumeMedian
        {
            get
            {
                return tbVolumeMedian.Text;
            }
            set
            {
                tbVolumeMedian.Text = value;
            }
        }


        public string AtrValue
        {
            get
            {
                return tbAtrValue.Text;
            }
            set
            {
                tbAtrValue.Text = value;
            }
        }
    }
}
