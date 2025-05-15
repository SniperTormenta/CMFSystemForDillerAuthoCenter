using CMFSystemForDillerAuthoCenter.CallWindow;
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
using CMFSystemForDillerAuthoCenter.Services;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CMFSystemForDillerAuthoCenter.Windows
{
    public partial class ClientsWindow : Window, INotifyPropertyChanged
    {
        private ClientStorage _storage;
        private EmployeeStorage _employeeStorage;
        private ObservableCollection<Client> _filteredClients;
        private ClientFilter _currentFilter;
        private DealData _dealData;
        private ClientStorage _clientStorage;
        private EmailService _emailService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Client> FilteredClients
        {
            get { return _filteredClients; }
            set
            {
                _filteredClients = value;
                OnPropertyChanged(nameof(FilteredClients));
                System.Diagnostics.Debug.WriteLine($"FilteredClients обновлен: {_filteredClients?.Count ?? 0} клиентов.");
            }
        }

        public ClientsWindow()
        {
            InitializeComponent();
            _storage = ClientStorage.Load() ?? new ClientStorage();
            _employeeStorage = EmployeeStorage.Load() ?? new EmployeeStorage();
            _dealData = DataStorage.DealData;
            _clientStorage = _storage; // Инициализация для использования в других окнах
            _emailService = new EmailService();
            _filteredClients = new ObservableCollection<Client>(_storage.Clients);
            _currentFilter = new ClientFilter();

            ResponsibleFilterComboBox.ItemsSource = new List<string> { "Все ответственные" }.Concat(_employeeStorage.Employees.Select(e => e.FullName)).ToList();
            TagFilterComboBox.ItemsSource = new List<string> { "Все метки" }.Concat(_storage.Clients.Select(c => c.Tag).Distinct().Where(t => !string.IsNullOrEmpty(t))).ToList();
            SavedFiltersItemsControl.ItemsSource = _storage.SavedFilters;

            DataContext = this;
            ClientsDataGrid.ItemsSource = FilteredClients; // Привязка к FilteredClients
            System.Diagnostics.Debug.WriteLine($"ClientsWindow инициализирован: Загружено {_storage.Clients.Count} клиентов, отображается {_filteredClients.Count}.");
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditClientWindow(_storage, _employeeStorage);
            addWindow.Owner = this;
            if (addWindow.ShowDialog() == true)
            {
                _storage.Save(); // Сохраняем изменения в файл
                UpdateFilteredClients();
                System.Diagnostics.Debug.WriteLine($"AddClient: После добавления клиента отображается {_filteredClients.Count} клиентов.");
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
                    _storage.Save();
                    UpdateFilteredClients();
                    System.Diagnostics.Debug.WriteLine($"EditClient: После редактирования клиента отображается {_filteredClients.Count} клиентов.");
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFilteredClients();
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
            UpdateFilterUI();
            System.Diagnostics.Debug.WriteLine($"ApplyFilter: Применен фильтр, отображается {_filteredClients.Count} клиентов.");
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
            UpdateFilterUI();
            System.Diagnostics.Debug.WriteLine($"ResetFilter: Сброшены фильтры, отображается {_filteredClients.Count} клиентов.");
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
                UpdateFilterUI();
                System.Diagnostics.Debug.WriteLine($"ApplySavedFilter: Применен сохраненный фильтр, отображается {_filteredClients.Count} клиентов.");
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
                System.Diagnostics.Debug.WriteLine($"DeleteSavedFilter: Удален сохраненный фильтр, осталось {_storage.SavedFilters.Count} фильтров.");
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
            System.Diagnostics.Debug.WriteLine($"SaveFilter: Сохранен новый фильтр, всего фильтров: {_storage.SavedFilters.Count}.");
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
                    UpdateTagFilterComboBox();
                    System.Diagnostics.Debug.WriteLine($"GroupAction Delete: Удалено {ClientsDataGrid.SelectedItems.Count} клиентов, осталось {_filteredClients.Count}.");
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
                    System.Diagnostics.Debug.WriteLine($"GroupAction ChangeCategory: Изменена категория для {ClientsDataGrid.SelectedItems.Count} клиентов.");
                }
            }

            GroupActionsComboBox.SelectedIndex = -1;
        }

        private void UpdateFilteredClients()
        {
            var searchText = SearchTextBox.Text.ToLower();

            var filtered = _storage.Clients.Where(client =>
                (string.IsNullOrEmpty(searchText) ||
                 client.ClientName.ToLower().Contains(searchText) ||
                 client.Phone.ToLower().Contains(searchText) ||
                 client.Email.ToLower().Contains(searchText) ||
                 (client.ContactPersons.Any(cp => cp.FullName.ToLower().Contains(searchText) ||
                                                cp.Phone.ToLower().Contains(searchText) ||
                                                cp.Email.ToLower().Contains(searchText)))) &&
                (_currentFilter.TypeFilter == "Все типы" || _currentFilter.TypeFilter == null || client.Type == _currentFilter.TypeFilter) &&
                (_currentFilter.CategoryFilter == "Все категории" || _currentFilter.CategoryFilter == null || client.Category == _currentFilter.CategoryFilter) &&
                (_currentFilter.ResponsibleFilter == "Все ответственные" || _currentFilter.ResponsibleFilter == null ||
                 client.Responsible == _employeeStorage.Employees.FirstOrDefault(e => e.FullName == _currentFilter.ResponsibleFilter)?.Id) &&
                (_currentFilter.TagFilter == "Все метки" || _currentFilter.TagFilter == null || client.Tag == _currentFilter.TagFilter) &&
                (_currentFilter.DateFilter == null || client.CreatedDate.Date == _currentFilter.DateFilter.Value.Date)
            ).ToList();

            if (_currentFilter.SortBy != null)
            {
                switch (_currentFilter.SortBy)
                {
                    case "По созданию":
                        filtered.Sort((a, b) => a.CreatedDate.CompareTo(b.CreatedDate));
                        break;
                    case "По изменению":
                        filtered.Sort((a, b) => a.ModifiedDate.CompareTo(b.CreatedDate));
                        break;
                    case "По названию":
                        filtered.Sort((a, b) => a.ClientName.CompareTo(b.ClientName));
                        break;
                }
            }

            FilteredClients.Clear();
            foreach (var client in filtered)
            {
                FilteredClients.Add(client);
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
            var newDealsWindow = new NewDealsWindow(DataStorage.CarData, _clientStorage, _employeeStorage, _emailService);
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

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}