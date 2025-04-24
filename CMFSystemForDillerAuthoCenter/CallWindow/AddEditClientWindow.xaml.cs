using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class AddEditClientWindow : Window, INotifyPropertyChanged
    {
        private ClientStorage _storage;
        private EmployeeStorage _employeeStorage;
        private Client _client;
        private bool _isIndividual;
        private bool _isLegalEntity;

        public bool IsIndividual
        {
            get => _isIndividual;
            set
            {
                _isIndividual = value;
                IsLegalEntity = !_isIndividual; // Инвертируем значение
                OnPropertyChanged(nameof(IsIndividual));
            }
        }

        public bool IsLegalEntity
        {
            get => _isLegalEntity;
            set
            {
                _isLegalEntity = value;
                OnPropertyChanged(nameof(IsLegalEntity));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AddEditClientWindow(ClientStorage storage, EmployeeStorage employeeStorage, Client clientToEdit = null)
        {
            InitializeComponent();
            _storage = storage;
            _employeeStorage = employeeStorage;
            _client = clientToEdit ?? new Client
            {
                Id = $"C{(_storage.Clients.Count + 1):D03}",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            ResponsibleComboBox.ItemsSource = _employeeStorage.Employees.Select(e => e.FullName).ToList();

            if (clientToEdit != null)
            {
                if (clientToEdit.Type == "Юрлицо")
                {
                    LegalEntityRadioButton.IsChecked = true;
                    IsIndividual = false;
                    CompanyNameTextBox.Text = clientToEdit.CompanyName;
                }
                else
                {
                    IndividualRadioButton.IsChecked = true;
                    IsIndividual = true;
                    FirstNameTextBox.Text = clientToEdit.FirstName;
                    LastNameTextBox.Text = clientToEdit.LastName;
                    MiddleNameTextBox.Text = clientToEdit.MiddleName;
                    GenderComboBox.SelectedItem = GenderComboBox.Items.Cast<ComboBoxItem>()
                        .FirstOrDefault(i => i.Content.ToString() == clientToEdit.Gender);
                }

                EmailTextBox.Text = clientToEdit.Email;
                PhoneTextBox.Text = clientToEdit.Phone;
                ContactPersonsItemsControl.ItemsSource = clientToEdit.ContactPersons;
                CategoryComboBox.SelectedItem = CategoryComboBox.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString() == clientToEdit.Category);
                TagTextBox.Text = clientToEdit.Tag;
                NotesTextBox.Text = clientToEdit.Notes;
                ResponsibleComboBox.SelectedItem = clientToEdit.Responsible;
            }
            else
            {
                IsIndividual = true;
            }

            DataContext = this;
        }

        private void ClientTypeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            IsIndividual = IndividualRadioButton.IsChecked == true;
        }

        private void AddContactPersonButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddContactPersonWindow();
            addWindow.Owner = this;
            if (addWindow.ShowDialog() == true)
            {
                _client.ContactPersons.Add(addWindow.GetContactPerson());
                ContactPersonsItemsControl.ItemsSource = null;
                ContactPersonsItemsControl.ItemsSource = _client.ContactPersons;
            }
        }

        private void EditContactPersonButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ContactPerson contactPerson)
            {
                var editWindow = new AddContactPersonWindow(contactPerson);
                editWindow.Owner = this;
                if (editWindow.ShowDialog() == true)
                {
                    ContactPersonsItemsControl.ItemsSource = null;
                    ContactPersonsItemsControl.ItemsSource = _client.ContactPersons;
                }
            }
        }

        private void DeleteContactPersonButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ContactPerson contactPerson)
            {
                if (MessageBox.Show($"Удалить контактное лицо {contactPerson.FullName}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _client.ContactPersons.Remove(contactPerson);
                    ContactPersonsItemsControl.ItemsSource = null;
                    ContactPersonsItemsControl.ItemsSource = _client.ContactPersons;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                CategoryComboBox.SelectedItem == null || ResponsibleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля.");
                return;
            }

            _client.Type = IndividualRadioButton.IsChecked == true ? "Физлицо" : "Юрлицо";
            if (_client.Type == "Физлицо")
            {
                if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) || string.IsNullOrWhiteSpace(LastNameTextBox.Text))
                {
                    MessageBox.Show("Укажите имя и фамилию для физлица.");
                    return;
                }
                _client.FirstName = FirstNameTextBox.Text;
                _client.LastName = LastNameTextBox.Text;
                _client.MiddleName = MiddleNameTextBox.Text;
                _client.Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                _client.CompanyName = null;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(CompanyNameTextBox.Text))
                {
                    MessageBox.Show("Укажите название компании для юрлица.");
                    return;
                }
                _client.CompanyName = CompanyNameTextBox.Text;
                _client.FirstName = null;
                _client.LastName = null;
                _client.MiddleName = null;
                _client.Gender = null;
            }

            _client.Email = EmailTextBox.Text;
            _client.Phone = PhoneTextBox.Text;
            _client.Category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            _client.Tag = TagTextBox.Text;
            _client.Notes = NotesTextBox.Text;
            _client.Responsible = ResponsibleComboBox.SelectedItem?.ToString();
            _client.ModifiedDate = DateTime.Now;

            if (!_storage.Clients.Contains(_client))
            {
                _storage.Clients.Add(_client);
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