using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Wpf.Ui.Controls;

namespace Voidstrap.UI.Elements.Dialogs
{
    public partial class EnhancedPresetManagerDialog
    {
        private readonly string _presetsDirectory;
        private const string LOG_IDENT = "EnhancedPresetManagerDialog";

        public EnhancedPresetManagerDialog()
        {
            try
            {
                InitializeComponent();
                _presetsDirectory = Path.Combine(Paths.Base, "CustomPresets");
                Directory.CreateDirectory(_presetsDirectory);
                LoadPresetsList();
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Failed to initialize dialog: {ex.Message}");
                Frontend.ShowMessageBox($"Failed to initialize Enhanced Preset Manager: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void LoadPresetsList()
        {
            try
            {
                LoadPresetList.Items.Clear();
                
                if (!Directory.Exists(_presetsDirectory))
                    return;

                var presetFiles = Directory.GetFiles(_presetsDirectory, "*.json");
                
                foreach (var file in presetFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var fileInfo = new FileInfo(file);
                    
                    var item = new ListBoxItem
                    {
                        Content = $"{fileName} ({fileInfo.LastWriteTime:yyyy-MM-dd HH:mm})",
                        Tag = file
                    };
                    
                    LoadPresetList.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Failed to load presets list: {ex.Message}");
                Frontend.ShowMessageBox($"Failed to load presets list: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        // Save Preset Tab
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Tabs.SelectedIndex == 0) // Save tab
                {
                    SavePreset();
                }
                else if (Tabs.SelectedIndex == 1) // Load tab
                {
                    LoadPreset();
                }
                else if (Tabs.SelectedIndex == 4) // Preset Library tab
                {
                    ApplyLibraryPreset_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Error in ApplyButton_Click: {ex.Message}");
                Frontend.ShowMessageBox($"Error: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void SavePreset()
        {
            var presetName = SavePresetName.Text.Trim();
            
            if (string.IsNullOrEmpty(presetName))
            {
                Frontend.ShowMessageBox("Please enter a preset name.", MessageBoxImage.Warning, MessageBoxButton.OK);
                return;
            }

            // Sanitize filename
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                presetName = presetName.Replace(c, '_');
            }

            if (SaveWithTimestamp.IsChecked == true)
            {
                presetName += $"_{DateTime.Now:yyyyMMdd_HHmmss}";
            }

            var filePath = Path.Combine(_presetsDirectory, $"{presetName}.json");

            try
            {
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = SaveCompressed.IsChecked != true 
                };
                
                var json = JsonSerializer.Serialize(App.FastFlags.Prop, options);
                File.WriteAllText(filePath, json);

                App.Logger.WriteLine(LOG_IDENT, $"Saved preset: {presetName}");
                Frontend.ShowMessageBox($"Preset '{presetName}' saved successfully!", MessageBoxImage.Information, MessageBoxButton.OK);
                
                LoadPresetsList();
                Close();
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Failed to save preset: {ex.Message}");
                Frontend.ShowMessageBox($"Failed to save preset: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        // Load Preset Tab
        private void LoadPresetList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable/disable buttons based on selection
        }

        private void LoadPreset()
        {
            if (LoadPresetList.SelectedItem is not ListBoxItem selectedItem)
            {
                Frontend.ShowMessageBox("Please select a preset to load.", MessageBoxImage.Warning, MessageBoxButton.OK);
                return;
            }

            var filePath = selectedItem.Tag as string;
            
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Frontend.ShowMessageBox("Preset file not found.", MessageBoxImage.Error, MessageBoxButton.OK);
                return;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                var flags = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                if (flags == null)
                {
                    Frontend.ShowMessageBox("Invalid preset file.", MessageBoxImage.Error, MessageBoxButton.OK);
                    return;
                }

                if (MergePreset.IsChecked == true)
                {
                    // Merge with existing flags
                    if (App.FastFlags.Prop is IDictionary<string, object> propDict)
                    {
                        foreach (var kvp in flags)
                        {
                            propDict[kvp.Key] = kvp.Value;
                        }
                    }
                }
                else
                {
                    // Replace all flags
                    App.FastFlags.Prop = flags;
                }

                App.FastFlags.Save();

                App.Logger.WriteLine(LOG_IDENT, $"Loaded preset from: {filePath}");
                Frontend.ShowMessageBox("Preset loaded successfully!", MessageBoxImage.Information, MessageBoxButton.OK);
                Close();
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Failed to load preset: {ex.Message}");
                Frontend.ShowMessageBox($"Failed to load preset: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void BrowsePreset_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Select Preset File",
                InitialDirectory = _presetsDirectory
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var json = File.ReadAllText(dialog.FileName);
                    var flags = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                    if (flags != null)
                    {
                        LoadPresetList.Items.Clear();
                        var item = new ListBoxItem
                        {
                            Content = Path.GetFileName(dialog.FileName),
                            Tag = dialog.FileName
                        };
                        LoadPresetList.Items.Add(item);
                        LoadPresetList.SelectedIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    Frontend.ShowMessageBox($"Failed to load file: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
                }
            }
        }

        private void RefreshPresets_Click(object sender, RoutedEventArgs e)
        {
            LoadPresetsList();
            Frontend.ShowMessageBox("Preset list refreshed!", MessageBoxImage.Information, MessageBoxButton.OK);
        }

        // Import/Export Tab
        private async void ImportFromUrl_Click(object sender, RoutedEventArgs e)
        {
            var url = ImportUrl.Text.Trim();
            
            if (string.IsNullOrEmpty(url))
            {
                Frontend.ShowMessageBox("Please enter a URL.", MessageBoxImage.Warning, MessageBoxButton.OK);
                return;
            }

            try
            {
                using var client = new HttpClient();
                var json = await client.GetStringAsync(url);
                var flags = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                if (flags == null)
                {
                    Frontend.ShowMessageBox("Invalid JSON from URL.", MessageBoxImage.Error, MessageBoxButton.OK);
                    return;
                }

                App.FastFlags.Prop = flags;
                App.FastFlags.Save();

                App.Logger.WriteLine(LOG_IDENT, $"Imported preset from URL: {url}");
                Frontend.ShowMessageBox("Preset imported successfully from URL!", MessageBoxImage.Information, MessageBoxButton.OK);
                Close();
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Failed to import from URL: {ex.Message}");
                Frontend.ShowMessageBox($"Failed to import from URL: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void ExportToFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                Title = "Export Preset",
                FileName = $"Preset_{DateTime.Now:yyyyMMdd_HHmmss}.json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var json = JsonSerializer.Serialize(App.FastFlags.Prop, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(dialog.FileName, json);
                    
                    Frontend.ShowMessageBox($"Preset exported to:\n{dialog.FileName}", MessageBoxImage.Information, MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    Frontend.ShowMessageBox($"Failed to export: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
                }
            }
        }

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var json = JsonSerializer.Serialize(App.FastFlags.Prop, new JsonSerializerOptions { WriteIndented = true });
                Clipboard.SetText(json);
                Frontend.ShowMessageBox("Preset copied to clipboard!", MessageBoxImage.Information, MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                Frontend.ShowMessageBox($"Failed to copy: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void GenerateShareLink_Click(object sender, RoutedEventArgs e)
        {
            Frontend.ShowMessageBox("Share link generation coming soon!\nFor now, use 'Export to File' or 'Copy to Clipboard'.", MessageBoxImage.Information, MessageBoxButton.OK);
        }

        // Bulk Operations Tab
        private void ApplyBulkReplace_Click(object sender, RoutedEventArgs e)
        {
            var findText = BulkFindText.Text;
            var replaceText = BulkReplaceText.Text;

            if (string.IsNullOrEmpty(findText))
            {
                Frontend.ShowMessageBox("Please enter text to find.", MessageBoxImage.Warning, MessageBoxButton.OK);
                return;
            }

            try
            {
                var comparison = BulkCaseSensitive.IsChecked == true 
                    ? StringComparison.Ordinal 
                    : StringComparison.OrdinalIgnoreCase;

                int count = 0;
                
                if (App.FastFlags.Prop is IDictionary<string, object> flags)
                {
                    var updates = new Dictionary<string, object>();
                    
                    foreach (var kvp in flags.ToList())
                    {
                        var newKey = kvp.Key.Replace(findText, replaceText, comparison);
                        var newValue = kvp.Value?.ToString()?.Replace(findText, replaceText, comparison) ?? kvp.Value;
                        
                        if (newKey != kvp.Key || newValue?.ToString() != kvp.Value?.ToString())
                        {
                            updates[newKey] = newValue;
                            if (newKey != kvp.Key)
                                flags.Remove(kvp.Key);
                            count++;
                        }
                    }
                    
                    foreach (var kvp in updates)
                    {
                        flags[kvp.Key] = kvp.Value;
                    }
                }

                App.FastFlags.Save();
                Frontend.ShowMessageBox($"Replaced {count} occurrences.", MessageBoxImage.Information, MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                Frontend.ShowMessageBox($"Bulk replace failed: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void EnableAllFlags_Click(object sender, RoutedEventArgs e)
        {
            if (App.FastFlags.Prop is IDictionary<string, object> flags)
            {
                foreach (var key in flags.Keys.ToList())
                {
                    flags[key] = "True";
                }
                App.FastFlags.Save();
                Frontend.ShowMessageBox("All flags enabled!", MessageBoxImage.Information, MessageBoxButton.OK);
            }
        }

        private void DisableAllFlags_Click(object sender, RoutedEventArgs e)
        {
            if (App.FastFlags.Prop is IDictionary<string, object> flags)
            {
                foreach (var key in flags.Keys.ToList())
                {
                    flags[key] = "False";
                }
                App.FastFlags.Save();
                Frontend.ShowMessageBox("All flags disabled!", MessageBoxImage.Information, MessageBoxButton.OK);
            }
        }

        private void ResetToDefaults_Click(object sender, RoutedEventArgs e)
        {
            var result = Frontend.ShowMessageBox(
                "This will reset all FastFlags to defaults. Continue?",
                MessageBoxImage.Warning,
                MessageBoxButton.YesNo
            );

            if (result == MessageBoxResult.Yes)
            {
                App.FastFlags.Prop = new Dictionary<string, object>();
                App.FastFlags.Save();
                Frontend.ShowMessageBox("FastFlags reset to defaults!", MessageBoxImage.Information, MessageBoxButton.OK);
                Close();
            }
        }

        // Preset Library Tab
        private void ApplyLibraryPreset_Click(object sender, RoutedEventArgs e)
        {
            if (PresetLibraryList.SelectedItem is not ListBoxItem selectedItem)
            {
                Frontend.ShowMessageBox("Please select a preset from the library.", MessageBoxImage.Warning, MessageBoxButton.OK);
                return;
            }

            var presetName = selectedItem.Content.ToString()?.Split(' ')[1] ?? "";
            
            try
            {
                // Load preset from Resources/PresetFlags directory
                var presetPath = Path.Combine(Paths.Base, "Resources", "PresetFlags", presetName);
                if (File.Exists(presetPath))
                {
                    var json = File.ReadAllText(presetPath);
                    var flags = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    if (flags != null)
                    {
                        App.FastFlags.Prop = flags;
                        App.FastFlags.Save();
                        Frontend.ShowMessageBox($"Applied {presetName} preset!", MessageBoxImage.Information, MessageBoxButton.OK);
                        Close();
                    }
                }
                else
                {
                    Frontend.ShowMessageBox($"Preset file not found: {presetName}", MessageBoxImage.Error, MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                Frontend.ShowMessageBox($"Failed to apply preset: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        // Advanced Tools Tab
        private void ValidateFlags_Click(object sender, RoutedEventArgs e)
        {
            Frontend.ShowMessageBox("Flag validation complete!\n(Validation logic to be implemented)", MessageBoxImage.Information, MessageBoxButton.OK);
        }

        private void RemoveDuplicates_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.FastFlags.Prop is IDictionary<string, object> flags)
                {
                    var originalCount = flags.Count;
                    // Duplicates are automatically handled by dictionary keys
                    Frontend.ShowMessageBox($"No duplicates found. Total flags: {originalCount}", MessageBoxImage.Information, MessageBoxButton.OK);
                }
                else
                {
                    Frontend.ShowMessageBox("No flags available.", MessageBoxImage.Warning, MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Failed to check duplicates: {ex.Message}");
                Frontend.ShowMessageBox($"Failed to check duplicates: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void SortAlphabetically_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.FastFlags.Prop is IDictionary<string, object> flags)
                {
                    var sorted = flags.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
                    App.FastFlags.Prop = sorted;
                    App.FastFlags.Save();
                    Frontend.ShowMessageBox("Flags sorted alphabetically!", MessageBoxImage.Information, MessageBoxButton.OK);
                }
                else
                {
                    Frontend.ShowMessageBox("No flags to sort.", MessageBoxImage.Warning, MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Failed to sort flags: {ex.Message}");
                Frontend.ShowMessageBox($"Failed to sort flags: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void RemoveInvalid_Click(object sender, RoutedEventArgs e)
        {
            Frontend.ShowMessageBox("Invalid flag removal complete!\n(Validation logic to be implemented)", MessageBoxImage.Information, MessageBoxButton.OK);
        }

        private void ComparePresets_Click(object sender, RoutedEventArgs e)
        {
            Frontend.ShowMessageBox("Preset comparison tool coming soon!", MessageBoxImage.Information, MessageBoxButton.OK);
        }

        private void ShowStatistics_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.FastFlags.Prop is IDictionary<string, object> flags)
                {
                    var stats = $"Total Flags: {flags.Count}\n" +
                               $"Enabled (True): {flags.Count(x => x.Value?.ToString()?.ToLower() == "true")}\n" +
                               $"Disabled (False): {flags.Count(x => x.Value?.ToString()?.ToLower() == "false")}\n" +
                               $"Numeric Values: {flags.Count(x => int.TryParse(x.Value?.ToString(), out _))}";
                    
                    Frontend.ShowMessageBox(stats, MessageBoxImage.Information, MessageBoxButton.OK);
                }
                else
                {
                    Frontend.ShowMessageBox("No flags available to analyze.", MessageBoxImage.Warning, MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Failed to show statistics: {ex.Message}");
                Frontend.ShowMessageBox($"Failed to show statistics: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void BackupCurrent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var backupName = $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}";
                var filePath = Path.Combine(_presetsDirectory, $"{backupName}.json");
                
                var json = JsonSerializer.Serialize(App.FastFlags.Prop, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                
                Frontend.ShowMessageBox($"Backup created: {backupName}", MessageBoxImage.Information, MessageBoxButton.OK);
                LoadPresetsList();
            }
            catch (Exception ex)
            {
                Frontend.ShowMessageBox($"Backup failed: {ex.Message}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }
    }
}
