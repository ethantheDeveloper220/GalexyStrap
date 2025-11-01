using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using Voidstrap.Helpers;

namespace Voidstrap.UI.ViewModels.Settings
{
    public class WebhooksViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string WebhookUrl
        {
            get => App.Settings.Prop.WebhookUrl;
            set
            {
                App.Settings.Prop.WebhookUrl = value;
                OnPropertyChanged();
            }
        }

        public string WebhookUsername
        {
            get => App.Settings.Prop.WebhookUsername;
            set
            {
                App.Settings.Prop.WebhookUsername = value;
                OnPropertyChanged();
            }
        }

        public bool WebhookOnGameLaunch
        {
            get => App.Settings.Prop.WebhookOnGameLaunch;
            set
            {
                App.Settings.Prop.WebhookOnGameLaunch = value;
                OnPropertyChanged();
            }
        }

        public bool WebhookOnGameClose
        {
            get => App.Settings.Prop.WebhookOnGameClose;
            set
            {
                App.Settings.Prop.WebhookOnGameClose = value;
                OnPropertyChanged();
            }
        }

        public bool WebhookOnUpdate
        {
            get => App.Settings.Prop.WebhookOnUpdate;
            set
            {
                App.Settings.Prop.WebhookOnUpdate = value;
                OnPropertyChanged();
            }
        }

        public bool WebhookOnError
        {
            get => App.Settings.Prop.WebhookOnError;
            set
            {
                App.Settings.Prop.WebhookOnError = value;
                OnPropertyChanged();
            }
        }

        public async Task SendTestWebhook()
        {
            if (string.IsNullOrWhiteSpace(WebhookUrl))
            {
                MessageBox.Show("Please enter a webhook URL first.", "Webhook Test", MessageBoxButton.OK, MessageBoxImage.Warning);
                ActivityLogger.LogWarning("Webhook test failed: No URL configured");
                return;
            }

            try
            {
                ActivityLogger.Log("Sending test webhook...", LogLevel.Info);
                await WebhookHelper.SendWebhook(WebhookUrl, WebhookUsername, "Test Message", "This is a test message from Bloodstrap!");
                MessageBox.Show("Test webhook sent successfully!", "Webhook Test", MessageBoxButton.OK, MessageBoxImage.Information);
                ActivityLogger.LogSuccess("Test webhook sent successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send webhook: {ex.Message}", "Webhook Test", MessageBoxButton.OK, MessageBoxImage.Error);
                ActivityLogger.LogError($"Webhook test failed: {ex.Message}");
            }
        }
    }
}
