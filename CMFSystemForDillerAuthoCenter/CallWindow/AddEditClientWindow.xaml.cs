﻿using CMFSystemForDillerAuthoCenter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private ClientStorage _clientStorage;
        private EmployeeStorage _employeeStorage;
        private Client _client;
        private string _clientType;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ClientType
        {
            get => _clientType;
            set
            {
                _clientType = value;
                OnPropertyChanged(nameof(ClientType));
                OnPropertyChanged(nameof(IsIndividual));
                OnPropertyChanged(nameof(IsLegalEntity));
                System.Diagnostics.Debug.WriteLine($"ClientType set to: {ClientType}, IsIndividual: {IsIndividual}, IsLegalEntity: {IsLegalEntity}");
            }
        }

        public bool IsIndividual => ClientType == "Физлицо";
        public bool IsLegalEntity => ClientType == "Юрлицо";

        public AddEditClientWindow(ClientStorage storage, EmployeeStorage employeeStorage, Client clientToEdit = null)
        {
            InitializeComponent();
            DataContext = this;
            _clientStorage = storage ?? ClientStorage.Load() ?? new ClientStorage();
            _employeeStorage = employeeStorage ?? new EmployeeStorage();
            _client = clientToEdit ?? new Client
            {
                Id = $"C{(_clientStorage.Clients.Count + 1):D03}",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            ResponsibleComboBox.ItemsSource = _employeeStorage.Employees;
            ResponsibleComboBox.DisplayMemberPath = "FullName";

            if (clientToEdit != null)
            {
                ClientType = clientToEdit.Type;
                IndividualRadioButton.IsChecked = ClientType == "Физлицо";
                LegalEntityRadioButton.IsChecked = ClientType == "Юрлицо";
                FirstNameTextBox.Text = clientToEdit.FirstName;
                LastNameTextBox.Text = clientToEdit.LastName;
                MiddleNameTextBox.Text = clientToEdit.MiddleName;
                GenderComboBox.SelectedItem = GenderComboBox.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString() == clientToEdit.Gender);
                CompanyNameTextBox.Text = clientToEdit.CompanyName;
                EmailTextBox.Text = clientToEdit.Email;
                PhoneTextBox.Text = clientToEdit.Phone;
                CategoryComboBox.SelectedItem = CategoryComboBox.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString() == clientToEdit.Category);
                TagTextBox.Text = clientToEdit.Tag;
                NotesTextBox.Text = clientToEdit.Notes;
                ResponsibleComboBox.SelectedItem = _employeeStorage.Employees
                    .FirstOrDefault(e => e.Id == clientToEdit.Responsible);
                ContactPersonsItemsControl.ItemsSource = _client.ContactPersons;
            }
            else
            {
                ClientType = "Физлицо";
                IndividualRadioButton.IsChecked = true;
            }
        }

        private void ClientTypeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                ClientType = radioButton == IndividualRadioButton ? "Физлицо" : "Юрлицо";
                System.Diagnostics.Debug.WriteLine($"ClientType: {ClientType}, IsIndividual: {IsIndividual}, IsLegalEntity: {IsLegalEntity}");
            }
        }

        private void AddContactPersonButton_Click(object sender, RoutedEventArgs e)
        {
            var contactPerson = new ContactPerson();
            var editWindow = new AddContactPersonWindow(contactPerson);
            if (editWindow.ShowDialog() == true)
            {
                _client.ContactPersons.Add(contactPerson);
                ContactPersonsItemsControl.ItemsSource = null;
                ContactPersonsItemsControl.ItemsSource = _client.ContactPersons;
            }
        }

        private void EditContactPersonButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ContactPerson contactPerson)
            {
                var editWindow = new AddContactPersonWindow(contactPerson);
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
                _client.ContactPersons.Remove(contactPerson);
                ContactPersonsItemsControl.ItemsSource = null;
                ContactPersonsItemsControl.ItemsSource = _client.ContactPersons;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                CategoryComboBox.SelectedItem == null ||
                ResponsibleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля.");
                return;
            }

            bool isIndividual = ClientType == "Физлицо";

            if (isIndividual)
            {
                if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(LastNameTextBox.Text))
                {
                    MessageBox.Show("Для физлица необходимо указать имя и фамилию.");
                    return;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(CompanyNameTextBox.Text))
                {
                    MessageBox.Show("Для юрлица необходимо указать название компании.");
                    return;
                }
            }

            string phone = PhoneTextBox.Text.Replace("+", "").Trim();
            if (!Regex.IsMatch(phone, @"^\d{11}$"))
            {
                MessageBox.Show("Телефон должен содержать ровно 11 цифр (например, +79991234567 или 79991234567).");
                return;
            }

            if (!Regex.IsMatch(EmailTextBox.Text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
            {
                MessageBox.Show("Email должен быть в формате example@domain.com");
                return;
            }

            if (FirstNameTextBox.Text.Length > 50 ||
                LastNameTextBox.Text.Length > 50 ||
                MiddleNameTextBox.Text.Length > 50 ||
                CompanyNameTextBox.Text.Length > 100 ||
                TagTextBox.Text.Length > 50 ||
                NotesTextBox.Text.Length > 500)
            {
                MessageBox.Show("Превышена максимальная длина одного из полей:\n" +
                                "Имя, Фамилия, Отчество: до 50 символов\n" +
                                "Название компании: до 100 символов\n" +
                                "Метка: до 50 символов\n" +
                                "Заметки: до 500 символов");
                return;
            }

            _client.Type = isIndividual ? "Физлицо" : "Юрлицо";
            _client.FirstName = FirstNameTextBox.Text;
            _client.LastName = LastNameTextBox.Text;
            _client.MiddleName = MiddleNameTextBox.Text;
            _client.Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            _client.CompanyName = CompanyNameTextBox.Text;
            _client.Email = EmailTextBox.Text;
            _client.Phone = PhoneTextBox.Text;
            _client.Category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            _client.Tag = TagTextBox.Text;
            _client.Notes = NotesTextBox.Text;
            _client.Responsible = (ResponsibleComboBox.SelectedItem as Employee)?.Id;
            _client.ModifiedDate = DateTime.Now;

            if (isIndividual)
            {
                _client.ContactPersons.Clear();
            }

            if (!_clientStorage.Clients.Contains(_client))
            {
                _clientStorage.Clients.Add(_client);
            }

            _clientStorage.Save();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}