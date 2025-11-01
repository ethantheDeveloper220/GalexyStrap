using System.Windows;
using Voidstrap.UI.Elements.Base;
using Voidstrap.UI.Elements.ContextMenu;

namespace Voidstrap.UI.Elements.Dialogs
{
    /// <summary>
    /// Global Settings Dialog - Unified access to all Froststrap and Voidstrap features
    /// </summary>
    public partial class GlobalSettingsDialog : WpfUiWindow
    {
        public GlobalSettingsDialog()
        {
            InitializeComponent();
        }

        // Configuration Management
        private void OpenConfigBackup_Click(object sender, RoutedEventArgs e)
        {
            new ConfigBackupDialog().ShowDialog();
        }

        // Froststrap Features
        private void OpenDebugMenu_Click(object sender, RoutedEventArgs e)
        {
            new DebugMenu().ShowDialog();
        }

        private void OpenAdvancedSettings_Click(object sender, RoutedEventArgs e)
        {
            new AdvancedSettingsDialog().ShowDialog();
        }

        private void OpenFlagDialog_Click(object sender, RoutedEventArgs e)
        {
            // Note: FlagDialog requires parameters, this is a placeholder
            Frontend.ShowMessageBox("Flag Dialog requires FastFlag editor context to open.", MessageBoxImage.Information);
        }

        private void OpenRobloxSettings_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Roblox Settings page is available in the main settings menu.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Bloodstrap Features
        private void OpenAccountConsole_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Account Console requires additional context to open.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenDataCenterConsole_Click(object sender, RoutedEventArgs e)
        {
            new BetterBloxDataCenterConsole().ShowDialog();
        }

        private void OpenChatLogs_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Chat Logs requires ActivityWatcher context to open.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenGamePassConsole_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Game Pass Console requires user ID to open.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenMusicConsole_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Music Console requires additional context to open.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenOutputConsole_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Output Console requires ActivityWatcher context to open.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenRPCWindow_Click(object sender, RoutedEventArgs e)
        {
            new RPCWindow().ShowDialog();
        }

        private void OpenCursorPreview_Click(object sender, RoutedEventArgs e)
        {
            new CursorPreviewDialog().ShowDialog();
        }

        private void OpenFFlagPresets_Click(object sender, RoutedEventArgs e)
        {
            new FFlagPresetsDialog().ShowDialog();
        }

        private void OpenFFlagSearch_Click(object sender, RoutedEventArgs e)
        {
            new FFlagSearchDialog().ShowDialog();
        }
    }
}
