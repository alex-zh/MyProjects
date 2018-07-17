namespace Visualizer.Core
{
      public partial class CursorInfoBox
    {
        public CursorInfoBox()
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

        public string Value
        {
            set
            {
                tbValue.Text = value;
            }
        }

        
    }
}
