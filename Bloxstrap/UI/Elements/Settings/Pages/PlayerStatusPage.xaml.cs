using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class PlayerStatusPage : UiPage
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private long? _currentUserId = null;

        public PlayerStatusPage()
        {
            InitializeComponent();
            _ = LoadCurrentUserAsync();
        }

        private async Task LoadCurrentUserAsync()
        {
            const string LOG_IDENT = "PlayerStatusPage::LoadCurrentUserAsync";

            try
            {
                UpdateStatus("Loading account information...", "#FFA500");

                // Try to get authenticated user from Roblox cookies
                var userId = await GetAuthenticatedUserIdAsync();

                if (userId.HasValue)
                {
                    _currentUserId = userId.Value;
                    await LoadUserInfoAsync(userId.Value);
                    await LoadUserStatsAsync(userId.Value);
                }
                else
                {
                    UpdateStatus("No Roblox account logged in. Please log in to Roblox first.", "#FF6666");
                    UsernameText.Text = "Not logged in";
                    UserIdText.Text = "-";
                    AccountAgeText.Text = "-";
                    PremiumText.Text = "-";
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Error loading user: {ex.Message}");
                UpdateStatus($"Error loading account: {ex.Message}", "#FF6666");
            }
        }

        private async Task<long?> GetAuthenticatedUserIdAsync()
        {
            try
            {
                // Try to get user ID from Roblox authentication
                var response = await _httpClient.GetAsync("https://users.roblox.com/v1/users/authenticated");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("id", out var idProp))
                    {
                        return idProp.GetInt64();
                    }
                }
            }
            catch { }

            return null;
        }

        private async Task LoadUserInfoAsync(long userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://users.roblox.com/v1/users/{userId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    // Update UI
                    UsernameText.Text = root.GetProperty("name").GetString() ?? "Unknown";
                    UserIdText.Text = userId.ToString();
                    
                    if (root.TryGetProperty("created", out var created))
                    {
                        var createdDate = DateTime.Parse(created.GetString());
                        var age = (DateTime.Now - createdDate).Days;
                        AccountAgeText.Text = $"{age} days ({age / 365} years)";
                    }

                    // Check premium status
                    var premiumResponse = await _httpClient.GetAsync($"https://premiumfeatures.roblox.com/v1/users/{userId}/validate-membership");
                    if (premiumResponse.IsSuccessStatusCode)
                    {
                        var premiumJson = await premiumResponse.Content.ReadAsStringAsync();
                        var premiumDoc = JsonDocument.Parse(premiumJson);
                        var isPremium = premiumDoc.RootElement.GetProperty("isPremium").GetBoolean();
                        PremiumText.Text = isPremium ? "✓ Premium" : "No Premium";
                    }

                    UpdateStatus("Account information loaded successfully!", "#66FF66");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error loading user info: {ex.Message}", "#FF6666");
            }
        }

        private async Task LoadUserStatsAsync(long userId)
        {
            try
            {
                // Get friends count
                var friendsResponse = await _httpClient.GetAsync($"https://friends.roblox.com/v1/users/{userId}/friends/count");
                if (friendsResponse.IsSuccessStatusCode)
                {
                    var json = await friendsResponse.Content.ReadAsStringAsync();
                    var doc = JsonDocument.Parse(json);
                    FriendsCountText.Text = doc.RootElement.GetProperty("count").GetInt32().ToString();
                }

                // Get followers count
                var followersResponse = await _httpClient.GetAsync($"https://friends.roblox.com/v1/users/{userId}/followers/count");
                if (followersResponse.IsSuccessStatusCode)
                {
                    var json = await followersResponse.Content.ReadAsStringAsync();
                    var doc = JsonDocument.Parse(json);
                    FollowersCountText.Text = doc.RootElement.GetProperty("count").GetInt32().ToString();
                }

                // Get following count
                var followingResponse = await _httpClient.GetAsync($"https://friends.roblox.com/v1/users/{userId}/followings/count");
                if (followingResponse.IsSuccessStatusCode)
                {
                    var json = await followingResponse.Content.ReadAsStringAsync();
                    var doc = JsonDocument.Parse(json);
                    FollowingCountText.Text = doc.RootElement.GetProperty("count").GetInt32().ToString();
                }

                // Get groups count
                var groupsResponse = await _httpClient.GetAsync($"https://groups.roblox.com/v1/users/{userId}/groups/roles");
                if (groupsResponse.IsSuccessStatusCode)
                {
                    var json = await groupsResponse.Content.ReadAsStringAsync();
                    var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("data", out var data))
                    {
                        GroupsCountText.Text = data.GetArrayLength().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("PlayerStatusPage", $"Error loading stats: {ex.Message}");
            }
        }

        private void UpdateStatus(string message, string colorHex)
        {
            StatusText.Text = message;
            // Update icon color based on status
            if (colorHex == "#66FF66")
                StatusIcon.Symbol = Wpf.Ui.Common.SymbolRegular.CheckmarkCircle24;
            else if (colorHex == "#FF6666")
                StatusIcon.Symbol = Wpf.Ui.Common.SymbolRegular.ErrorCircle24;
            else
                StatusIcon.Symbol = Wpf.Ui.Common.SymbolRegular.Info24;
        }

        private void RefreshAccount_Click(object sender, RoutedEventArgs e)
        {
            _ = LoadCurrentUserAsync();
        }

        private void RefreshStats_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUserId.HasValue)
            {
                _ = LoadUserStatsAsync(_currentUserId.Value);
                UpdateStatus("Statistics refreshed!", "#66FF66");
            }
            else
            {
                UpdateStatus("No account loaded. Click 'Refresh Account Info' first.", "#FF6666");
            }
        }

        private void OpenBadges_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUserId.HasValue)
            {
                // Open badges viewer window
                var badgesWindow = new Elements.ContextMenu.BadgesViewer(_currentUserId.Value);
                badgesWindow.Show();
                UpdateStatus($"Opened Badges Viewer", "#66FF66");
            }
            else
            {
                UpdateStatus("No account loaded!", "#FF6666");
            }
        }

        private void OpenInventory_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUserId.HasValue)
            {
                // Open inventory viewer window
                var inventoryWindow = new Elements.ContextMenu.InventoryViewer(_currentUserId.Value);
                inventoryWindow.Show();
                UpdateStatus($"Opened Inventory Viewer", "#66FF66");
            }
            else
            {
                UpdateStatus("No account loaded!", "#FF6666");
            }
        }

        private void OpenGamePasses_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUserId.HasValue)
            {
                // Open gamepass console window
                var gamePassWindow = new Elements.ContextMenu.GamePassConsole(_currentUserId.Value);
                gamePassWindow.Show();
                UpdateStatus($"Opened GamePass Console", "#66FF66");
            }
            else
            {
                UpdateStatus("No account loaded!", "#FF6666");
            }
        }

        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUserId.HasValue)
            {
                OpenUrl($"https://www.roblox.com/users/{_currentUserId}/profile");
            }
            else
            {
                UpdateStatus("No account loaded!", "#FF6666");
            }
        }

        private void OpenFriends_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUserId.HasValue)
            {
                OpenUrl($"https://www.roblox.com/users/{_currentUserId}/friends");
            }
            else
            {
                UpdateStatus("No account loaded!", "#FF6666");
            }
        }

        private void OpenGroups_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUserId.HasValue)
            {
                OpenUrl($"https://www.roblox.com/users/{_currentUserId}/groups");
            }
            else
            {
                UpdateStatus("No account loaded!", "#FF6666");
            }
        }

        private void OpenTransactions_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://www.roblox.com/transactions");
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
                UpdateStatus($"Opened in browser: {url}", "#66FF66");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error opening URL: {ex.Message}", "#FF6666");
            }
        }

        private void LoginWeb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a new window for web login
                var loginWindow = new Window
                {
                    Title = "Roblox Login - GalaxyStrap",
                    Width = 800,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                var webView = new WebView2
                {
                    Source = new Uri("https://www.roblox.com/login")
                };

                // Handle navigation completed to detect successful login
                webView.NavigationCompleted += async (s, args) =>
                {
                    try
                    {
                        // Check if user is logged in by trying to get cookies
                        var cookies = await webView.CoreWebView2.CookieManager.GetCookiesAsync("https://www.roblox.com");
                        
                        bool hasRobloSecurity = false;
                        foreach (var cookie in cookies)
                        {
                            if (cookie.Name == ".ROBLOSECURITY")
                            {
                                hasRobloSecurity = true;
                                break;
                            }
                        }

                        if (hasRobloSecurity && webView.Source.ToString().Contains("roblox.com/home"))
                        {
                            // User successfully logged in - save the cookie
                            foreach (var cookie in cookies)
                            {
                                if (cookie.Name == ".ROBLOSECURITY")
                                {
                                    await SaveRobloxCookieAsync(cookie.Value);
                                    break;
                                }
                            }
                            
                            UpdateStatus("Login successful! Cookie saved. Refreshing account info...", "#66FF66");
                            loginWindow.Close();
                            await Task.Delay(500);
                            await LoadCurrentUserAsync();
                        }
                    }
                    catch { }
                };

                // Handle initialization
                webView.CoreWebView2InitializationCompleted += (s, args) =>
                {
                    if (args.IsSuccess)
                    {
                        UpdateStatus("Web login window opened. Please log in to your Roblox account.", "#FFA500");
                    }
                    else
                    {
                        UpdateStatus($"Failed to initialize web view: {args.InitializationException?.Message}", "#FF6666");
                        loginWindow.Close();
                    }
                };

                loginWindow.Content = webView;
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error opening web login: {ex.Message}", "#FF6666");
                
                // Fallback: Open in default browser
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://www.roblox.com/login",
                        UseShellExecute = true
                    });
                    UpdateStatus("Opened Roblox login in your default browser. After logging in, click 'Refresh Account Info'.", "#FFA500");
                }
                catch { }
            }
        }

        private async Task SaveRobloxCookieAsync(string cookieValue)
        {
            const string LOG_IDENT = "PlayerStatusPage::SaveRobloxCookie";

            try
            {
                // Save to Roblox's cookie file location
                string cookieFilePath = App.RobloxCookiesFilePath;
                string cookieDir = Path.GetDirectoryName(cookieFilePath);

                // Create directory if it doesn't exist
                if (!Directory.Exists(cookieDir))
                {
                    Directory.CreateDirectory(cookieDir);
                }

                // Format: .ROBLOSECURITY cookie value
                string cookieData = $".ROBLOSECURITY\t{cookieValue}";

                // Write to file
                await File.WriteAllTextAsync(cookieFilePath, cookieData, Encoding.UTF8);

                App.Logger.WriteLine(LOG_IDENT, "Successfully saved Roblox cookie");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Error saving cookie: {ex.Message}");
            }
        }
    }
}
