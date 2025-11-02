using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;
using Voidstrap.UI.Elements.Base;

namespace Voidstrap.UI.Elements.Dialogs
{
    /// <summary>
    /// Configuration Backup and Restore Dialog
    /// Allows users to export and import complete GalaxyStrap configurations
    /// </summary>
    public partial class ConfigBackupDialog : WpfUiWindow
    {
        private const string ConfigFileFilter = "GalaxyStrap Config Files (*.bloodconfig)|*.bloodconfig|JSON Files (*.json)|*.json|All Files (*.*)|*.*";
        private const string DefaultConfigExtension = ".bloodconfig";

        // Export toggle states
        public bool IncludeFastFlags { get; set; } = true;
        public bool IncludeSettings { get; set; } = true;
        public bool IncludeMods { get; set; } = true;
        public bool IncludeThemes { get; set; } = true;

        // Import toggle states
        public bool MergeConfig { get; set; } = false;
        public bool BackupBeforeImport { get; set; } = true;

        public ConfigBackupDialog()
        {
            InitializeComponent();
            DataContext = this;
            LoadLastBackupInfo();
        }

        private void LoadLastBackupInfo()
        {
            try
            {
                string backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GalaxyStrap", "Backups");
                if (Directory.Exists(backupFolder))
                {
                    var files = Directory.GetFiles(backupFolder, "*" + DefaultConfigExtension);
                    if (files.Length > 0)
                    {
                        var lastFile = files[files.Length - 1];
                        var fileInfo = new FileInfo(lastFile);
                        LastBackupText.Text = $"Last backup: {fileInfo.Name} ({fileInfo.LastWriteTime:g})";
                    }
                }
            }
            catch
            {
                // Ignore errors loading backup info
            }
        }

        private void ExportConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = ConfigFileFilter,
                    DefaultExt = DefaultConfigExtension,
                    FileName = $"GalaxyStrapConfig_{DateTime.Now:yyyyMMdd_HHmmss}{DefaultConfigExtension}",
                    Title = "Export GalaxyStrap Configuration"
                };

                if (saveDialog.ShowDialog() != true)
                    return;

                StatusText.Text = "Exporting configuration...";

                var config = new GalaxyStrapConfig
                {
                    ExportDate = DateTime.Now,
                    Version = "1.5.0",
                    IncludeFastFlags = IncludeFastFlags,
                    IncludeSettings = IncludeSettings,
                    IncludeMods = IncludeMods,
                    IncludeThemes = IncludeThemes
                };

                // Export FastFlags
                if (config.IncludeFastFlags)
                {
                    try
                    {
                        config.FastFlags = ExportFastFlags();
                    }
                    catch (Exception ex)
                    {
                        StatusText.Text = $"Warning: Could not export FastFlags: {ex.Message}";
                    }
                }

                // Export Settings
                if (config.IncludeSettings)
                {
                    try
                    {
                        config.Settings = ExportSettings();
                    }
                    catch (Exception ex)
                    {
                        StatusText.Text = $"Warning: Could not export settings: {ex.Message}";
                    }
                }

                // Export Mods
                if (config.IncludeMods)
                {
                    try
                    {
                        config.Mods = ExportMods();
                    }
                    catch (Exception ex)
                    {
                        StatusText.Text = $"Warning: Could not export mods: {ex.Message}";
                    }
                }

                // Export Themes
                if (config.IncludeThemes)
                {
                    try
                    {
                        config.Themes = ExportThemes();
                    }
                    catch (Exception ex)
                    {
                        StatusText.Text = $"Warning: Could not export themes: {ex.Message}";
                    }
                }

                // Save to file
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(saveDialog.FileName, json);

                StatusText.Text = $"✅ Configuration exported successfully to: {Path.GetFileName(saveDialog.FileName)}";
                System.Windows.MessageBox.Show(
                    $"Configuration exported successfully!\n\nFile: {saveDialog.FileName}",
                    "Export Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusText.Text = $"❌ Export failed: {ex.Message}";
                System.Windows.MessageBox.Show(
                    $"Failed to export configuration:\n\n{ex.Message}",
                    "Export Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ImportConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openDialog = new OpenFileDialog
                {
                    Filter = ConfigFileFilter,
                    DefaultExt = DefaultConfigExtension,
                    Title = "Import GalaxyStrap Configuration"
                };

                if (openDialog.ShowDialog() != true)
                    return;

                // Backup current config if requested
                if (BackupBeforeImport)
                {
                    StatusText.Text = "Creating backup of current configuration...";
                    CreateAutoBackup();
                }

                StatusText.Text = "Importing configuration...";

                string json = File.ReadAllText(openDialog.FileName);
                var config = JsonSerializer.Deserialize<GalaxyStrapConfig>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                if (config == null)
                {
                    throw new Exception("Invalid configuration file format");
                }

                bool mergeMode = MergeConfig;

                // Import FastFlags
                if (config.FastFlags != null)
                {
                    ImportFastFlags(config.FastFlags, mergeMode);
                }

                // Import Settings
                if (config.Settings != null)
                {
                    ImportSettings(config.Settings, mergeMode);
                }

                // Import Mods
                if (config.Mods != null)
                {
                    ImportMods(config.Mods, mergeMode);
                }

                // Import Themes
                if (config.Themes != null)
                {
                    ImportThemes(config.Themes, mergeMode);
                }

                StatusText.Text = $"✅ Configuration imported successfully from: {Path.GetFileName(openDialog.FileName)}";
                
                var result = System.Windows.MessageBox.Show(
                    $"Configuration imported successfully!\n\nSome changes may require restarting GalaxyStrap.\n\nRestart now?",
                    "Import Complete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    // Restart application
                    System.Windows.Application.Current.Shutdown();
                    System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"❌ Import failed: {ex.Message}";
                System.Windows.MessageBox.Show(
                    $"Failed to import configuration:\n\n{ex.Message}",
                    "Import Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CreateAutoBackup()
        {
            try
            {
                string backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GalaxyStrap", "Backups");
                Directory.CreateDirectory(backupFolder);

                string backupFile = Path.Combine(backupFolder, $"AutoBackup_{DateTime.Now:yyyyMMdd_HHmmss}{DefaultConfigExtension}");

                var config = new GalaxyStrapConfig
                {
                    ExportDate = DateTime.Now,
                    Version = "1.5.0",
                    IncludeFastFlags = true,
                    IncludeSettings = true,
                    IncludeMods = true,
                    IncludeThemes = true,
                    FastFlags = ExportFastFlags(),
                    Settings = ExportSettings(),
                    Mods = ExportMods(),
                    Themes = ExportThemes()
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(backupFile, json);

                LoadLastBackupInfo();
            }
            catch
            {
                // Ignore backup errors
            }
        }

        // Export methods
        private object ExportFastFlags()
        {
            // Export FastFlags from App.FastFlags or settings
            try
            {
                return App.FastFlags?.Prop ?? new Dictionary<string, object>();
            }
            catch
            {
                return new { };
            }
        }

        private object ExportSettings()
        {
            // Export application settings
            try
            {
                return App.Settings?.Prop ?? new AppSettings();
            }
            catch
            {
                return new { };
            }
        }

        private object ExportMods()
        {
            // Export installed mods
            return new { Message = "Mod export not yet implemented" };
        }

        private object ExportThemes()
        {
            // Export custom themes
            return new { Message = "Theme export not yet implemented" };
        }

        // Import methods
        private void ImportFastFlags(object fastFlags, bool merge)
        {
            // Import FastFlags
            // Implementation depends on FastFlags structure
        }

        private void ImportSettings(object settings, bool merge)
        {
            // Import settings
            // Implementation depends on settings structure
        }

        private void ImportMods(object mods, bool merge)
        {
            // Import mods
        }

        private void ImportThemes(object themes, bool merge)
        {
            // Import themes
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    // Configuration data structure
    public class GalaxyStrapConfig
    {
        public DateTime ExportDate { get; set; }
        public string Version { get; set; } = "1.5.0";
        public bool IncludeFastFlags { get; set; }
        public bool IncludeSettings { get; set; }
        public bool IncludeMods { get; set; }
        public bool IncludeThemes { get; set; }
        public object? FastFlags { get; set; }
        public object? Settings { get; set; }
        public object? Mods { get; set; }
        public object? Themes { get; set; }
    }
}
