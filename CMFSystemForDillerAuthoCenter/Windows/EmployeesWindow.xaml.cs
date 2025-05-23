﻿using CMFSystemForDillerAuthoCenter.CallWindow;
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
    public partial class EmployeesWindow : Window
    {
        private EmployeeStorage _storage;
        private List<Employee> _filteredEmployees;
        private ClientStorage _clientStorage;
        private DealData _dealData;
        private EmployeeStorage _employeeStorage;
        private EmailService _emailService;

        public List<Employee> FilteredEmployees
        {
            get { return _filteredEmployees; }
            set { _filteredEmployees = value; }
        }

        public EmployeesWindow()
        {
            InitializeComponent();
            _storage = EmployeeStorage.Load();
            _filteredEmployees = _storage.Employees.ToList();
            DataContext = this;
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditEmployeeWindow(_storage);
            addWindow.Owner = this;
            if (addWindow.ShowDialog() == true)
            {
                UpdateFilteredEmployees();
                EmployeesDataGrid.Items.Refresh();
            }
        }

        private void EmployeesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is Employee selectedEmployee)
            {
                var editWindow = new AddEditEmployeeWindow(_storage, selectedEmployee);
                editWindow.Owner = this;
                if (editWindow.ShowDialog() == true)
                {
                    UpdateFilteredEmployees();
                    EmployeesDataGrid.Items.Refresh();
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFilteredEmployees();
            EmployeesDataGrid.Items.Refresh();
        }

        private void PositionFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFilteredEmployees();
            EmployeesDataGrid.Items.Refresh();
        }

        private void UpdateFilteredEmployees()
        {
            var searchText = SearchTextBox.Text.ToLower();
            var selectedPosition = (PositionFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            _filteredEmployees = _storage.Employees.Where(emp =>
                (string.IsNullOrEmpty(searchText) ||
                 emp.FullName.ToLower().Contains(searchText) ||
                 emp.Phone.ToLower().Contains(searchText) ||
                 emp.Email.ToLower().Contains(searchText)) &&
                (selectedPosition == "Все должности" || selectedPosition == null || emp.Position == selectedPosition)
            ).ToList();
        }
        private void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Employee employee)
            {
                if (MessageBox.Show($"Удалить сотрудника {employee.FullName}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _storage.Employees.Remove(employee);
                    _storage.Save();
                    UpdateFilteredEmployees();
                    EmployeesDataGrid.Items.Refresh();
                }
            }
        }
        private void MainWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void EmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            var employeesWindow = new EmployeesWindow();
            employeesWindow.Show();
            Close();
        }
        private void SkadButton_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            var warehouseWindow = new WarehouseWindow();
            warehouseWindow.Show();
            Close();
        }

        private void GoToWarehouseButton_Click(object sender, RoutedEventArgs e)
        {
            var warehouseWindow = new WarehouseWindow();
            warehouseWindow.Show();
            Close();
        }

        private void NewDealsButton_Click(object sender, RoutedEventArgs e)
        {
            var newDealsWindow = new NewDealsWindow(DataStorage.CarData, _clientStorage);
            newDealsWindow.Show();
            Close();
        }

        private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            var emailwindow = new EmailWindow();
            emailwindow.Show();
        }

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            var clientsWindow = new ClientsWindow();
            clientsWindow.Show();
            Close();
        }

        private void CalendareButton_Click(object sender, RoutedEventArgs e)
        {
            var reportWindow = new ReportWindow(_dealData, _clientStorage, _employeeStorage, _emailService)
            {
                Owner = this
            };
            reportWindow.ShowDialog();
        }

    }
}