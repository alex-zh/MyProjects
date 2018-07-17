using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using RegessionAnalisys.Properties;

namespace RegessionAnalisys
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitSettings();
        }
    
        private void InitSettings()
        {
            if (String.IsNullOrEmpty(Settings.Default.PathToFiles))
            {
                Settings.Default.PathToFiles = Directory.GetCurrentDirectory();
            }
            Settings.Default.Save();
        }
    }
}
