using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Voidstrap.UI.Elements.ContextMenu
{
    public partial class InventoryViewer
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public ObservableCollection<InventoryItem> Items { get; set; } = new ObservableCollection<InventoryItem>();
        private long _userId;

        public InventoryViewer(long userId)
        {
            InitializeComponent();
            _userId = userId;
            InventoryListBox.ItemsSource = Items;
            _ = LoadInventoryAsync("All");
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem is ComboBoxItem item)
            {
                string category = item.Content.ToString() ?? "All";
                _ = LoadInventoryAsync(category);
            }
        }

        private async Task LoadInventoryAsync(string category)
        {
            try
            {
                LoadingRing.Visibility = Visibility.Visible;
                InventoryListBox.Visibility = Visibility.Collapsed;
                Items.Clear();

                // Map category to asset type ID
                int? assetTypeId = category switch
                {
                    "Accessories" => 8,
                    "Gear" => 19,
                    "Faces" => 18,
                    "Heads" => 17,
                    "T-Shirts" => 2,
                    "Shirts" => 11,
                    "Pants" => 12,
                    _ => null
                };

                string url = assetTypeId.HasValue
                    ? $"https://inventory.roblox.com/v1/users/{_userId}/assets/collectibles?assetType={assetTypeId}&limit=100&sortOrder=Asc"
                    : $"https://inventory.roblox.com/v1/users/{_userId}/assets/collectibles?limit=100&sortOrder=Asc";

                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var doc = JsonDocument.Parse(json);
                    
                    if (doc.RootElement.TryGetProperty("data", out var data))
                    {
                        foreach (var item in data.EnumerateArray())
                        {
                            long assetId = item.GetProperty("assetId").GetInt64();
                            
                            Items.Add(new InventoryItem
                            {
                                Name = item.GetProperty("name").GetString() ?? "Unknown",
                                AssetType = item.GetProperty("assetType").GetString() ?? "",
                                ThumbnailUrl = $"https://assetdelivery.roblox.com/v1/asset?id={assetId}"
                            });
                        }
                    }
                }

                LoadingRing.Visibility = Visibility.Collapsed;
                InventoryListBox.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading inventory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadingRing.Visibility = Visibility.Collapsed;
            }
        }
    }

    public class InventoryItem
    {
        public string Name { get; set; } = "";
        public string AssetType { get; set; } = "";
        public string ThumbnailUrl { get; set; } = "";
    }
}
