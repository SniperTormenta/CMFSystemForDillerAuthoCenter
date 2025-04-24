using CMFSystemForDillerAuthoCenter.CallWindow;
using CMFSystemForDillerAuthoCenter.Models;
using CMFSystemForDillerAuthoCenter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CMFSystemForDillerAuthoCenter.Windows
{
    public partial class EmailWindow : Window
    {
        private EmailService _emailService;
        private EmailStorage _storage;

        public EmailWindow()
        {
            InitializeComponent();
            _emailService = new EmailService();
            _storage = _emailService.GetStorage();
            DataContext = _storage;
            LoadEmailsAsync();
        }

        private async void LoadEmailsAsync()
        {
            await _emailService.FetchEmailsAsync();
            InboxDataGrid.Items.Refresh();
            SentDataGrid.Items.Refresh();
        }

        private void NewEmailButton_Click(object sender, RoutedEventArgs e)
        {
            var composeWindow = new ComposeEmailWindow(_emailService);
            composeWindow.Owner = this;
            if (composeWindow.ShowDialog() == true)
            {
                SentDataGrid.Items.Refresh();
            }
        }

        private void NewBulkEmailButton_Click(object sender, RoutedEventArgs e)
        {
            var bulkWindow = new BulkEmailWindow(_emailService);
            bulkWindow.Owner = this;
            if (bulkWindow.ShowDialog() == true)
            {
                SentDataGrid.Items.Refresh();
            }
        }

        private async void RefreshInboxButton_Click(object sender, RoutedEventArgs e)
        {
            await _emailService.FetchEmailsAsync();
            InboxDataGrid.Items.Refresh();
        }

        private void EmailDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid grid && grid.SelectedItem is EmailMessage email)
            {
                _emailService.MarkAsRead(email.Id);
                var viewWindow = new ViewEmailWindow(email);
                viewWindow.Owner = this;
                viewWindow.ShowDialog();
                InboxDataGrid.Items.Refresh();
                SentDataGrid.Items.Refresh();
            }
        }
    }
}