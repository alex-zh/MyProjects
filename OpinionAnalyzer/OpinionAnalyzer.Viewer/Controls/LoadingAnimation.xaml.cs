using System.Windows.Controls;

namespace OpinionAnalyzer.Viewer.Controls
{
    /// <summary>
    /// Interaction logic for LoadingAnimation.xaml
    /// </summary>
    public partial class LoadingAnimation : UserControl
    {
        public LoadingAnimation()
        {
            InitializeComponent();
        }

        public string AnimationText
        {
            get { return tbText.Text; }
            set {  tbText.Text = value; }
        }
    }
}
