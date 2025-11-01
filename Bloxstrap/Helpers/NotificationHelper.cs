using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Voidstrap.Helpers
{
    public static class NotificationHelper
    {
        /// <summary>
        /// Shows a notification based on user preferences
        /// </summary>
        public static void ShowNotification(string title, string message, NotificationType type = NotificationType.Info)
        {
            bool useTrayTip = App.Settings.Prop.UseTrayTip;
            bool useMessageBox = App.Settings.Prop.UseMessageBox;
            bool playSound = App.Settings.Prop.PlayNotificationSound;
            int duration = App.Settings.Prop.TrayTipDuration;

            // Show tray tip
            if (useTrayTip)
            {
                ShowTrayTip(title, message, type, duration);
            }

            // Show message box
            if (useMessageBox)
            {
                ShowMessageBox(title, message, type);
            }

            // Play sound
            if (playSound)
            {
                PlayNotificationSound(type);
            }
        }

        /// <summary>
        /// Shows a tray tip notification
        /// </summary>
        private static void ShowTrayTip(string title, string message, NotificationType type, int durationSeconds)
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

            notifyIcon.ShowBalloonTip(durationSeconds * 1000);

            // Clean up after showing
            notifyIcon.BalloonTipClosed += (s, e) =>
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            };

            // Also dispose after timeout
            System.Threading.Tasks.Task.Delay((durationSeconds + 1) * 1000).ContinueWith(_ =>
            {
                try
                {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                }
                catch { }
            });
        }

        /// <summary>
        /// Shows a message box notification
        /// </summary>
        private static void ShowMessageBox(string title, string message, NotificationType type)
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

        /// <summary>
        /// Plays a notification sound
        /// </summary>
        private static void PlayNotificationSound(NotificationType type)
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

        // Convenience methods for specific events
        public static void NotifyGameLaunch()
        {
            if (App.Settings.Prop.NotifyOnGameLaunch)
            {
                ShowNotification(
                    $"{ProjectInfo.PROJECT_NAME} - Game Launched",
                    "Roblox has been launched successfully!",
                    NotificationType.Info
                );
            }
        }

        public static void NotifyGameClose()
        {
            if (App.Settings.Prop.NotifyOnGameClose)
            {
                ShowNotification(
                    $"{ProjectInfo.PROJECT_NAME} - Game Closed",
                    "Roblox has been closed.",
                    NotificationType.Info
                );
            }
        }

        public static void NotifyUpdate(string version)
        {
            if (App.Settings.Prop.NotifyOnUpdate)
            {
                ShowNotification(
                    $"{ProjectInfo.PROJECT_NAME} - Update Downloaded",
                    $"Roblox has been updated to version {version}",
                    NotificationType.Info
                );
            }
        }

        public static void NotifySettingsSaved()
        {
            if (App.Settings.Prop.NotifyOnSettingsSaved)
            {
                ShowNotification(
                    $"{ProjectInfo.PROJECT_NAME} - Settings Saved",
                    "Your settings have been saved successfully!",
                    NotificationType.Info
                );
            }
        }

        public static void NotifyError(string errorMessage)
        {
            if (App.Settings.Prop.NotifyOnErrors)
            {
                ShowNotification(
                    $"{ProjectInfo.PROJECT_NAME} - Error",
                    errorMessage,
                    NotificationType.Error
                );
            }
        }

        public static void NotifyBloodstrapUpdate(string version)
        {
            if (App.Settings.Prop.NotifyOnBloodstrapUpdate)
            {
                ShowNotification(
                    $"{ProjectInfo.PROJECT_NAME} - Update Available",
                    $"A new version of {ProjectInfo.PROJECT_NAME} is available: v{version}",
                    NotificationType.Info
                );
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
