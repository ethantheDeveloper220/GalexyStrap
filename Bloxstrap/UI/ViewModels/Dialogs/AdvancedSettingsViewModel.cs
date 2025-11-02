using System.ComponentModel;

namespace Voidstrap.UI.ViewModels.Dialogs
{
    public class AdvancedSettingViewModel : INotifyPropertyChanged
    {
        public static event EventHandler? ShowPresetColumnChanged;
        public static event EventHandler? CtrlCJsonFormatChanged;
        public static event EventHandler? ShowFlagCountChanged;
        public event EventHandler? ShowAddWithIDChanged;

        // Placeholder properties - these would need to be connected to actual GalaxyStrap Settings
        private bool _showCtrlCJsonFormat;
        private bool _showPresetColumn;
        private bool _showFlagCount;
        private bool _showAddWithID;
        private bool _useAltManually;

        public bool ShowCtrlCJsonFormatSetting
        {
            get => _showCtrlCJsonFormat;
            set
            {
                if (_showCtrlCJsonFormat != value)
                {
                    _showCtrlCJsonFormat = value;
                    OnPropertyChanged(nameof(ShowCtrlCJsonFormatSetting));
                    CtrlCJsonFormatChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool ShowPresetColumnSetting
        {
            get => _showPresetColumn;
            set
            {
                if (_showPresetColumn != value)
                {
                    _showPresetColumn = value;
                    OnPropertyChanged(nameof(ShowPresetColumnSetting));
                    ShowPresetColumnChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool ShowFlagCount
        {
            get => _showFlagCount;
            set
            {
                if (_showFlagCount != value)
                {
                    _showFlagCount = value;
                    OnPropertyChanged(nameof(ShowFlagCount));
                    ShowFlagCountChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool ShowAddWithID
        {
            get => _showAddWithID;
            set
            {
                if (_showAddWithID != value)
                {
                    _showAddWithID = value;
                    OnPropertyChanged(nameof(ShowAddWithID));
                    ShowAddWithIDChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool UseAltManually
        {
            get => _useAltManually;
            set
            {
                if (_useAltManually != value)
                {
                    _useAltManually = value;
                    OnPropertyChanged(nameof(UseAltManually));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
