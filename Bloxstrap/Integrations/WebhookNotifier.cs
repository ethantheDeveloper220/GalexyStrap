using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Voidstrap.UI.Elements.Dialogs;

namespace Voidstrap.Integrations
{
    /// <summary>
    /// Helper class for sending webhook notifications throughout the application
    /// </summary>
    public static class WebhookNotifier
    {
        /// <summary>
        /// Send a notification to all enabled webhooks
        /// </summary>
        public static async Task SendNotification(string message, string? title = null, string? description = null)
        {
            try
            {
                var config = App.Settings.Prop.WebhookConfig;
                if (config == null)
                    return;

                // Build embed if title/description provided
                object payload;
                if (!string.IsNullOrWhiteSpace(title) || !string.IsNullOrWhiteSpace(description))
                {
                    payload = new
                    {
                        content = message,
                        embeds = new[]
                        {
                            new
                            {
                                title = title ?? "",
                                description = description ?? "",
                                color = 0x981bfe, // Purple color
                                timestamp = DateTime.UtcNow.ToString("o")
                            }
                        }
                    };
                }
                else
                {
                    payload = new
                    {
                        content = message
                    };
                }

                var tasks = new System.Collections.Generic.List<Task>();

                if (config.Webhook1Enabled && !string.IsNullOrWhiteSpace(config.Webhook1Url))
                {
                    tasks.Add(SendToWebhook(config.Webhook1Url, config.Webhook1Name, payload));
                }

                if (config.Webhook2Enabled && !string.IsNullOrWhiteSpace(config.Webhook2Url))
                {
                    tasks.Add(SendToWebhook(config.Webhook2Url, config.Webhook2Name, payload));
                }

                if (config.Webhook3Enabled && !string.IsNullOrWhiteSpace(config.Webhook3Url))
                {
                    tasks.Add(SendToWebhook(config.Webhook3Url, config.Webhook3Name, payload));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("WebhookNotifier", $"Error sending notification: {ex.Message}");
            }
        }

        /// <summary>
        /// Send game launch notification
        /// </summary>
        public static async Task NotifyGameLaunch(string gameName, long placeId)
        {
            if (!App.Settings.Prop.WebhookOnGameLaunch)
                return;

            await SendNotification(
                $"🎮 **Game Launched**",
                gameName,
                $"Place ID: {placeId}\nLaunched at: {DateTime.Now:HH:mm:ss}"
            );
        }

        /// <summary>
        /// Send game close notification
        /// </summary>
        public static async Task NotifyGameClose(string gameName, TimeSpan playTime)
        {
            if (!App.Settings.Prop.WebhookOnGameClose)
                return;

            await SendNotification(
                $"🛑 **Game Closed**",
                gameName,
                $"Play time: {playTime:hh\\:mm\\:ss}\nClosed at: {DateTime.Now:HH:mm:ss}"
            );
        }

        /// <summary>
        /// Send Roblox update notification
        /// </summary>
        public static async Task NotifyRobloxUpdate(string version)
        {
            if (!App.Settings.Prop.WebhookOnUpdate)
                return;

            await SendNotification(
                $"🔄 **Roblox Updated**",
                "New Version Available",
                $"Version: {version}\nUpdated at: {DateTime.Now:HH:mm:ss}"
            );
        }

        /// <summary>
        /// Send error notification
        /// </summary>
        public static async Task NotifyError(string errorMessage, string? details = null)
        {
            if (!App.Settings.Prop.WebhookOnError)
                return;

            await SendNotification(
                $"⚠️ **Error Occurred**",
                errorMessage,
                details ?? $"Time: {DateTime.Now:HH:mm:ss}"
            );
        }

        /// <summary>
        /// Send custom notification
        /// </summary>
        public static async Task NotifyCustom(string emoji, string title, string description)
        {
            await SendNotification(
                $"{emoji} **{title}**",
                title,
                description
            );
        }

        private static async Task SendToWebhook(string webhookUrl, string webhookName, object payload)
        {
            try
            {
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    await client.PostAsync(webhookUrl, content);
                }

                App.Logger.WriteLine("WebhookNotifier", $"Notification sent to {webhookName}");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("WebhookNotifier", $"Failed to send to {webhookName}: {ex.Message}");
            }
        }
    }
}
