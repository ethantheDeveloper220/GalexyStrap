using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Voidstrap.Helpers;

namespace Voidstrap.UI.Elements.ActivityLogger
{
    public partial class ActivityLoggerWindow
    {
        private ObservableCollection<LogEntry> _allLogs = new();
        private ObservableCollection<LogEntry> _filteredLogs = new();

        public ActivityLoggerWindow()
        {
            InitializeComponent();
            LogItemsControl.ItemsSource = _filteredLogs;
            
            // Subscribe to activity logger events
            Helpers.ActivityLogger.LogAdded += OnLogAdded;
            
            // Log that the window opened
            Helpers.ActivityLogger.Log("Activity Logger window opened", LogLevel.Info);
            
            // Prevent closing when main window closes
            this.Closing += (s, e) =>
            {
                e.Cancel = true;
                this.Hide();
            };
        }

        private void OnLogAdded(LogEntry entry)
        {
            Dispatcher.Invoke(() =>
            {
                _allLogs.Add(entry);
                ApplyFilter();
                UpdateStatus();
                
                if (AutoScrollToggle.IsChecked == true)
                {
                    LogScrollViewer.ScrollToEnd();
                }
            });
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string filter = FilterTextBox.Text?.ToLower() ?? "";
            
            _filteredLogs.Clear();
            foreach (var log in _allLogs)
            {
                if (string.IsNullOrEmpty(filter) || 
                    log.Message.ToLower().Contains(filter) ||
                    log.Level.ToString().ToLower().Contains(filter))
                {
                    _filteredLogs.Add(log);
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear all logs?",
                "Clear Logs",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                _allLogs.Clear();
                _filteredLogs.Clear();
                UpdateStatus();
                Helpers.ActivityLogger.Log("Logs cleared", LogLevel.Info);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|Log Files (*.log)|*.log|All Files (*.*)|*.*",
                FileName = $"Bloodstrap_ActivityLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (var writer = new StreamWriter(dialog.FileName))
                    {
                        writer.WriteLine($"Bloodstrap Activity Log");
                        writer.WriteLine($"Generated: {DateTime.Now}");
                        writer.WriteLine($"Total Events: {_allLogs.Count}");
                        writer.WriteLine(new string('=', 80));
                        writer.WriteLine();

                        foreach (var log in _allLogs)
                        {
                            writer.WriteLine($"[{log.Timestamp}] [{log.Level}] {log.Message}");
                        }
                    }

                    Helpers.ActivityLogger.Log($"Logs exported to: {dialog.FileName}", LogLevel.Success);
                    MessageBox.Show("Logs exported successfully!", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Helpers.ActivityLogger.Log($"Failed to export logs: {ex.Message}", LogLevel.Error);
                    MessageBox.Show($"Failed to export logs: {ex.Message}", "Export Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateStatus()
        {
            StatusText.Text = $"Ready - {_allLogs.Count} events logged ({_filteredLogs.Count} shown)";
        }

        protected override void OnClosed(EventArgs e)
        {
            Helpers.ActivityLogger.LogAdded -= OnLogAdded;
            base.OnClosed(e);
        }
    }
}
