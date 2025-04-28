using CMFSystemForDillerAuthoCenter.Models;
using CMFSystemForDillerAuthoCenter.Services;
using Microsoft.Win32;
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

namespace CMFSystemForDillerAuthoCenter.CallWindow
{
    public partial class ComposeEmailWindow : Window
    {
        private EmailService _emailService;
        private List<string> _attachments;

        public ComposeEmailWindow(EmailService emailService)
        {
            InitializeComponent();
            _emailService = emailService;
            _attachments = new List<string>();
        }

        public void AddAttachment(string filePath)
        {
            _attachments.Add(filePath);
            AttachmentsTextBlock.Text = $"Вложения: {string.Join(", ", _attachments.Select(System.IO.Path.GetFileName))}";
        }

        private void AddAttachmentButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Multiselect = true };
            if (openFileDialog.ShowDialog() == true)
            {
                _attachments.AddRange(openFileDialog.FileNames);
                AttachmentsTextBlock.Text = $"Вложения: {string.Join(", ", openFileDialog.SafeFileNames)}";
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ToTextBox.Text) || string.IsNullOrWhiteSpace(SubjectTextBox.Text) || string.IsNullOrWhiteSpace(BodyTextBox.Text))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            var email = new EmailMessage
            {
                Id = Guid.NewGuid().ToString(),
                Sender = "galochkin666@gmail.com",
                Recipients = new List<string> { ToTextBox.Text },
                Subject = SubjectTextBox.Text,
                Body = BodyTextBox.Text,
                Attachments = _attachments,
                IsRead = false,
                IsSent = false,
                IsDraft = false
            };

            try
            {
                await _emailService.SendEmailAsync(email);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}