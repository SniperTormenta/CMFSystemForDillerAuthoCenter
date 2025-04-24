using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class AddEditEmployeeWindow : Window
    {
        private EmployeeStorage _storage;
        private Employee _employee;

        public AddEditEmployeeWindow(EmployeeStorage storage, Employee employeeToEdit = null)
        {
            InitializeComponent();
            _storage = storage;
            _employee = employeeToEdit ?? new Employee { Id = $"E{(_storage.Employees.Count + 1):D03}" };

            if (employeeToEdit != null)
            {
                FullNameTextBox.Text = employeeToEdit.FullName;
                PositionComboBox.SelectedItem = PositionComboBox.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString() == employeeToEdit.Position);
                PhoneTextBox.Text = employeeToEdit.Phone;
                EmailTextBox.Text = employeeToEdit.Email;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                PositionComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            _employee.FullName = FullNameTextBox.Text;
            _employee.Position = (PositionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            _employee.Phone = PhoneTextBox.Text;
            _employee.Email = EmailTextBox.Text;

            if (!_storage.Employees.Contains(_employee))
            {
                _storage.Employees.Add(_employee);
            }
            if (!Regex.IsMatch(PhoneTextBox.Text, @"^\d{11}$"))
            {
                MessageBox.Show("Телефон должен содержать 11 цифр.");
                return;
            }
            if (!Regex.IsMatch(EmailTextBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Некорректный формат email.");
                return;
            }

            _storage.Save();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}