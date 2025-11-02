using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Wpf.Ui.Controls;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class RegionPage : UiPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public RegionPage()
        {
            InitializeComponent();
            DataContext = this;
            InitializeRegions();
            UpdateRegionInfo();
        }

        private void InitializeRegions()
        {
            AvailableRegions = new ObservableCollection<ServerRegion>
            {
                new ServerRegion { Name = "Automatic (Best)", Code = "auto", Description = "Automatically select the best server based on ping" },
                new ServerRegion { Name = "US East (Virginia)", Code = "us-east", Description = "East Coast USA - Good for Eastern US and South America" },
                new ServerRegion { Name = "US West (California)", Code = "us-west", Description = "West Coast USA - Good for Western US and Pacific" },
                new ServerRegion { Name = "US Central (Texas)", Code = "us-central", Description = "Central USA - Balanced for all US regions" },
                new ServerRegion { Name = "EU West (Ireland)", Code = "eu-west", Description = "Western Europe - Good for UK, Ireland, Western EU" },
                new ServerRegion { Name = "EU Central (Germany)", Code = "eu-central", Description = "Central Europe - Good for Central and Eastern EU" },
                new ServerRegion { Name = "Asia (Singapore)", Code = "asia-sg", Description = "Southeast Asia - Good for SEA region" },
                new ServerRegion { Name = "Asia (Japan)", Code = "asia-jp", Description = "East Asia - Good for Japan, Korea, nearby regions" },
                new ServerRegion { Name = "Asia (Hong Kong)", Code = "asia-hk", Description = "East Asia - Good for China, Taiwan, nearby regions" },
                new ServerRegion { Name = "South America (Brazil)", Code = "sa-br", Description = "South America - Best for Brazilian players" },
                new ServerRegion { Name = "Oceania (Australia)", Code = "oce-au", Description = "Australia/New Zealand - Best for Oceania region" }
            };

            // Set default region if not already set
            if (string.IsNullOrEmpty(App.Settings.Prop.PreferredServerRegion))
            {
                App.Settings.Prop.PreferredServerRegion = "auto";
            }

            // Find and set the selected region
            var savedRegion = AvailableRegions.FirstOrDefault(r => r.Code == App.Settings.Prop.PreferredServerRegion);
            if (savedRegion != null)
            {
                SelectedRegion = savedRegion;
            }
            else
            {
                SelectedRegion = AvailableRegions[0]; // Default to Automatic
            }
        }

        // Region Selection Properties
        public ObservableCollection<ServerRegion> AvailableRegions { get; set; } = new();

        private ServerRegion? _selectedRegion;
        public ServerRegion? SelectedRegion
        {
            get => _selectedRegion;
            set
            {
                _selectedRegion = value;
                if (value != null)
                {
                    App.Settings.Prop.PreferredServerRegion = value.Code;
                    UpdateRegionInfo();
                    ApplyRegionSettings();
                }
                OnPropertyChanged();
            }
        }

        public bool EnableRegionSelection
        {
            get => App.Settings.Prop.EnableRegionSelection;
            set
            {
                App.Settings.Prop.EnableRegionSelection = value;
                OnPropertyChanged();
                ApplyRegionSettings();
            }
        }

        // Auto-Select Best Server Properties
        public bool EnableAutoSelectBestServer
        {
            get => App.Settings.Prop.EnableAutoSelectBestServer;
            set
            {
                App.Settings.Prop.EnableAutoSelectBestServer = value;
                OnPropertyChanged();
            }
        }

        public int MaxPingThreshold
        {
            get => App.Settings.Prop.MaxPingThreshold;
            set
            {
                App.Settings.Prop.MaxPingThreshold = value;
                OnPropertyChanged();
            }
        }

        public bool ShowPingInGame
        {
            get => App.Settings.Prop.ShowPingInGame;
            set
            {
                App.Settings.Prop.ShowPingInGame = value;
                OnPropertyChanged();
                ApplyPingDisplaySettings();
            }
        }

        // Connection Optimization Properties
        public bool ReduceNetworkLag
        {
            get => App.Settings.Prop.ReduceNetworkLag;
            set
            {
                App.Settings.Prop.ReduceNetworkLag = value;
                OnPropertyChanged();
                ApplyNetworkOptimization();
            }
        }

        public bool IncreaseBandwidth
        {
            get => App.Settings.Prop.IncreaseBandwidth;
            set
            {
                App.Settings.Prop.IncreaseBandwidth = value;
                OnPropertyChanged();
                ApplyBandwidthSettings();
            }
        }

        public bool OptimizePacketSize
        {
            get => App.Settings.Prop.OptimizePacketSize;
            set
            {
                App.Settings.Prop.OptimizePacketSize = value;
                OnPropertyChanged();
                ApplyPacketOptimization();
            }
        }

        public bool EnableFastPreloading
        {
            get => App.Settings.Prop.EnableFastPreloading;
            set
            {
                App.Settings.Prop.EnableFastPreloading = value;
                OnPropertyChanged();
                ApplyPreloadingSettings();
            }
        }

        private void UpdateRegionInfo()
        {
            if (SelectedRegion != null)
            {
                RegionInfoText.Text = $"Selected: {SelectedRegion.Name}\n{SelectedRegion.Description}";
            }
            else
            {
                RegionInfoText.Text = "Select a region to see details";
            }
        }

        private void ApplyRegionSettings()
        {
            if (!EnableRegionSelection || SelectedRegion == null)
            {
                // Remove region-specific flags
                App.FastFlags?.SetPreset("Network.PreferredRegion", null);
                return;
            }

            // Apply region-specific FastFlags
            // Note: These are example flags - actual implementation may vary
            if (SelectedRegion.Code != "auto")
            {
                App.FastFlags?.SetPreset("Network.PreferredRegion", SelectedRegion.Code);
            }
            else
            {
                App.FastFlags?.SetPreset("Network.PreferredRegion", null);
            }
        }

        private void ApplyPingDisplaySettings()
        {
            if (ShowPingInGame)
            {
                App.FastFlags?.SetPreset("Debug.PingBreakdown", "True");
            }
            else
            {
                App.FastFlags?.SetPreset("Debug.PingBreakdown", null);
            }
        }

        private void ApplyNetworkOptimization()
        {
            if (ReduceNetworkLag)
            {
                // Apply network optimization flags
                App.FastFlags?.SetPreset("Network.DefaultBps", "10000000");
                App.FastFlags?.SetPreset("Network.MaxWorkCatchupMs", "100");
            }
            else
            {
                App.FastFlags?.SetPreset("Network.DefaultBps", null);
                App.FastFlags?.SetPreset("Network.MaxWorkCatchupMs", null);
            }
        }

        private void ApplyBandwidthSettings()
        {
            if (IncreaseBandwidth)
            {
                // Increase payload limits for better bandwidth
                App.FastFlags?.SetPreset("Network.Payload1", "60000");
                App.FastFlags?.SetPreset("Network.Payload2", "60000");
                App.FastFlags?.SetPreset("Network.Payload3", "60000");
                App.FastFlags?.SetPreset("Network.Payload4", "60000");
            }
            else
            {
                App.FastFlags?.SetPreset("Network.Payload1", null);
                App.FastFlags?.SetPreset("Network.Payload2", null);
                App.FastFlags?.SetPreset("Network.Payload3", null);
                App.FastFlags?.SetPreset("Network.Payload4", null);
            }
        }

        private void ApplyPacketOptimization()
        {
            if (OptimizePacketSize)
            {
                App.FastFlags?.SetPreset("Network.Mtusize", "1400");
            }
            else
            {
                App.FastFlags?.SetPreset("Network.Mtusize", null);
            }
        }

        private void ApplyPreloadingSettings()
        {
            if (EnableFastPreloading)
            {
                App.FastFlags?.SetPreset("Network.MeshPreloadding", "True");
                App.FastFlags?.SetPreset("Network.MaxAssetPreload", "100");
            }
            else
            {
                App.FastFlags?.SetPreset("Network.MeshPreloadding", null);
                App.FastFlags?.SetPreset("Network.MaxAssetPreload", null);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ServerRegion
    {
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
