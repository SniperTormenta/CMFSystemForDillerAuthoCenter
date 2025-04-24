using CMFSystemForDillerAuthoCenter.CallWindow;
using System;
using CMFSystemForDillerAuthoCenter.Models;
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
    public partial class ClientsWindow : Window
    {
        private ClientStorage _storage;
        private EmployeeStorage _employeeStorage;
        private List<Client> _filteredClients;
        private ClientFilter _currentFilter;

        public List<Client> FilteredClients
        {
            get { return _filteredClients; }
            set { _filteredClients = value; }
        }

        public ClientsWindow()
        {
            InitializeComponent();
            _storage = ClientStorage.Load();
            _employeeStorage = EmployeeStorage.Load();
            _filteredClients = _storage.Clients.ToList();
            _currentFilter = new ClientFilter();

            ResponsibleFilterComboBox.ItemsSource = new List<string> { "Все ответственные" }.Concat(_employeeStorage.Employees.Select(e => e.FullName)).ToList();
            TagFilterComboBox.ItemsSource = new List<string> { "Все метки" }.Concat(_storage.Clients.Select(c => c.Tag).Distinct().Where(t => !string.IsNullOrEmpty(t))).ToList();
            SavedFiltersItemsControl.ItemsSource = _storage.SavedFilters;

            DataContext = this;
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditClientWindow(_storage, _employeeStorage);
            addWindow.Owner = this;
            if (addWindow.ShowDialog() == true)
            {
                UpdateFilteredClients();
                ClientsDataGrid.Items.Refresh();
                UpdateTagFilterComboBox();
            }
        }

        private void ClientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client selectedClient)
            {
                var editWindow = new AddEditClientWindow(_storage, _employeeStorage, selectedClient);
                editWindow.Owner = this;
                if (editWindow.ShowDialog() == true)
                {
                    UpdateFilteredClients();
                    ClientsDataGrid.Items.Refresh();
                    UpdateTagFilterComboBox();
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFilteredClients();
            ClientsDataGrid.Items.Refresh();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterPanel.Visibility = FilterPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            _currentFilter.SortBy = (SortByComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            _currentFilter.DateFilter = DateFilterPicker.SelectedDate;
            _currentFilter.TypeFilter = (TypeFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            _currentFilter.CategoryFilter = (CategoryFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            _currentFilter.ResponsibleFilter = ResponsibleFilterComboBox.SelectedItem?.ToString();
            _currentFilter.TagFilter = TagFilterComboBox.SelectedItem?.ToString();

            UpdateFilteredClients();
            ClientsDataGrid.Items.Refresh();
            UpdateFilterUI();
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            _currentFilter = new ClientFilter();
            SortByComboBox.SelectedIndex = -1;
            DateFilterPicker.SelectedDate = null;
            TypeFilterComboBox.SelectedItem = TypeFilterComboBox.Items.Cast<ComboBoxItem>().First(i => i.Content.ToString() == "Все типы");
            CategoryFilterComboBox.SelectedItem = CategoryFilterComboBox.Items.Cast<ComboBoxItem>().First(i => i.Content.ToString() == "Все категории");
            ResponsibleFilterComboBox.SelectedItem = ResponsibleFilterComboBox.Items.Cast<string>().First(i => i == "Все ответственные");
            TagFilterComboBox.SelectedItem = TagFilterComboBox.Items.Cast<string>().First(i => i == "Все метки");

            UpdateFilteredClients();
            ClientsDataGrid.Items.Refresh();
            UpdateFilterUI();
        }

        private void ApplySavedFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ClientFilter filter)
            {
                _currentFilter = filter;
                SortByComboBox.SelectedItem = SortByComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == filter.SortBy);
                DateFilterPicker.SelectedDate = filter.DateFilter;
                TypeFilterComboBox.SelectedItem = TypeFilterComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == filter.TypeFilter) ??
                    TypeFilterComboBox.Items.Cast<ComboBoxItem>().First(i => i.Content.ToString() == "Все типы");
                CategoryFilterComboBox.SelectedItem = CategoryFilterComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == filter.CategoryFilter) ??
                    CategoryFilterComboBox.Items.Cast<ComboBoxItem>().First(i => i.Content.ToString() == "Все категории");
                ResponsibleFilterComboBox.SelectedItem = ResponsibleFilterComboBox.Items.Cast<string>().FirstOrDefault(i => i == filter.ResponsibleFilter) ??
                    ResponsibleFilterComboBox.Items.Cast<string>().First(i => i == "Все ответственные");
                TagFilterComboBox.SelectedItem = TagFilterComboBox.Items.Cast<string>().FirstOrDefault(i => i == filter.TagFilter) ??
                    TagFilterComboBox.Items.Cast<string>().First(i => i == "Все метки");

                UpdateFilteredClients();
                ClientsDataGrid.Items.Refresh();
                UpdateFilterUI();
            }
        }

        private void DeleteSavedFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ClientFilter filter)
            {
                _storage.SavedFilters.Remove(filter);
                _storage.Save();
                SavedFiltersItemsControl.ItemsSource = null;
                SavedFiltersItemsControl.ItemsSource = _storage.SavedFilters;
            }
        }

        private void SaveFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FilterNameTextBox.Text))
            {
                MessageBox.Show("Укажите имя фильтра.");
                return;
            }

            _currentFilter.Name = FilterNameTextBox.Text;
            _storage.SavedFilters.Add(_currentFilter);
            _storage.Save();
            SavedFiltersItemsControl.ItemsSource = null;
            SavedFiltersItemsControl.ItemsSource = _storage.SavedFilters;
            FilterNameTextBox.Text = string.Empty;
            SaveFilterPanel.Visibility = Visibility.Collapsed;
        }

        private void GroupActionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedAction = (GroupActionsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(selectedAction) || ClientsDataGrid.SelectedItems.Count == 0)
                return;

            if (selectedAction == "Удалить")
            {
                if (MessageBox.Show($"Удалить {ClientsDataGrid.SelectedItems.Count} клиентов?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    foreach (Client client in ClientsDataGrid.SelectedItems.Cast<Client>().ToList())
                    {
                        _storage.Clients.Remove(client);
                    }
                    _storage.Save();
                    UpdateFilteredClients();
                    ClientsDataGrid.Items.Refresh();
                    UpdateTagFilterComboBox();
                }
            }
            else if (selectedAction == "Изменить категорию")
            {
                var categoryWindow = new ComboBox { ItemsSource = new[] { "Клиент", "Конкурент", "Партнёр" } };
                var dialog = new Window
                {
                    Title = "Выберите категорию",
                    Content = categoryWindow,
                    Width = 200,
                    Height = 100,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this
                };
                dialog.ShowDialog();

                if (categoryWindow.SelectedItem != null)
                {
                    foreach (Client client in ClientsDataGrid.SelectedItems.Cast<Client>().ToList())
                    {
                        client.Category = categoryWindow.SelectedItem.ToString();
                        client.ModifiedDate = DateTime.Now;
                    }
                    _storage.Save();
                    UpdateFilteredClients();
                    ClientsDataGrid.Items.Refresh();
                }
            }

            GroupActionsComboBox.SelectedIndex = -1;
        }

        private void UpdateFilteredClients()
        {
            var searchText = SearchTextBox.Text.ToLower();

            _filteredClients = _storage.Clients.Where(client =>
                (string.IsNullOrEmpty(searchText) ||
                 client.ClientName.ToLower().Contains(searchText) ||
                 client.Phone.ToLower().Contains(searchText) ||
                 client.Email.ToLower().Contains(searchText) ||
                 (client.ContactPersons.Any(cp => cp.FullName.ToLower().Contains(searchText) ||
                                                cp.Phone.ToLower().Contains(searchText) ||
                                                cp.Email.ToLower().Contains(searchText)))) &&
                (_currentFilter.TypeFilter == "Все типы" || _currentFilter.TypeFilter == null || client.Type == _currentFilter.TypeFilter) &&
                (_currentFilter.CategoryFilter == "Все категории" || _currentFilter.CategoryFilter == null || client.Category == _currentFilter.CategoryFilter) &&
                (_currentFilter.ResponsibleFilter == "Все ответственные" || _currentFilter.ResponsibleFilter == null || client.Responsible == _currentFilter.ResponsibleFilter) &&
                (_currentFilter.TagFilter == "Все метки" || _currentFilter.TagFilter == null || client.Tag == _currentFilter.TagFilter) &&
                (_currentFilter.DateFilter == null || client.CreatedDate.Date == _currentFilter.DateFilter.Value.Date)
            ).ToList();

            if (_currentFilter.SortBy != null)
            {
                switch (_currentFilter.SortBy)
                {
                    case "По созданию":
                        _filteredClients.Sort((a, b) => a.CreatedDate.CompareTo(b.CreatedDate));
                        break;
                    case "По изменению":
                        _filteredClients.Sort((a, b) => a.ModifiedDate.CompareTo(b.ModifiedDate));
                        break;
                    case "По названию":
                        _filteredClients.Sort((a, b) => a.ClientName.CompareTo(b.ClientName));
                        break;
                }
            }
        }

        private void UpdateFilterUI()
        {
            int criteriaCount = 0;
            if (_currentFilter.SortBy != null) criteriaCount++;
            if (_currentFilter.DateFilter != null) criteriaCount++;
            if (_currentFilter.TypeFilter != null && _currentFilter.TypeFilter != "Все типы") criteriaCount++;
            if (_currentFilter.CategoryFilter != null && _currentFilter.CategoryFilter != "Все категории") criteriaCount++;
            if (_currentFilter.ResponsibleFilter != null && _currentFilter.ResponsibleFilter != "Все ответственные") criteriaCount++;
            if (_currentFilter.TagFilter != null && _currentFilter.TagFilter != "Все метки") criteriaCount++;

            ResetFilterButton.Visibility = criteriaCount > 0 ? Visibility.Visible : Visibility.Collapsed;
            SaveFilterPanel.Visibility = criteriaCount >= 2 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateTagFilterComboBox()
        {
            TagFilterComboBox.ItemsSource = new List<string> { "Все метки" }.Concat(_storage.Clients.Select(c => c.Tag).Distinct().Where(t => !string.IsNullOrEmpty(t))).ToList();
        }

        private void MainWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void NewDealsButton_Click(object sender, RoutedEventArgs e)
        {
            var newDealsWindow = new NewDealsWindow();
            newDealsWindow.Show();
            Close();
        }

        private void SkadButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var warehouseWindow = new WarehouseWindow();
            warehouseWindow.Show();
            Close();
        }

        private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            var emailWindow = new EmailWindow();
            emailWindow.Show();
            Close();
        }

        private void EmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            var employeesWindow = new EmployeesWindow();
            employeesWindow.Show();
            Close();
        }
    }
}