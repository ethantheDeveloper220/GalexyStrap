using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Forms;
using System.ComponentModel;
using MessageBox = System.Windows.MessageBox;

namespace Voidstrap.UI.ViewModels.Settings
{
    public class NotificationsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private NotifyIcon? _notifyIcon;

        // Notification Type Settings
        public bool UseTrayTip
        {
            get => App.Settings.Prop.UseTrayTip;
            set
            {
                App.Settings.Prop.UseTrayTip = value;
                OnPropertyChanged();
            }
        }

        public bool UseMessageBox
        {
            get => App.Settings.Prop.UseMessageBox;
            set
            {
                App.Settings.Prop.UseMessageBox = value;
                OnPropertyChanged();
            }
        }

        // Roblox Event Notifications
        public bool NotifyOnGameLaunch
        {
            get => App.Settings.Prop.NotifyOnGameLaunch;
            set
            {
                App.Settings.Prop.NotifyOnGameLaunch = value;
                OnPropertyChanged();
            }
        }

        public bool NotifyOnGameClose
        {
            get => App.Settings.Prop.NotifyOnGameClose;
            set
            {
                App.Settings.Prop.NotifyOnGameClose = value;
                OnPropertyChanged();
            }
        }

        public bool NotifyOnUpdate
        {
            get => App.Settings.Prop.NotifyOnUpdate;
            set
            {
                App.Settings.Prop.NotifyOnUpdate = value;
                OnPropertyChanged();
            }
        }

        // GalaxyStrap Event Notifications
        public bool NotifyOnSettingsSaved
        {
            get => App.Settings.Prop.NotifyOnSettingsSaved;
            set
            {
                App.Settings.Prop.NotifyOnSettingsSaved = value;
                OnPropertyChanged();
            }
        }

        public bool NotifyOnErrors
        {
            get => App.Settings.Prop.NotifyOnErrors;
            set
            {
                App.Settings.Prop.NotifyOnErrors = value;
                OnPropertyChanged();
            }
        }

        public bool NotifyOnGalaxyStrapUpdate
        {
            get => App.Settings.Prop.NotifyOnGalaxyStrapUpdate;
            set
            {
                App.Settings.Prop.NotifyOnGalaxyStrapUpdate = value;
                OnPropertyChanged();
            }
        }

        // Notification Behavior
        public int TrayTipDuration
        {
            get => App.Settings.Prop.TrayTipDuration;
            set
            {
                App.Settings.Prop.TrayTipDuration = value;
                OnPropertyChanged();
            }
        }

        public bool PlayNotificationSound
        {
            get => App.Settings.Prop.PlayNotificationSound;
            set
            {
                App.Settings.Prop.PlayNotificationSound = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public IRelayCommand TestTrayTipCommand => new RelayCommand(TestTrayTip);
        public IRelayCommand TestMessageBoxCommand => new RelayCommand(TestMessageBox);
        public IRelayCommand TestErrorCommand => new RelayCommand(TestError);

        public NotificationsViewModel()
        {
            InitializeNotifyIcon();
        }

        private void InitializeNotifyIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location),
                Visible = true,
                Text = ProjectInfo.PROJECT_NAME
            };
        }

        private void TestTrayTip()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.BalloonTipTitle = $"{ProjectInfo.PROJECT_NAME} Test Notification";
                _notifyIcon.BalloonTipText = "This is a test tray tip notification! If you can see this, tray tips are working correctly.";
                _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                _notifyIcon.ShowBalloonTip(TrayTipDuration * 1000);

                if (PlayNotificationSound)
                {
                    System.Media.SystemSounds.Asterisk.Play();
                }
            }
        }

        private void TestMessageBox()
        {
            if (PlayNotificationSound)
            {
                System.Media.SystemSounds.Asterisk.Play();
            }

            MessageBox.Show(
                "This is a test message box notification! If you can see this, message box notifications are working correctly.",
                $"{ProjectInfo.PROJECT_NAME} Test Notification",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void TestError()
        {
            if (UseTrayTip && _notifyIcon != null)
            {
                _notifyIcon.BalloonTipTitle = $"{ProjectInfo.PROJECT_NAME} Error";
                _notifyIcon.BalloonTipText = "This is a test error notification!";
                _notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
                _notifyIcon.ShowBalloonTip(TrayTipDuration * 1000);
            }

            if (UseMessageBox)
            {
                MessageBox.Show(
                    "This is a test error notification!",
                    $"{ProjectInfo.PROJECT_NAME} Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }

            if (PlayNotificationSound)
            {
                System.Media.SystemSounds.Hand.Play();
            }
        }

        public static void ShowNotification(string title, string message, NotificationType type = NotificationType.Info)
        {
            bool useTrayTip = App.Settings.Prop.UseTrayTip;
            bool useMessageBox = App.Settings.Prop.UseMessageBox;
            bool playSound = App.Settings.Prop.PlayNotificationSound;
            int duration = App.Settings.Prop.TrayTipDuration;

            // Show tray tip
            if (useTrayTip)
            {
                var notifyIcon = new NotifyIcon
                {
                    Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    Visible = true,
                    Text = ProjectInfo.PROJECT_NAME
                };

                notifyIcon.BalloonTipTitle = title;
                notifyIcon.BalloonTipText = message;
                notifyIcon.BalloonTipIcon = type switch
                {
                    NotificationType.Info => ToolTipIcon.Info,
                    NotificationType.Warning => ToolTipIcon.Warning,
                    NotificationType.Error => ToolTipIcon.Error,
                    _ => ToolTipIcon.None
                };
                notifyIcon.ShowBalloonTip(duration * 1000);

                // Clean up after showing
                notifyIcon.BalloonTipClosed += (s, e) => notifyIcon.Dispose();
            }

            // Show message box
            if (useMessageBox)
            {
                MessageBoxImage icon = type switch
                {
                    NotificationType.Info => MessageBoxImage.Information,
                    NotificationType.Warning => MessageBoxImage.Warning,
                    NotificationType.Error => MessageBoxImage.Error,
                    _ => MessageBoxImage.None
                };

                MessageBox.Show(message, title, MessageBoxButton.OK, icon);
            }

            // Play sound
            if (playSound)
            {
                System.Media.SystemSound sound = type switch
                {
                    NotificationType.Info => System.Media.SystemSounds.Asterisk,
                    NotificationType.Warning => System.Media.SystemSounds.Exclamation,
                    NotificationType.Error => System.Media.SystemSounds.Hand,
                    _ => System.Media.SystemSounds.Beep
                };
                sound.Play();
            }
        }
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Error
    }
}
