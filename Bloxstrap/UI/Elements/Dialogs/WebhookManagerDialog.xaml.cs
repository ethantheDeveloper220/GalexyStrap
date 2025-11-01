using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;
using Newtonsoft.Json;

namespace Voidstrap.UI.Elements.Dialogs
{
    /// <summary>
    /// Interaction logic for WebhookManagerDialog.xaml
    /// </summary>
    public partial class WebhookManagerDialog : UiWindow
    {
        private readonly List<string> _activityLog = new List<string>();
        private WebhookConfig _config;

        public WebhookManagerDialog()
        {
            InitializeComponent();
            LoadConfiguration();
            LogActivity("Webhook Manager initialized");
        }

        private void LoadConfiguration()
        {
            try
            {
                // Load saved webhook configuration
                _config = App.Settings.Prop.WebhookConfig ?? new WebhookConfig();

                // Populate UI with saved data
                Webhook1UrlBox.Text = _config.Webhook1Url ?? "";
                Webhook1NameBox.Text = _config.Webhook1Name ?? "Webhook #1";
                Webhook1EnabledCheck.IsChecked = _config.Webhook1Enabled;

                Webhook2UrlBox.Text = _config.Webhook2Url ?? "";
                Webhook2NameBox.Text = _config.Webhook2Name ?? "Webhook #2";
                Webhook2EnabledCheck.IsChecked = _config.Webhook2Enabled;

                Webhook3UrlBox.Text = _config.Webhook3Url ?? "";
                Webhook3NameBox.Text = _config.Webhook3Name ?? "Webhook #3";
                Webhook3EnabledCheck.IsChecked = _config.Webhook3Enabled;

                // Load license info
                LicenseKeyBox.Text = _config.LicenseKey ?? "";
                UpdateLicenseStatus();

                LogActivity("Configuration loaded successfully");
            }
            catch (Exception ex)
            {
                LogActivity($"Error loading configuration: {ex.Message}", true);
            }
        }

        private void SaveConfiguration()
        {
            try
            {
                _config.Webhook1Url = Webhook1UrlBox.Text;
                _config.Webhook1Name = Webhook1NameBox.Text;
                _config.Webhook1Enabled = Webhook1EnabledCheck.IsChecked ?? false;

                _config.Webhook2Url = Webhook2UrlBox.Text;
                _config.Webhook2Name = Webhook2NameBox.Text;
                _config.Webhook2Enabled = Webhook2EnabledCheck.IsChecked ?? false;

                _config.Webhook3Url = Webhook3UrlBox.Text;
                _config.Webhook3Name = Webhook3NameBox.Text;
                _config.Webhook3Enabled = Webhook3EnabledCheck.IsChecked ?? false;

                _config.LicenseKey = LicenseKeyBox.Text;

                App.Settings.Prop.WebhookConfig = _config;
                App.Settings.Save();

                LogActivity("Configuration saved successfully");
                
                NotificationSnackbar.Title = "Saved";
                NotificationSnackbar.Message = "Webhook configuration saved successfully";
                NotificationSnackbar.Appearance = Wpf.Ui.Common.ControlAppearance.Success;
                NotificationSnackbar.Icon = Wpf.Ui.Common.SymbolRegular.Checkmark24;
                NotificationSnackbar.Show();
            }
            catch (Exception ex)
            {
                LogActivity($"Error saving configuration: {ex.Message}", true);
                
                NotificationSnackbar.Title = "Error";
                NotificationSnackbar.Message = "Failed to save configuration";
                NotificationSnackbar.Appearance = Wpf.Ui.Common.ControlAppearance.Danger;
                NotificationSnackbar.Icon = Wpf.Ui.Common.SymbolRegular.ErrorCircle24;
                NotificationSnackbar.Show();
            }
        }

        private async Task<bool> SendWebhookMessage(string webhookUrl, string webhookName, string message, bool isTest = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(webhookUrl))
                {
                    LogActivity($"[{webhookName}] No webhook URL provided", true);
                    return false;
                }

                if (!webhookUrl.StartsWith("https://discord.com/api/webhooks/") && 
                    !webhookUrl.StartsWith("https://discordapp.com/api/webhooks/"))
                {
                    LogActivity($"[{webhookName}] Invalid webhook URL format", true);
                    return false;
                }

                var payload = new
                {
                    content = message,
                    username = $"{ProjectInfo.PROJECT_NAME} - {webhookName}",
                    avatar_url = "https://i.imgur.com/AfFp7pu.png" // Optional: Add your bot avatar URL
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    var response = await client.PostAsync(webhookUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        LogActivity($"[{webhookName}] Message sent successfully: {message}");
                        return true;
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        LogActivity($"[{webhookName}] Failed to send message. Status: {response.StatusCode}, Error: {error}", true);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogActivity($"[{webhookName}] Exception: {ex.Message}", true);
                return false;
            }
        }

        private void LogActivity(string message, bool isError = false)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var logEntry = $"[{timestamp}] {message}";
            
            _activityLog.Add(logEntry);

            // Keep only last 100 entries
            if (_activityLog.Count > 100)
            {
                _activityLog.RemoveAt(0);
            }

            // Update UI
            Dispatcher.Invoke(() =>
            {
                ActivityLogText.Text = string.Join("\n", _activityLog);
            });
        }

        private void UpdateLicenseStatus()
        {
            if (string.IsNullOrWhiteSpace(_config.LicenseKey))
            {
                LicenseTypeText.Text = "Free";
                FeaturesCountText.Text = "3 Webhooks";
                ExpiryDateText.Text = "Never";
                LicenseStatusText.Text = "";
            }
            else if (_config.IsLicenseValid)
            {
                LicenseTypeText.Text = "Pro";
                FeaturesCountText.Text = "Unlimited Webhooks (Coming Soon)";
                ExpiryDateText.Text = _config.LicenseExpiry?.ToString("yyyy-MM-dd") ?? "Lifetime";
                LicenseStatusText.Text = "✓ License Active";
                LicenseStatusText.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                LicenseTypeText.Text = "Free";
                FeaturesCountText.Text = "3 Webhooks";
                ExpiryDateText.Text = "Never";
                LicenseStatusText.Text = "✗ Invalid License";
                LicenseStatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        // Event Handlers
        private async void TestWebhook1_Click(object sender, RoutedEventArgs e)
        {
            if (Webhook1EnabledCheck.IsChecked == true)
            {
                await SendWebhookMessage(
                    Webhook1UrlBox.Text, 
                    Webhook1NameBox.Text, 
                    $"🧪 Test message from {ProjectInfo.PROJECT_NAME} Webhook Manager", 
                    true
                );
            }
            else
            {
                LogActivity($"[{Webhook1NameBox.Text}] Webhook is disabled", true);
            }
        }

        private async void TestWebhook2_Click(object sender, RoutedEventArgs e)
        {
            if (Webhook2EnabledCheck.IsChecked == true)
            {
                await SendWebhookMessage(
                    Webhook2UrlBox.Text, 
                    Webhook2NameBox.Text, 
                    $"🧪 Test message from {ProjectInfo.PROJECT_NAME} Webhook Manager", 
                    true
                );
            }
            else
            {
                LogActivity($"[{Webhook2NameBox.Text}] Webhook is disabled", true);
            }
        }

        private async void TestWebhook3_Click(object sender, RoutedEventArgs e)
        {
            if (Webhook3EnabledCheck.IsChecked == true)
            {
                await SendWebhookMessage(
                    Webhook3UrlBox.Text, 
                    Webhook3NameBox.Text, 
                    $"🧪 Test message from {ProjectInfo.PROJECT_NAME} Webhook Manager", 
                    true
                );
            }
            else
            {
                LogActivity($"[{Webhook3NameBox.Text}] Webhook is disabled", true);
            }
        }

        private async void TestAllButton_Click(object sender, RoutedEventArgs e)
        {
            LogActivity("Testing all enabled webhooks...");

            var tasks = new List<Task>();

            if (Webhook1EnabledCheck.IsChecked == true && !string.IsNullOrWhiteSpace(Webhook1UrlBox.Text))
            {
                tasks.Add(SendWebhookMessage(Webhook1UrlBox.Text, Webhook1NameBox.Text, 
                    $"🧪 Test message from {ProjectInfo.PROJECT_NAME} Webhook Manager", true));
            }

            if (Webhook2EnabledCheck.IsChecked == true && !string.IsNullOrWhiteSpace(Webhook2UrlBox.Text))
            {
                tasks.Add(SendWebhookMessage(Webhook2UrlBox.Text, Webhook2NameBox.Text, 
                    $"🧪 Test message from {ProjectInfo.PROJECT_NAME} Webhook Manager", true));
            }

            if (Webhook3EnabledCheck.IsChecked == true && !string.IsNullOrWhiteSpace(Webhook3UrlBox.Text))
            {
                tasks.Add(SendWebhookMessage(Webhook3UrlBox.Text, Webhook3NameBox.Text, 
                    $"🧪 Test message from {ProjectInfo.PROJECT_NAME} Webhook Manager", true));
            }

            if (tasks.Count == 0)
            {
                LogActivity("No enabled webhooks to test", true);
                return;
            }

            await Task.WhenAll(tasks);
            LogActivity($"Finished testing {tasks.Count} webhook(s)");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveConfiguration();
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            _activityLog.Clear();
            ActivityLogText.Text = "Log cleared...";
            LogActivity("Log cleared by user");
        }

        private void ActivateLicense_Click(object sender, RoutedEventArgs e)
        {
            var licenseKey = LicenseKeyBox.Text?.Trim();

            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                LicenseStatusText.Text = "✗ Please enter a license key";
                LicenseStatusText.Foreground = System.Windows.Media.Brushes.Red;
                LogActivity("License activation failed: No key provided", true);
                return;
            }

            // Validate license key (placeholder logic - implement your own validation)
            bool isValid = ValidateLicenseKey(licenseKey);

            if (isValid)
            {
                _config.LicenseKey = licenseKey;
                _config.IsLicenseValid = true;
                _config.LicenseExpiry = DateTime.Now.AddYears(1); // Example: 1 year validity
                
                SaveConfiguration();
                UpdateLicenseStatus();
                
                LogActivity($"License activated successfully: {licenseKey}");
                
                NotificationSnackbar.Title = "Success";
                NotificationSnackbar.Message = "Pro license activated successfully!";
                NotificationSnackbar.Appearance = Wpf.Ui.Common.ControlAppearance.Success;
                NotificationSnackbar.Icon = Wpf.Ui.Common.SymbolRegular.Checkmark24;
                NotificationSnackbar.Show();
            }
            else
            {
                LicenseStatusText.Text = "✗ Invalid license key";
                LicenseStatusText.Foreground = System.Windows.Media.Brushes.Red;
                LogActivity($"License activation failed: Invalid key", true);
                
                NotificationSnackbar.Title = "Error";
                NotificationSnackbar.Message = "Invalid license key. Please check and try again.";
                NotificationSnackbar.Appearance = Wpf.Ui.Common.ControlAppearance.Danger;
                NotificationSnackbar.Icon = Wpf.Ui.Common.SymbolRegular.ErrorCircle24;
                NotificationSnackbar.Show();
            }
        }

        private bool ValidateLicenseKey(string key)
        {
            // TODO: Implement your license validation logic here
            // This is a placeholder that accepts any key in the format XXXX-XXXX-XXXX-XXXX
            
            // Example validation:
            // - Check format
            // - Verify with server
            // - Check expiration date
            // - Validate signature/hash
            
            if (string.IsNullOrWhiteSpace(key))
                return false;

            // Simple format check (4 groups of 4 characters separated by dashes)
            var parts = key.Split('-');
            if (parts.Length != 4)
                return false;

            foreach (var part in parts)
            {
                if (part.Length != 4)
                    return false;
            }

            // For now, accept any properly formatted key
            // In production, you would validate against a database or licensing server
            return true;
        }

        /// <summary>
        /// Public method to send webhook messages from other parts of the application
        /// </summary>
        public static async Task SendWebhookNotification(string message)
        {
            try
            {
                var config = App.Settings.Prop.WebhookConfig;
                if (config == null)
                    return;

                var tasks = new List<Task>();

                if (config.Webhook1Enabled && !string.IsNullOrWhiteSpace(config.Webhook1Url))
                {
                    tasks.Add(SendWebhookMessageStatic(config.Webhook1Url, config.Webhook1Name, message));
                }

                if (config.Webhook2Enabled && !string.IsNullOrWhiteSpace(config.Webhook2Url))
                {
                    tasks.Add(SendWebhookMessageStatic(config.Webhook2Url, config.Webhook2Name, message));
                }

                if (config.Webhook3Enabled && !string.IsNullOrWhiteSpace(config.Webhook3Url))
                {
                    tasks.Add(SendWebhookMessageStatic(config.Webhook3Url, config.Webhook3Name, message));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("WebhookManager", $"Error sending webhook notification: {ex.Message}");
            }
        }

        private static async Task SendWebhookMessageStatic(string webhookUrl, string webhookName, string message)
        {
            try
            {
                var payload = new
                {
                    content = message,
                    username = $"{ProjectInfo.PROJECT_NAME} - {webhookName}",
                    avatar_url = "https://i.imgur.com/AfFp7pu.png"
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    await client.PostAsync(webhookUrl, content);
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("WebhookManager", $"Error sending webhook: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Webhook configuration model
    /// </summary>
    public class WebhookConfig
    {
        public string? Webhook1Url { get; set; }
        public string? Webhook1Name { get; set; } = "Webhook #1";
        public bool Webhook1Enabled { get; set; } = false;

        public string? Webhook2Url { get; set; }
        public string? Webhook2Name { get; set; } = "Webhook #2";
        public bool Webhook2Enabled { get; set; } = false;

        public string? Webhook3Url { get; set; }
        public string? Webhook3Name { get; set; } = "Webhook #3";
        public bool Webhook3Enabled { get; set; } = false;

        public string? LicenseKey { get; set; }
        public bool IsLicenseValid { get; set; } = false;
        public DateTime? LicenseExpiry { get; set; }
    }
}
