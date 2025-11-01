using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Voidstrap.UI.Elements.ContextMenu
{
    public partial class BadgesViewer
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public ObservableCollection<BadgeItem> Badges { get; set; } = new ObservableCollection<BadgeItem>();

        public BadgesViewer(long userId)
        {
            InitializeComponent();
            BadgesListBox.ItemsSource = Badges;
            _ = LoadBadgesAsync(userId);
        }

        private async Task LoadBadgesAsync(long userId)
        {
            try
            {
                LoadingRing.Visibility = Visibility.Visible;
                BadgesListBox.Visibility = Visibility.Collapsed;

                var response = await _httpClient.GetAsync($"https://badges.roblox.com/v1/users/{userId}/badges?limit=100&sortOrder=Asc");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var doc = JsonDocument.Parse(json);
                    
                    if (doc.RootElement.TryGetProperty("data", out var data))
                    {
                        foreach (var badge in data.EnumerateArray())
                        {
                            Badges.Add(new BadgeItem
                            {
                                Name = badge.GetProperty("name").GetString() ?? "Unknown",
                                Description = badge.GetProperty("description").GetString() ?? "",
                                IconUrl = badge.GetProperty("iconImageId").GetInt64() > 0 
                                    ? $"https://assetdelivery.roblox.com/v1/asset?id={badge.GetProperty("iconImageId").GetInt64()}"
                                    : ""
                            });
                        }
                    }
                }

                LoadingRing.Visibility = Visibility.Collapsed;
                BadgesListBox.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading badges: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadingRing.Visibility = Visibility.Collapsed;
            }
        }
    }

    public class BadgeItem
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string IconUrl { get; set; } = "";
    }
}
