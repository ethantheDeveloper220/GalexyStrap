using System.Windows;
using Voidstrap.UI.ViewModels.Settings;
using Voidstrap.UI.Elements.Dialogs;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class WebhooksPage
    {
        public WebhooksPage()
        {
            InitializeComponent();
            DataContext = new WebhooksViewModel();
        }

        private async void SendTestWebhook_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as WebhooksViewModel;
            if (viewModel != null)
            {
                await viewModel.SendTestWebhook();
            }
        }

        private void OpenWebhookManager_Click(object sender, RoutedEventArgs e)
        {
            var webhookManager = new WebhookManagerDialog();
            webhookManager.ShowDialog();
        }
    }
}
