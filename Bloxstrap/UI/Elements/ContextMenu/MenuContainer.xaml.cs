using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Voidstrap.Integrations;
using Voidstrap.UI.Elements.Dialogs;
using Voidstrap.UI.Elements.Settings;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Voidstrap.UI.Elements.ContextMenu
{
    /// <summary>
    /// Interaction logic for NotifyIconMenu.xaml
    /// </summary>
    public partial class MenuContainer
    {
        // i wouldve gladly done this as mvvm but turns out that data binding just does not work with menuitems for some reason so idk this sucks

        private readonly Watcher _watcher;

        private ActivityWatcher? _activityWatcher => _watcher.ActivityWatcher;

        private ServerInformation? _serverInformationWindow;

        private ServerHistory? _gameHistoryWindow;

        private MusicPlayer? _musicplayerWindow;

        private GamePassConsole? _GamepassWindow;

        private BetterBloxDataCenterConsole? _betterbloxWindow;

        private OutputConsole? _OutputConsole;

        private ChatLogs? _ChatLogs;

        private PerformanceMonitor.PerformanceMonitorWindow? _performanceMonitorWindow;

        private PerformanceMonitor.PerformanceOverlay? _performanceOverlay;

        private TimeSpan playTime = TimeSpan.Zero;
        private DispatcherTimer playTimer;
        private DispatcherTimer statusTimer;

        public MenuContainer(Watcher watcher)
        {
            InitializeComponent();
            StartPlayTimeTimer();
            StartRobloxStatusChecker();
            _watcher = watcher;

            if (_activityWatcher is not null)
            {
                _activityWatcher.OnLogOpen += ActivityWatcher_OnLogOpen;
                _activityWatcher.OnGameJoin += ActivityWatcher_OnGameJoin;
                _activityWatcher.OnGameLeave += ActivityWatcher_OnGameLeave;

                if (!App.Settings.Prop.UseDisableAppPatch)
                    GameHistoryMenuItem.Visibility = Visibility.Visible;
                if (!App.Settings.Prop.UseDisableAppPatch)
                    MusicMenuItem.Visibility = Visibility.Visible;
            }

            if (_watcher.RichPresence is not null)
                RichPresenceMenuItem.Visibility = Visibility.Visible;

            VersionTextBlock.Text = $"{App.ProjectName} v{App.Version}";
        }

        public void ShowServerInformationWindow()
        {
            if (_serverInformationWindow is null)
            {
                _serverInformationWindow = new(_watcher);
                _serverInformationWindow.Closed += (_, _) => _serverInformationWindow = null;
            }

            if (!_serverInformationWindow.IsVisible)
                _serverInformationWindow.ShowDialog();
            else
                _serverInformationWindow.Activate();
        }

        public void ActivityWatcher_OnLogOpen(object? sender, EventArgs e) => 
            Dispatcher.Invoke(() => LogTracerMenuItem.Visibility = Visibility.Visible);

        public void ActivityWatcher_OnGameJoin(object? sender, EventArgs e)
        {
            if (_activityWatcher is null)
                return;

            Dispatcher.Invoke(() => {
                if (_activityWatcher.Data.ServerType == ServerType.Public)
                    InviteDeeplinkMenuItem.Visibility = Visibility.Visible;

                ServerDetailsMenuItem.Visibility = Visibility.Visible;
                GamePassDetailsMenuItem.Visibility = Visibility.Visible;

                if (App.FastFlags.GetPreset("Players.LogLevel") == "trace")
                {
                    OutputConsoleMenuItem.Visibility = Visibility.Visible;
                    ChatLogsMenuItem.Visibility = Visibility.Visible;
                }
            });
        }

        public void ActivityWatcher_OnGameLeave(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() => {
                InviteDeeplinkMenuItem.Visibility = Visibility.Collapsed;
                ServerDetailsMenuItem.Visibility = Visibility.Collapsed;
                GamePassDetailsMenuItem.Visibility = Visibility.Collapsed;

                if (App.FastFlags.GetPreset("Players.LogLevel") == "trace")
                {
                    OutputConsoleMenuItem.Visibility = Visibility.Collapsed;
                    ChatLogsMenuItem.Visibility = Visibility.Collapsed;

                    _ChatLogs?.Close();
                    _OutputConsole?.Close();
                }

                _serverInformationWindow?.Close();
            });
        }

        private void StartPlayTimeTimer()
        {
            playTimer = new DispatcherTimer();
            playTimer.Interval = TimeSpan.FromSeconds(1);
            playTimer.Tick += PlayTimer_Tick;
            playTimer.Start();
        }

        private void PlayTimer_Tick(object sender, EventArgs e)
        {
            playTime = playTime.Add(TimeSpan.FromSeconds(1));
            UpdatePlayTime(playTime);
        }

        private void UpdatePlayTime(TimeSpan playTime)
        {
            PlayTimeTextBlock.Text = $"PlayTime: {playTime:hh\\:mm\\:ss}";
        }

        private void Window_Loaded(object? sender, RoutedEventArgs e)
        {
            // this is an awful hack lmao im so sorry to anyone who reads this
            // this is done to register the context menu wrapper as a tool window so it doesnt appear in the alt+tab switcher
            // https://stackoverflow.com/a/551847/11852173

            HWND hWnd = (HWND)new WindowInteropHelper(this).Handle;

            int exStyle = PInvoke.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
            exStyle |= 0x00000080; //NativeMethods.WS_EX_TOOLWINDOW;
            PInvoke.SetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, exStyle);
        }

        private void Window_Closed(object sender, EventArgs e) => App.Logger.WriteLine("MenuContainer::Window_Closed", "Context menu container closed");

        private void RichPresenceMenuItem_Click(object sender, RoutedEventArgs e) => _watcher.RichPresence?.SetVisibility(((MenuItem)sender).IsChecked);

        private void InviteDeeplinkMenuItem_Click(object sender, RoutedEventArgs e) => Clipboard.SetDataObject(_activityWatcher?.Data.GetInviteDeeplink());

        private void ServerDetailsMenuItem_Click(object sender, RoutedEventArgs e) => ShowServerInformationWindow();

        private void LogTracerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string? location = _activityWatcher?.LogLocation;

            if (location is not null)
                Utilities.ShellExecute(location);
        }

        private void CloseRobloxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = Frontend.ShowMessageBox(
                Strings.ContextMenu_CloseRobloxMessage,
                MessageBoxImage.Warning,
                MessageBoxButton.YesNo
            );

            if (result != MessageBoxResult.Yes)
                return;

            _watcher.KillRobloxProcess();
        }

        private void JoinLastServerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_activityWatcher is null)
                throw new ArgumentNullException(nameof(_activityWatcher));

            if (_gameHistoryWindow is null)
            {
                _gameHistoryWindow = new(_activityWatcher);
                _gameHistoryWindow.Closed += (_, _) => _gameHistoryWindow = null;
            }

            if (!_gameHistoryWindow.IsVisible)
                _gameHistoryWindow.ShowDialog();
            else
                _gameHistoryWindow.Activate();
        }
        private void GamePassDetailsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_activityWatcher is null)
                throw new ArgumentNullException(nameof(_activityWatcher));
            long userId = _activityWatcher.Data.UserId;

            if (_GamepassWindow is null)
            {
                _GamepassWindow = new GamePassConsole(userId);
                _GamepassWindow.Closed += (_, _) => _GamepassWindow = null;
            }

            if (!_GamepassWindow.IsVisible)
                _GamepassWindow.ShowDialog();
            else
                _GamepassWindow.Activate();
        }

        private void BetterBloxDataCentersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_activityWatcher is null)
                throw new ArgumentNullException(nameof(_activityWatcher));

            if (_betterbloxWindow is null)
            {
                _betterbloxWindow = new BetterBloxDataCenterConsole();
                _betterbloxWindow.Closed += (_, _) => _betterbloxWindow = null;
            }

            if (!_betterbloxWindow.IsVisible)
                _betterbloxWindow.ShowDialog();
            else
                _betterbloxWindow.Activate();
        }

        private void MusicPlayerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_activityWatcher is null)
                throw new ArgumentNullException(nameof(_activityWatcher));

            if (_musicplayerWindow is null)
            {
                _musicplayerWindow = new MusicPlayer();
                _musicplayerWindow.Closed += (_, _) => _musicplayerWindow = null;
            }

            if (!_musicplayerWindow.IsVisible)
                _musicplayerWindow.ShowDialog();
            else
                _musicplayerWindow.Activate();
        }

        private void OutputConsoleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_activityWatcher is null)
                throw new ArgumentNullException(nameof(_activityWatcher));

            if (_OutputConsole is null)
            {
                _OutputConsole = new(_activityWatcher);
                _OutputConsole.Closed += (_, _) => _OutputConsole = null;
            }

            if (!_OutputConsole.IsVisible)
                _OutputConsole.ShowDialog();
            else
                _OutputConsole.Activate();
        }

        private void ChatLogsMenuItemMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_activityWatcher is null)
                throw new ArgumentNullException(nameof(_activityWatcher));

            if (_ChatLogs is null)
            {
                _ChatLogs = new(_activityWatcher);
                _ChatLogs.Closed += (_, _) => _ChatLogs = null;
            }

            if (!_ChatLogs.IsVisible)
                _ChatLogs.ShowDialog();
            else
                _ChatLogs.Activate();
        }

        private void PerformanceMonitorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_performanceMonitorWindow is null)
            {
                _performanceMonitorWindow = new PerformanceMonitor.PerformanceMonitorWindow();
                _performanceMonitorWindow.Closed += (_, _) => _performanceMonitorWindow = null;
            }

            if (!_performanceMonitorWindow.IsVisible)
                _performanceMonitorWindow.Show();
            else
                _performanceMonitorWindow.Activate();
        }

        private void PerformanceOverlayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_performanceOverlay is null)
            {
                _performanceOverlay = new PerformanceMonitor.PerformanceOverlay();
                _performanceOverlay.Closed += (_, _) => _performanceOverlay = null;
                _performanceOverlay.Show();
            }
            else
            {
                _performanceOverlay.Close();
                _performanceOverlay = null;
            }
        }

        private void WebhookManagerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var webhookManager = new WebhookManagerDialog();
            webhookManager.Show();
        }

        private void ShowSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Check if settings window is already open
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow mainWindow)
                {
                    mainWindow.Activate();
                    mainWindow.WindowState = System.Windows.WindowState.Normal;
                    return;
                }
            }

            // If not open, create new settings window
            var settingsWindow = new MainWindow(false);
            settingsWindow.Show();
        }

        private void ExitGalaxyStrapMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = Frontend.ShowMessageBox(
                "Are you sure you want to exit GalaxyStrap?",
                MessageBoxImage.Question,
                MessageBoxButton.YesNo
            );

            if (result != MessageBoxResult.Yes)
                return;

            App.Terminate();
        }

        // Roblox Status Checker
        private void StartRobloxStatusChecker()
        {
            statusTimer = new DispatcherTimer();
            statusTimer.Interval = TimeSpan.FromMinutes(2); // Check every 2 minutes
            statusTimer.Tick += async (s, e) => await CheckRobloxStatus();
            statusTimer.Start();

            // Check immediately on startup
            Task.Run(async () => await CheckRobloxStatus());
        }

        private async Task CheckRobloxStatus()
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);

                // Use Roblox Status API
                var response = await client.GetAsync("https://status.roblox.com/api/v2/status");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var statusData = JsonSerializer.Deserialize<RobloxStatusResponse>(json);

                    Dispatcher.Invoke(() =>
                    {
                        if (statusData?.status?.indicator == "none")
                        {
                            RobloxStatusText.Text = "Roblox Status: ✅ All Systems Operational";
                            RobloxStatusText.Foreground = Brushes.Green;
                            RobloxStatusIcon.Foreground = Brushes.Green;
                        }
                        else if (statusData?.status?.indicator == "minor")
                        {
                            RobloxStatusText.Text = $"Roblox Status: ⚠️ {statusData.status.description}";
                            RobloxStatusText.Foreground = Brushes.Orange;
                            RobloxStatusIcon.Foreground = Brushes.Orange;
                        }
                        else if (statusData?.status?.indicator == "major" || statusData?.status?.indicator == "critical")
                        {
                            RobloxStatusText.Text = $"Roblox Status: ❌ {statusData.status.description}";
                            RobloxStatusText.Foreground = Brushes.Red;
                            RobloxStatusIcon.Foreground = Brushes.Red;
                        }
                        else
                        {
                            RobloxStatusText.Text = "Roblox Status: ⚠️ Unknown";
                            RobloxStatusText.Foreground = Brushes.Gray;
                            RobloxStatusIcon.Foreground = Brushes.Gray;
                        }
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        RobloxStatusText.Text = "Roblox Status: ❌ Unable to check";
                        RobloxStatusText.Foreground = Brushes.Gray;
                        RobloxStatusIcon.Foreground = Brushes.Gray;
                    });
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("MenuContainer::CheckRobloxStatus", $"Failed to check Roblox status: {ex.Message}");
                Dispatcher.Invoke(() =>
                {
                    RobloxStatusText.Text = "Roblox Status: ⚠️ Check failed";
                    RobloxStatusText.Foreground = Brushes.Gray;
                    RobloxStatusIcon.Foreground = Brushes.Gray;
                });
            }
        }

        private void RobloxStatusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Refresh status on click
            Task.Run(async () => await CheckRobloxStatus());
            
            // Open Roblox status page
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://status.roblox.com",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("MenuContainer::RobloxStatusMenuItem_Click", $"Failed to open status page: {ex.Message}");
            }
        }

        // Data models for Roblox Status API
        private class RobloxStatusResponse
        {
            public StatusInfo? status { get; set; }
        }

        private class StatusInfo
        {
            public string? indicator { get; set; }
            public string? description { get; set; }
        }
    }
}
