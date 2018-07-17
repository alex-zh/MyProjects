using System.IO;
using System.Windows;
using Microsoft.Win32;
using RegessionAnalisys.Properties;

namespace RegessionAnalisys
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            FillData();
        }

        private void FillData()
        {
            tbPathToFiles.Text = Settings.Default.PathToFiles;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {            
            Settings.Default.PathToFiles = tbPathToFiles.Text;

            Settings.Default.Save();

            this.Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()    {                 
                Multiselect = true,                
                InitialDirectory = Settings.Default.PathToFiles
            };

            if (dialog.ShowDialog() == true)
            {
               tbPathToFiles.Text =   Path.GetDirectoryName(dialog.FileName);                
            }           
        }

        private void Clean_OnClick(object sender, RoutedEventArgs e)
        {
            var path = Settings.Default.PathToFiles;
            if (Directory.Exists(path))
            {
                foreach (FileInfo fileInfo in new DirectoryInfo(path).GetFiles())
                {
                    fileInfo.Delete();
                }
            }
        }
    }
}
