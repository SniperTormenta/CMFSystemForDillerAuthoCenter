using CMFSystemForDillerAuthoCenter.Models;
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
    public partial class ViewEmailWindow : Window
    {
        private EmailMessage _email;

        public ViewEmailWindow(EmailMessage email)
        {
            InitializeComponent();
            _email = email;
            DataContext = _email;
        }

        private void DownloadAttachmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string filePath)
            {
                var saveFileDialog = new SaveFileDialog
                {
                    FileName = System.IO.Path.GetFileName(filePath),
                    Filter = "All files (*.*)|*.*"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.File.Copy(filePath, saveFileDialog.FileName, true);
                    MessageBox.Show("Вложение сохранено.");
                }
            }
        }
    }
}