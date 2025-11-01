using System.Windows.Controls;
using Voidstrap.UI.ViewModels.Settings;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    /// <summary>
    /// Interaction logic for NotificationsPage.xaml
    /// </summary>
    public partial class NotificationsPage
    {
        public NotificationsPage()
        {
            InitializeComponent();
            DataContext = new NotificationsViewModel();
        }
    }
}
