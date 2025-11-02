using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class PerformanceMonitorPage : UiPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public PerformanceMonitorPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        // Monitor Settings
        public bool EnablePerformanceMonitor
        {
            get => App.Settings?.Prop?.EnablePerformanceMonitor ?? true;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.EnablePerformanceMonitor = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool AutoStartMonitor
        {
            get => App.Settings?.Prop?.AutoStartPerformanceMonitor ?? false;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.AutoStartPerformanceMonitor = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool PerformanceMonitorAlwaysOnTop
        {
            get => App.Settings?.Prop?.PerformanceMonitorAlwaysOnTop ?? false;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.PerformanceMonitorAlwaysOnTop = value;
                    OnPropertyChanged();
                }
            }
        }

        public int UpdateInterval
        {
            get => App.Settings?.Prop?.PerformanceMonitorUpdateInterval ?? 500;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.PerformanceMonitorUpdateInterval = value;
                    OnPropertyChanged();
                }
            }
        }

        // Display Options
        public bool ShowCpuUsage
        {
            get => App.Settings?.Prop?.ShowCpuUsage ?? true;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.ShowCpuUsage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowMemoryUsage
        {
            get => App.Settings?.Prop?.ShowMemoryUsage ?? true;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.ShowMemoryUsage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowFps
        {
            get => App.Settings?.Prop?.ShowFps ?? true;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.ShowFps = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowPing
        {
            get => App.Settings?.Prop?.ShowPing ?? true;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.ShowPing = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowGpuUsage
        {
            get => App.Settings?.Prop?.ShowGpuUsage ?? true;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.ShowGpuUsage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowDiskActivity
        {
            get => App.Settings?.Prop?.ShowDiskActivity ?? true;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.ShowDiskActivity = value;
                    OnPropertyChanged();
                }
            }
        }

        // Alerts & Notifications
        public bool EnablePerformanceAlerts
        {
            get => App.Settings?.Prop?.EnablePerformanceAlerts ?? false;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.EnablePerformanceAlerts = value;
                    OnPropertyChanged();
                }
            }
        }

        public int CpuAlertThreshold
        {
            get => App.Settings?.Prop?.CpuAlertThreshold ?? 80;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.CpuAlertThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MemoryAlertThreshold
        {
            get => App.Settings?.Prop?.MemoryAlertThreshold ?? 8000;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.MemoryAlertThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        public int FpsAlertThreshold
        {
            get => App.Settings?.Prop?.FpsAlertThreshold ?? 30;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.FpsAlertThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        // Logging Options
        public bool EnablePerformanceLogging
        {
            get => App.Settings?.Prop?.EnablePerformanceLogging ?? false;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.EnablePerformanceLogging = value;
                    OnPropertyChanged();
                }
            }
        }

        public int LogInterval
        {
            get => App.Settings?.Prop?.PerformanceLogInterval ?? 30;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.PerformanceLogInterval = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool AutoDeleteOldLogs
        {
            get => App.Settings?.Prop?.AutoDeleteOldPerformanceLogs ?? true;
            set
            {
                if (App.Settings?.Prop != null)
                {
                    App.Settings.Prop.AutoDeleteOldPerformanceLogs = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OpenMonitor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var monitorWindow = new PerformanceMonitor.PerformanceMonitorWindow();
                monitorWindow.Show();
                
                App.Logger.WriteLine("[PerformanceMonitor]", "Performance Monitor window opened");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("[PerformanceMonitor]", $"Failed to open monitor window: {ex.Message}");
                MessageBox.Show(
                    $"Failed to open Performance Monitor:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
