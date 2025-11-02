using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Drawing;
using Voidstrap.UI.Elements.Dialogs;
using Voidstrap.UI.ViewModels.Settings;
using Wpf.Ui.Common;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace Voidstrap.UI.Elements.Settings
{
    public partial class MainWindow : INavigationWindow
    {
        private Models.Persistable.WindowState _state => App.State.Prop.SettingsWindow;
        private bool _isSaveAndLaunchClicked = false;
        private NotifyIcon? _trayIcon;
        private bool _isReallyClosing = false;

        public MainWindow(bool showAlreadyRunningWarning)
        {
            InitializeComponent();
            InitializeViewModel();
            InitializeWindowState();
            InitializeNavigation();
            InitializeTrayIcon();
            UpdateButtonContent();
            App.Logger.WriteLine("MainWindow", "Initializing settings window");
            if (showAlreadyRunningWarning)
                _ = ShowAlreadyRunningSnackbarAsync();
        }



        #region Initialization

        private void InitializeViewModel()
        {
            var viewModel = new MainWindowViewModel();
            DataContext = viewModel;

            viewModel.RequestSaveNoticeEvent += OnRequestSaveNotice;
            viewModel.RequestSaveLaunchNoticeEvent += OnRequestSaveLaunchNotice;
            viewModel.RequestCloseWindowEvent += OnRequestCloseWindow;
        }

        private void UpdateButtonContent()
        {
            if (InstallLaunchButton == null)
                return;

            string versionsPath = Paths.Versions;

            InstallLaunchButton.Content =
                (Directory.Exists(versionsPath) && Directory.EnumerateFileSystemEntries(versionsPath).Any())
                    ? "Save and Launch"
                    : "Install";
        }


        private void InitializeWindowState()
        {
            if (_state.Left > SystemParameters.VirtualScreenWidth || _state.Top > SystemParameters.VirtualScreenHeight)
            {
                _state.Left = 0;
                _state.Top = 0;
            }

            if (_state.Width > 0) Width = _state.Width;
            if (_state.Height > 0) Height = _state.Height;

            if (_state.Left > 0 && _state.Top > 0)
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                Left = _state.Left;
                Top = _state.Top;
            }
        }

        private void InitializeNavigation()
        {
            if (RootNavigation == null)
                return;

            RootNavigation.SelectedPageIndex = App.State.Prop.LastPage;
            RootNavigation.Navigated += SaveNavigation;
        }

        private void InitializeTrayIcon()
        {
            _trayIcon = new NotifyIcon
            {
                Icon = Properties.Resources.IconBloodstrap,
                Text = "Bloodstrap - Click to show",
                Visible = false
            };

            // Create context menu
            var contextMenu = new ContextMenuStrip();
            contextMenu.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
            contextMenu.ForeColor = System.Drawing.Color.White;
            contextMenu.Renderer = new ModernMenuRenderer();

            // Show/Hide item
            var showItem = new ToolStripMenuItem
            {
                Text = "Show Bloodstrap",
                Image = ResizeIcon(SystemIcons.Application, 16, 16),
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold)
            };
            showItem.Click += (s, e) => ShowFromTray();
            contextMenu.Items.Add(showItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            // Webhook Manager item
            var webhookItem = new ToolStripMenuItem
            {
                Text = "Webhook Manager",
                Image = ResizeIcon(SystemIcons.Shield, 16, 16)
            };
            webhookItem.Click += (s, e) =>
            {
                ShowFromTray();
                var webhookManager = new WebhookManagerDialog();
                webhookManager.ShowDialog();
            };
            contextMenu.Items.Add(webhookItem);

            // Activity Logger item
            var activityItem = new ToolStripMenuItem
            {
                Text = "Activity Logger",
                Image = ResizeIcon(SystemIcons.Information, 16, 16)
            };
            activityItem.Click += (s, e) =>
            {
                ShowFromTray();
                // Open activity logger if you have one
            };
            contextMenu.Items.Add(activityItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            // Settings item
            var settingsItem = new ToolStripMenuItem
            {
                Text = "Settings",
                Image = ResizeIcon(SystemIcons.WinLogo, 16, 16)
            };
            settingsItem.Click += (s, e) => ShowFromTray();
            contextMenu.Items.Add(settingsItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            // Exit item
            var exitItem = new ToolStripMenuItem
            {
                Text = "Exit Bloodstrap",
                Image = ResizeIcon(SystemIcons.Error, 16, 16),
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular)
            };
            exitItem.Click += (s, e) => ExitApplication();
            contextMenu.Items.Add(exitItem);

            _trayIcon.ContextMenuStrip = contextMenu;
            _trayIcon.MouseClick += TrayIcon_MouseClick;
        }

        private void TrayIcon_MouseClick(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowFromTray();
            }
        }

        private void ShowFromTray()
        {
            Show();
            this.WindowState = System.Windows.WindowState.Normal;
            Activate();
            _trayIcon!.Visible = false;
        }

        private void ExitApplication()
        {
            _isReallyClosing = true;
            _trayIcon?.Dispose();
            Close();
        }

        private Bitmap ResizeIcon(Icon icon, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(icon.ToBitmap(), 0, 0, width, height);
            }
            return bitmap;
        }

        #endregion

        #region Snackbar Events

        private void OnRequestSaveNotice(object? sender, EventArgs e)
        {
            if (!_isSaveAndLaunchClicked)
                SettingsSavedSnackbar.Show();
        }

        private void OnRequestSaveLaunchNotice(object? sender, EventArgs e)
        {
            if (!_isSaveAndLaunchClicked)
                SettingsSavedLaunchSnackbar.Show();
        }

        private async Task ShowAlreadyRunningSnackbarAsync()
        {
            await Task.Delay(225);
            if (!Dispatcher.HasShutdownStarted)
                Dispatcher.InvokeAsync(() => AlreadyRunningSnackbar?.Show());
        }


        #endregion

        #region ViewModel Events

        private async void OnRequestCloseWindow(object? sender, EventArgs e)
        {
            await Task.Yield();
            Close();
        }

        private void OnSaveAndLaunchButtonClick(object sender, EventArgs e)
        {
            _isSaveAndLaunchClicked = true;
        }

        #endregion

        #region Window Events

        private void WpfUiWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isReallyClosing)
            {
                // Minimize to tray instead of closing
                e.Cancel = true;
                Hide();
                _trayIcon!.Visible = true;
                _trayIcon.ShowBalloonTip(2000, "Bloodstrap", "Bloodstrap is still running in the system tray", ToolTipIcon.Info);
            }
            else
            {
                SaveWindowState();
            }
        }

        private void WpfUiWindow_Closed(object sender, EventArgs e)
        {
            if (_isReallyClosing)
            {
                if (App.LaunchSettings.TestModeFlag.Active)
                    LaunchHandler.LaunchRoblox(LaunchMode.Player);
                else
                    App.SoftTerminate();
            }
        }

        private void SaveWindowState()
        {
            _state.Width = Width;
            _state.Height = Height;
            _state.Top = Top;
            _state.Left = Left;

            App.State.Save();
        }

        #endregion

        #region Navigation

        private void SaveNavigation(INavigation sender, RoutedNavigationEventArgs e)
        {
            App.State.Prop.LastPage = RootNavigation.SelectedPageIndex;
        }

        #endregion

        #region INavigationWindow Implementation

        public Frame GetFrame() => RootFrame;
        public INavigation GetNavigation() => RootNavigation;
        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);
        public void SetPageService(IPageService pageService) => RootNavigation.PageService = pageService;
        public void ShowWindow() => Show();
        public void CloseWindow() => Close();

        #endregion

        #region Placeholder Events

        // TODO: Implement these handlers or remove if unused
        private void NavigationItem_Click(object sender, RoutedEventArgs e) { }
        private void NavigationItem_Click_1(object sender, RoutedEventArgs e) { }
        private void Button_Click(object sender, RoutedEventArgs e) { }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }


        private void Button_Click_2(object sender, RoutedEventArgs e) { }

        #endregion
    }

    // Modern dark theme renderer for context menu
    public class ModernMenuRenderer : ToolStripProfessionalRenderer
    {
        public ModernMenuRenderer() : base(new ModernColorTable()) { }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                var rect = new Rectangle(System.Drawing.Point.Empty, e.Item.Size);
                using (var brush = new SolidBrush(System.Drawing.Color.FromArgb(60, 60, 60)))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            }
            else
            {
                base.OnRenderMenuItemBackground(e);
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            var rect = new Rectangle(30, 0, e.Item.Width - 30, 1);
            using (var pen = new Pen(System.Drawing.Color.FromArgb(60, 60, 60)))
            {
                e.Graphics.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
            }
        }
    }

    public class ModernColorTable : ProfessionalColorTable
    {
        public override System.Drawing.Color MenuItemSelected => System.Drawing.Color.FromArgb(60, 60, 60);
        public override System.Drawing.Color MenuItemSelectedGradientBegin => System.Drawing.Color.FromArgb(60, 60, 60);
        public override System.Drawing.Color MenuItemSelectedGradientEnd => System.Drawing.Color.FromArgb(60, 60, 60);
        public override System.Drawing.Color MenuItemBorder => System.Drawing.Color.FromArgb(60, 60, 60);
        public override System.Drawing.Color MenuBorder => System.Drawing.Color.FromArgb(45, 45, 45);
        public override System.Drawing.Color ImageMarginGradientBegin => System.Drawing.Color.FromArgb(32, 32, 32);
        public override System.Drawing.Color ImageMarginGradientMiddle => System.Drawing.Color.FromArgb(32, 32, 32);
        public override System.Drawing.Color ImageMarginGradientEnd => System.Drawing.Color.FromArgb(32, 32, 32);
    }
}
