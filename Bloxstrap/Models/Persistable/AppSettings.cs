using System.Collections.ObjectModel;
using System.IO;
using Voidstrap.Enums;
using Voidstrap.UI.Elements.Dialogs;

namespace Voidstrap.Models.Persistable
{
    /// <summary>
    /// Represents configuration settings for Voidstrap.
    /// </summary>
    public class AppSettings
    {
        // General Configuration
        public BootstrapperStyle BootstrapperStyle { get; set; } = BootstrapperStyle.FluentAeroDialog;
        public BootstrapperIcon BootstrapperIcon { get; set; } = BootstrapperIcon.IconVoidstrap;
        public CleanerOptions CleanerOptions { get; set; } = CleanerOptions.Never;
        public List<string> CleanerDirectories { get; set; } = new List<string>();
        public string BootstrapperTitle { get; set; } = App.ProjectName;
        public string BootstrapperIconCustomLocation { get; set; } = "";
        public Theme Theme { get; set; } = Theme.Voidstrap;
        public string? SelectedCustomTheme { get; set; } = null;

        public bool CheckForUpdates { get; set; } = false;
        public string SelectedCpuPriority { get; set; } = "Automatic";
        public int MaxCpuCores { get; set; } = Environment.ProcessorCount;
        public int TotalLogicalCores { get; set; } = Environment.ProcessorCount;
        public int TotalPhysicalCores { get; set; } = Environment.ProcessorCount;
        public bool IsChannelEnabled { get; set; } = false;
        public bool UpdateRoblox { get; set; } = true;
        public bool DisableCrash { get; set; } = false;
        public int CpuCoreLimit { get; set; } = Environment.ProcessorCount;
        public string ShiftlockCursorSelectedPath { get; set; } = "";
        public string UseCustomIcon { get; set; } = "";
        public string CustomGameName { get; set; } = "";
        public string PriorityLimit { get; set; } = "Normal";
        public string SelectedStatus { get; set; } = "Gray";
        public string ArrowCursorSelectedPath { get; set; } = "";
        public string ArrowFarCursorSelectedPath { get; set; } = "";
        public string IBeamCursorSelectedPath { get; set; } = "";

        public bool DisableSplashScreen { get; set; } = true;
        public bool EnableAnalytics { get; set; } = true;
        public bool ShouldExportConfig { get; set; } = true;
        public bool ShouldExportLogs { get; set; } = true;
        public bool UseFastFlagManager { get; set; } = true;
        public bool WPFSoftwareRender { get; set; } = false;
        public bool FixTeleports { get; set; } = false;

        // External Tools
        public List<ExternalTool> ExternalTools { get; set; } = new List<ExternalTool>();

        // Gameplay Tools
        public bool EnableAntiAFK { get; set; } = false;
        public int AntiAFKInterval { get; set; } = 15;
        public bool AntiAFKRandomize { get; set; } = true;
        public bool AntiAFKRandomPosition { get; set; } = true;
        public bool EnableAutoRejoin { get; set; } = false;
        public int AutoRejoinDelay { get; set; } = 5;
        public int MaxRejoinAttempts { get; set; } = 3;
        public bool AutoClearCache { get; set; } = false;
        public bool ReduceMotionBlur { get; set; } = false;
        public bool BoostLoadingSpeed { get; set; } = false;

        // Launch Configuration
        public bool ConfirmLaunches { get; set; } = false;
        public bool HasLaunchedGame { get; set; } = false;
        public bool OptimizeRoblox { get; set; } = false;
        public bool BackgroundUpdatesEnabled { get; set; } = true;
        public bool MultiInstanceLaunching { get; set; } = false;
        public bool EnableCustomStatusDisplay { get; set; } = true;
        public bool RenameClientToEuroTrucks2 { get; set; } = false;
        public string ClientPath { get; set; } = Path.Combine(Paths.Base, "Roblox", "Player");

        // Localization
        public string Locale { get; set; } = "nil";
        public bool ForceRobloxLanguage { get; set; } = true;
        public bool FastFlagBypass { get; set; } = false;

        // Analytics & Tracking

        public bool DarkTextures { get; set; } = false;
        public bool EnableActivityTracking { get; set; } = true;
        public bool OverClockCPU { get; set; } = false;
        public bool exitondissy { get; set; } = false;
        public bool ServerUptimeBetterBLOXcuzitsbetterXD { get; set; } = true;

        public string DownloadingStringFormat { get; set; } = Strings.Bootstrapper_Status_Downloading + " {0} - {1}MB / {2}MB";
        public bool ConnectCloset { get; set; } = false;

        public bool GameIconChecked { get; set; } = true;
        public bool ServerLocationGame { get; set; } = false;
        public bool GameNameChecked { get; set; } = true;
        public bool GameCreatorChecked { get; set; } = true;
        public bool GameStatusChecked { get; set; } = true;

        public bool DX12Like { get; set; } = false;

        // Rich Presence (Discord Integration)
        public bool UseDiscordRichPresence { get; set; } = true;
        public bool HideRPCButtons { get; set; } = true;
        public bool ShowAccountOnRichPresence { get; set; } = true;
        public bool MultiAccount { get; set; } = false;
        public bool ShowServerDetails { get; set; } = true;

        // Mod Settings
        public string CustomFontLocation { get; set; } = string.Empty;
        public CursorType CursorType { get; set; } = CursorType.Default;

        // Custom Integrations
        public ObservableCollection<CustomIntegration> CustomIntegrations { get; set; } = new();

        // Mod Preset Configuration
        public bool UseDisableAppPatch { get; set; } = false;

        // Roblox Deployment Settings
        public string Channel { get; set; } = RobloxInterfaces.Deployment.DefaultChannel;
        public string ChannelHash { get; set; } = "";

        public string LaunchGameID { get; set; } = "";
        public bool IsGameEnabled { get; set; } = false;
        public bool IsBetterServersEnabled { get; set; } = false;
        public bool OverClockGPU { get; set; } = false;

        // Notification Settings
        public bool UseTrayTip { get; set; } = true;
        public bool UseMessageBox { get; set; } = false;
        public bool NotifyOnGameLaunch { get; set; } = true;
        public bool NotifyOnGameClose { get; set; } = false;
        public bool NotifyOnUpdate { get; set; } = true;
        public bool NotifyOnSettingsSaved { get; set; } = true;
        public bool NotifyOnErrors { get; set; } = true;
        public bool NotifyOnGalaxyStrapUpdate { get; set; } = true;
        public int TrayTipDuration { get; set; } = 5;
        public bool PlayNotificationSound { get; set; } = true;

        // Webhook Settings (Legacy - kept for backwards compatibility)
        public string WebhookUrl { get; set; } = "";
        public string WebhookUsername { get; set; } = "GalaxyStrap";
        public bool WebhookOnGameLaunch { get; set; } = false;
        public bool WebhookOnGameClose { get; set; } = false;
        public bool WebhookOnUpdate { get; set; } = false;
        public bool WebhookOnError { get; set; } = false;

        // New Webhook Manager Configuration
        public WebhookConfig? WebhookConfig { get; set; } = null;

        // Region & Server Settings
        public bool EnableRegionSelection { get; set; } = false;
        public string PreferredServerRegion { get; set; } = "auto";
        public bool EnableAutoSelectBestServer { get; set; } = false;
        public int MaxPingThreshold { get; set; } = 100;
        public bool ShowPingInGame { get; set; } = false;
        public bool ReduceNetworkLag { get; set; } = false;
        public bool IncreaseBandwidth { get; set; } = false;
        public bool OptimizePacketSize { get; set; } = false;
        public bool EnableFastPreloading { get; set; } = false;

        // Performance Monitor Settings
        public bool EnablePerformanceMonitor { get; set; } = true;
        public bool AutoStartPerformanceMonitor { get; set; } = false;
        public bool PerformanceMonitorAlwaysOnTop { get; set; } = false;
        public int PerformanceMonitorUpdateInterval { get; set; } = 500;
        
        // Display Options
        public bool ShowCpuUsage { get; set; } = true;
        public bool ShowMemoryUsage { get; set; } = true;
        public bool ShowFps { get; set; } = true;
        public bool ShowPing { get; set; } = true;
        public bool ShowGpuUsage { get; set; } = true;
        public bool ShowDiskActivity { get; set; } = true;
        
        // Performance Alerts
        public bool EnablePerformanceAlerts { get; set; } = false;
        public int CpuAlertThreshold { get; set; } = 80;
        public int MemoryAlertThreshold { get; set; } = 8000;
        public int FpsAlertThreshold { get; set; } = 30;
        
        // Performance Logging
        public bool EnablePerformanceLogging { get; set; } = false;
        public int PerformanceLogInterval { get; set; } = 30;
        public bool AutoDeleteOldPerformanceLogs { get; set; } = true;
    }

    public class ExternalTool
    {
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public string Arguments { get; set; } = "";
    }
}