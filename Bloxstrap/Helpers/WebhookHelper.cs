using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Voidstrap.Helpers
{
    public static class WebhookHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task SendWebhook(string webhookUrl, string username, string title, string message, int color = 0x8B0000)
        {
            if (string.IsNullOrWhiteSpace(webhookUrl))
                return;

            try
            {
                var payload = new
                {
                    username = string.IsNullOrWhiteSpace(username) ? "GalaxyStrap" : username,
                    embeds = new[]
                    {
                        new
                        {
                            title = title,
                            description = message,
                            color = color,
                            timestamp = DateTime.UtcNow.ToString("o"),
                            footer = new
                            {
                                text = "GalaxyStrap v1.3.0"
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(webhookUrl, content);
                response.EnsureSuccessStatusCode();

                ActivityLogger.Log($"Webhook sent: {title}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                ActivityLogger.LogError($"Failed to send webhook: {ex.Message}");
            }
        }

        // Convenience methods for specific events
        public static async Task SendGameLaunchWebhook(string gameName)
        {
            if (!App.Settings.Prop.WebhookOnGameLaunch)
                return;

            await SendWebhook(
                App.Settings.Prop.WebhookUrl,
                App.Settings.Prop.WebhookUsername,
                "🎮 Game Launched",
                $"Roblox game launched: **{gameName}**",
                0x00FF00 // Green
            );
        }

        public static async Task SendGameCloseWebhook()
        {
            if (!App.Settings.Prop.WebhookOnGameClose)
                return;

            await SendWebhook(
                App.Settings.Prop.WebhookUrl,
                App.Settings.Prop.WebhookUsername,
                "🛑 Game Closed",
                "Roblox game has been closed.",
                0xFF0000 // Red
            );
        }

        public static async Task SendUpdateWebhook(string version)
        {
            if (!App.Settings.Prop.WebhookOnUpdate)
                return;

            await SendWebhook(
                App.Settings.Prop.WebhookUrl,
                App.Settings.Prop.WebhookUsername,
                "🔄 Roblox Updated",
                $"Roblox has been updated to version **{version}**",
                0x0099FF // Blue
            );
        }

        public static async Task SendErrorWebhook(string errorMessage)
        {
            if (!App.Settings.Prop.WebhookOnError)
                return;

            await SendWebhook(
                App.Settings.Prop.WebhookUrl,
                App.Settings.Prop.WebhookUsername,
                "⚠️ Error Occurred",
                $"An error has occurred:\n```{errorMessage}```",
                0xFF0000 // Red
            );
        }
    }
}
