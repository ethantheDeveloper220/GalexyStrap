namespace Voidstrap
{
    /// <summary>
    /// Centralized project information and branding
    /// Change PROJECT_NAME to rebrand the entire application
    /// </summary>
    public static class ProjectInfo
    {
        // ========================================
        // MAIN PROJECT CONFIGURATION
        // Change these values to rebrand
        // ========================================
        
        public const string PROJECT_NAME = "Bloodstrap";
        public const string PROJECT_OWNER = "ethantheDeveloper220";
        public const string PROJECT_REPO = "vibetrap";
        
        // ========================================
        // Auto-generated values (do not edit)
        // ========================================
        
        public static string FullRepository => $"{PROJECT_OWNER}/{PROJECT_REPO}";
        public static string DownloadLink => $"https://github.com/{FullRepository}/releases";
        public static string HelpLink => $"https://github.com/{FullRepository}/wiki";
        public static string SupportLink => $"https://github.com/{FullRepository}/issues/new";
        public static string DiscordLink => "https://discord.gg/bsR6EbZmvX";
        
        // Window titles
        public static string WindowTitle => PROJECT_NAME;
        public static string InstallerTitle => $"{PROJECT_NAME} Installer";
        public static string ConfigBackupTitle => $"{PROJECT_NAME} Configuration Backup & Restore";
        
        // Registry and file paths
        public static string UninstallKey => $@"Software\Microsoft\Windows\CurrentVersion\Uninstall\{PROJECT_NAME}";
        public static string RegistryKey => $@"Software\{PROJECT_NAME}";
        public static string AppDataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PROJECT_NAME);
    }
}
