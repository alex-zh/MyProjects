using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using Robot.Core.Keepers;
using Robot.TradeApplication.Keepers;
using Application = System.Windows.Application;

namespace Robot.TradeApplication
{    
    public partial class MainWindow
    {                
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;            
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {        
            InitRobotViews();
        }

        private void InitRobotViews()
        {
            SiRobotViewControl.Init(RobotNames.TrendFollowerSi, Securities.Si);            
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var logger = LoggerKeeper.Get();

            logger.Log(e.Exception.ToString(), LogMessageTypes.Error);

            foreach (var ex in e.Exception.InnerExceptions)
            {
                logger.Log(ex.ToString(), LogMessageTypes.Error);    
            }
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LoggerKeeper.Get().Log(e.Exception.ToString(), LogMessageTypes.Error);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var workingRectangle = Screen.PrimaryScreen.WorkingArea;

            Width = workingRectangle.Width * 0.85;
            Height = workingRectangle.Height * 0.85;
        }
    }   
}
