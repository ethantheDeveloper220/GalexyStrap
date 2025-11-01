using System.Diagnostics;
using System.IO;
using System.Windows;
using Voidstrap.UI.Elements.ActivityLogger;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class LoggingPage
    {
        private static ActivityLoggerWindow? _activityLoggerWindow;

        public LoggingPage()
        {
            InitializeComponent();
            
            // Set version info
            VersionText.Text = $"v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
            DotNetVersionText.Text = Environment.Version.ToString();
            OSVersionText.Text = Environment.OSVersion.ToString();
        }

        private void OpenActivityLogger_Click(object sender, RoutedEventArgs e)
        {
            if (_activityLoggerWindow == null || !_activityLoggerWindow.IsLoaded)
            {
                _activityLoggerWindow = new ActivityLoggerWindow();
                _activityLoggerWindow.Show();
            }
            else
            {
                _activityLoggerWindow.Show();
                _activityLoggerWindow.Activate();
            }
        }

        private void OpenLogsFolder_Click(object sender, RoutedEventArgs e)
        {
            string logsPath = Path.Combine(Paths.Logs);
            
            if (!Directory.Exists(logsPath))
            {
                Directory.CreateDirectory(logsPath);
            }

            Process.Start("explorer.exe", logsPath);
        }
    }
}
