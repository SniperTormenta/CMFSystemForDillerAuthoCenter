using CMFSystemForDillerAuthoCenter;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CMFSystemForDillerAuthoCenter.CallWindow
{
    public partial class AddEditDealWindow : Window
    {
        private DealData dealData;
        private CarData carData;
        private ClientStorage clientStorage;
        private EmployeeStorage employeeStorage;
        private Deal deal;
        private string initialType;
        private DealData dealData1;
        private ClientStorage client;

        public DealData DealData { get; }
        public DealData DealData1 { get; }

        public AddEditDealWindow(DealData dealData, CarData carData, ClientStorage clientStorage, EmployeeStorage employeeStorage, Deal dealToEdit = null, string defaultType = null)
        {
            InitializeComponent();
            this.dealData = DataStorage.DealData;
            this.carData = DataStorage.CarData;
            this.clientStorage = clientStorage;
            this.employeeStorage = employeeStorage;
            this.deal = dealToEdit ?? new Deal { Id = $"D{(dealData?.Deals?.Count ?? 0) + 1:D03}" };

            System.Diagnostics.Debug.WriteLine($"AddEditDealWindow: carData содержит {carData?.Cars?.Count ?? 0} автомобилей.");
            System.Diagnostics.Debug.WriteLine($"AddEditDealWindow: dealData содержит {dealData?.Deals?.Count ?? 0} сделок перед редактированием.");

            InitializeCarComboBox();
            InitializeClientComboBox();
            InitializeServicedByComboBox(); // Инициализация списка сотрудников
            InitializeForm(defaultType);

            if (dealToEdit != null)
            {
                PopulateFields();
            }
        }

        public AddEditDealWindow(DealData dealData1, ClientStorage client, EmployeeStorage employeeStorage, Deal deal)
        {
            this.dealData1 = dealData1;
            this.client = client;
            this.employeeStorage = employeeStorage;
            this.deal = deal;
        }

        private void InitializeCarComboBox()
        {
            if (carData == null || !carData.Cars.Any())
            {
                MessageBox.Show("Список автомобилей пуст. Добавьте автомобили в склад перед созданием заказа.");
                CarComboBox.IsEnabled = false;
                return;
            }

            CarComboBox.Items.Clear();
            foreach (var car in carData.Cars)
            {
                CarComboBox.Items.Add(new ComboBoxItem
                {
                    Content = $"{car.Id}: {car.Brand} {car.Model} ({car.Year})",
                    Tag = car.Id
                });
            }
        }

        private void InitializeClientComboBox()
        {
            ClientComboBox.ItemsSource = clientStorage.Clients;
            ClientComboBox.DisplayMemberPath = "ClientName";
        }

        private void InitializeServicedByComboBox()
        {
            ServicedByComboBox.ItemsSource = employeeStorage.Employees;
            ServicedByComboBox.DisplayMemberPath = "FullName";
        }

        private void InitializeForm(string defaultType)
        {
            if (defaultType != null)
            {
                TypeComboBox.SelectedItem = TypeComboBox.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString() == defaultType);
                TypeComboBox.IsEnabled = false;
            }

            initialType = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            UpdateFormVisibility();
        }

        private void PopulateFields()
        {
            TypeComboBox.SelectedItem = TypeComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == deal.Type);
            ClientComboBox.SelectedItem = clientStorage.Clients.FirstOrDefault(c => c.Id == deal.ClientId);
            ClientNameTextBox.Text = deal.ClientName;
            ClientPhoneTextBox.Text = deal.ClientPhone;
            ClientEmailTextBox.Text = deal.ClientEmail;
            if (DateTime.TryParse(deal.Date, out DateTime date))
            {
                DatePicker.SelectedDate = date;
            }
            ServicedByComboBox.SelectedItem = employeeStorage.Employees.FirstOrDefault(e => e.Id == deal.ServicedBy);
            ThemeTextBox.Text = deal.Theme;
            NotesTextBox.Text = deal.Notes;

            if (deal.Type == "Обращение")
            {
                AppealStatusComboBox.SelectedItem = AppealStatusComboBox.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString() == deal.Status);
            }
            else
            {
                OrderStatusComboBox.SelectedItem = OrderStatusComboBox.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString() == deal.Status);

                CarComboBox.SelectedItem = CarComboBox.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Tag.ToString() == deal.CarId);

                UpdateCarImage();

                AmountTextBox.Text = deal.Amount.ToString();
                PaymentTermsComboBox.SelectedItem = PaymentTermsComboBox.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString() == deal.PaymentTerms);
                DeliveryCheckBox.IsChecked = deal.IsDeliveryRequired;
                if (deal.IsDeliveryRequired)
                {
                    DeliveryDateLabel.Visibility = Visibility.Visible;
                    DeliveryDatePicker.Visibility = Visibility.Visible;
                    DeliveryAddressLabel.Visibility = Visibility.Visible;
                    DeliveryAddressTextBox.Visibility = Visibility.Visible;
                    if (DateTime.TryParse(deal.DeliveryDate, out DateTime deliveryDate))
                    {
                        DeliveryDatePicker.SelectedDate = deliveryDate;
                    }
                    DeliveryAddressTextBox.Text = deal.DeliveryAddress;
                }
            }
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFormVisibility();
        }

        private void UpdateFormVisibility()
        {
            var selectedType = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            ClientSelectionPanel.Visibility = selectedType == "Заказ" ? Visibility.Visible : Visibility.Collapsed;
            if (selectedType != "Заказ")
            {
                ClientComboBox.SelectedItem = null;
            }

            if (selectedType == "Обращение")
            {
                ThemeLabel.Visibility = Visibility.Visible;
                ThemeTextBox.Visibility = Visibility.Visible;
                AppealStatusLabel.Visibility = Visibility.Visible;
                AppealStatusComboBox.Visibility = Visibility.Visible;
                NotesLabel.Visibility = Visibility.Visible;
                NotesTextBox.Visibility = Visibility.Visible;

                CarLabel.Visibility = Visibility.Collapsed;
                CarComboBox.Visibility = Visibility.Collapsed;
                CarImage.Visibility = Visibility.Collapsed;
                AmountLabel.Visibility = Visibility.Collapsed;
                AmountTextBox.Visibility = Visibility.Collapsed;
                PaymentTermsLabel.Visibility = Visibility.Collapsed;
                PaymentTermsComboBox.Visibility = Visibility.Collapsed;
                DeliveryCheckBox.Visibility = Visibility.Collapsed;
                DeliveryDateLabel.Visibility = Visibility.Collapsed;
                DeliveryDatePicker.Visibility = Visibility.Collapsed;
                DeliveryAddressLabel.Visibility = Visibility.Collapsed;
                DeliveryAddressTextBox.Visibility = Visibility.Collapsed;
                PaymentStatusLabel.Visibility = Visibility.Collapsed;
                PaymentStatusComboBox.Visibility = Visibility.Collapsed;
                OrderStatusLabel.Visibility = Visibility.Collapsed;
                OrderStatusComboBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                ThemeLabel.Visibility = Visibility.Collapsed;
                ThemeTextBox.Visibility = Visibility.Collapsed;
                AppealStatusLabel.Visibility = Visibility.Collapsed;
                AppealStatusComboBox.Visibility = Visibility.Collapsed;
                NotesLabel.Visibility = Visibility.Collapsed;
                NotesTextBox.Visibility = Visibility.Collapsed;

                CarLabel.Visibility = Visibility.Visible;
                CarComboBox.Visibility = Visibility.Visible;
                CarImage.Visibility = CarComboBox.SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;
                if (carData != null && carData.Cars.Any())
                {
                    CarComboBox.IsEnabled = true;
                }
                else
                {
                    CarComboBox.IsEnabled = false;
                }
                AmountLabel.Visibility = Visibility.Visible;
                AmountTextBox.Visibility = Visibility.Visible;
                PaymentTermsLabel.Visibility = Visibility.Visible;
                PaymentTermsComboBox.Visibility = Visibility.Visible;
                DeliveryCheckBox.Visibility = Visibility.Visible;
                PaymentStatusLabel.Visibility = Visibility.Visible;
                PaymentStatusComboBox.Visibility = Visibility.Visible;
                OrderStatusLabel.Visibility = Visibility.Visible;
                OrderStatusComboBox.Visibility = Visibility.Visible;

                if (DeliveryCheckBox.IsChecked == true)
                {
                    DeliveryDateLabel.Visibility = Visibility.Visible;
                    DeliveryDatePicker.Visibility = Visibility.Visible;
                    DeliveryAddressLabel.Visibility = Visibility.Visible;
                    DeliveryAddressTextBox.Visibility = Visibility.Visible;
                }
                else
                {
                    DeliveryDateLabel.Visibility = Visibility.Collapsed;
                    DeliveryDatePicker.Visibility = Visibility.Collapsed;
                    DeliveryAddressLabel.Visibility = Visibility.Collapsed;
                    DeliveryAddressTextBox.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientComboBox.SelectedItem is Client selectedClient)
            {
                ClientNameTextBox.Text = selectedClient.ClientName;
                ClientPhoneTextBox.Text = selectedClient.Phone;
                ClientEmailTextBox.Text = selectedClient.Email;
            }
        }

        private void CarComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCarImage();
        }

        private void UpdateCarImage()
        {
            if (CarComboBox.SelectedItem != null)
            {
                var selectedCarId = (CarComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();
                var selectedCar = carData?.Cars.FirstOrDefault(c => c.Id == selectedCarId);
                if (selectedCar != null && !string.IsNullOrEmpty(selectedCar.PhotoPath))
                {
                    try
                    {
                        string basePath = AppDomain.CurrentDomain.BaseDirectory;
                        string absolutePath = System.IO.Path.Combine(basePath, selectedCar.PhotoPath);
                        CarImage.Source = new BitmapImage(new Uri(absolutePath, UriKind.Absolute));
                        CarImage.Visibility = Visibility.Visible;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                        CarImage.Source = null;
                        CarImage.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    CarImage.Source = null;
                    CarImage.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                CarImage.Source = null;
                CarImage.Visibility = Visibility.Collapsed;
            }
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            var addClientWindow = new AddEditClientWindow(clientStorage, employeeStorage, null);
            addClientWindow.Owner = this;
            if (addClientWindow.ShowDialog() == true)
            {
                ClientComboBox.ItemsSource = null;
                ClientComboBox.ItemsSource = clientStorage.Clients;
                ClientComboBox.SelectedItem = clientStorage.Clients.Last();
            }
        }

        private void DeliveryCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DeliveryDateLabel.Visibility = Visibility.Visible;
            DeliveryDatePicker.Visibility = Visibility.Visible;
            DeliveryAddressLabel.Visibility = Visibility.Visible;
            DeliveryAddressTextBox.Visibility = Visibility.Visible;
        }

        private void DeliveryCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DeliveryDateLabel.Visibility = Visibility.Collapsed;
            DeliveryDatePicker.Visibility = Visibility.Collapsed;
            DeliveryAddressLabel.Visibility = Visibility.Collapsed;
            DeliveryAddressTextBox.Visibility = Visibility.Collapsed;
        }

        private void ClientPhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9+]");
        }

        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ClientNameTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, укажите ФИО клиента.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(ClientPhoneTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, укажите телефон клиента.");
                    return;
                }

                string phone = ClientPhoneTextBox.Text.Replace("+", "").Trim();
                if (!Regex.IsMatch(phone, @"^\d{11}$"))
                {
                    MessageBox.Show("Телефон должен содержать ровно 11 цифр (например, +79991234567 или 79991234567).");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(ClientEmailTextBox.Text) &&
                    !Regex.IsMatch(ClientEmailTextBox.Text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                {
                    MessageBox.Show("Email должен быть в формате example@domain.com");
                    return;
                }

                if (!DatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Пожалуйста, выберите дату.");
                    return;
                }

                if (DatePicker.SelectedDate > DateTime.Now.AddYears(1))
                {
                    MessageBox.Show("Дата не может быть больше текущей более чем на год.");
                    return;
                }

                if (ServicedByComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите обслуживающего сотрудника.");
                    return;
                }

                deal.Type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                deal.ClientId = (ClientComboBox.SelectedItem as Client)?.Id;
                deal.ClientName = ClientNameTextBox.Text;
                deal.ClientPhone = ClientPhoneTextBox.Text;
                deal.ClientEmail = ClientEmailTextBox.Text;
                deal.Date = DatePicker.SelectedDate.Value.ToString("dd.MM.yyyy");
                deal.Notes = NotesTextBox.Text;
                deal.ServicedBy = (ServicedByComboBox.SelectedItem as Employee)?.Id;

                if (deal.Type == "Обращение")
                {
                    if (string.IsNullOrWhiteSpace(ThemeTextBox.Text))
                    {
                        MessageBox.Show("Пожалуйста, укажите тему обращения.");
                        return;
                    }
                    if (ThemeTextBox.Text.Length > 100)
                    {
                        MessageBox.Show("Тема обращения не должна превышать 100 символов.");
                        return;
                    }
                    if (!string.IsNullOrEmpty(NotesTextBox.Text) && NotesTextBox.Text.Length > 500)
                    {
                        MessageBox.Show("Подробности не должны превышать 500 символов.");
                        return;
                    }
                    deal.Theme = ThemeTextBox.Text;
                    deal.Status = (AppealStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                }
                else
                {
                    if (CarComboBox.SelectedItem == null)
                    {
                        MessageBox.Show("Пожалуйста, выберите автомобиль.");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(AmountTextBox.Text) || !decimal.TryParse(AmountTextBox.Text, out decimal amount) || amount <= 0)
                    {
                        MessageBox.Show("Пожалуйста, укажите корректную сумму сделки (положительное число).");
                        return;
                    }

                    if (PaymentStatusComboBox.SelectedItem == null || OrderStatusComboBox.SelectedItem == null)
                    {
                        MessageBox.Show("Пожалуйста, выберите статус оплаты и статус выполнения.");
                        return;
                    }

                    if (DeliveryCheckBox.IsChecked == true)
                    {
                        if (string.IsNullOrWhiteSpace(DeliveryAddressTextBox.Text))
                        {
                            MessageBox.Show("Укажите адрес доставки.");
                            return;
                        }
                        if (DeliveryDatePicker.SelectedDate == null)
                        {
                            MessageBox.Show("Укажите дату доставки.");
                            return;
                        }
                        if (DeliveryDatePicker.SelectedDate < DateTime.Now)
                        {
                            MessageBox.Show("Дата доставки не может быть в прошлом.");
                            return;
                        }
                    }

                    deal.CarId = (CarComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();
                    deal.Amount = amount;
                    deal.PaymentTerms = (PaymentTermsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    deal.IsDeliveryRequired = DeliveryCheckBox.IsChecked ?? false;
                    if (deal.IsDeliveryRequired)
                    {
                        deal.DeliveryDate = DeliveryDatePicker.SelectedDate?.ToString("dd.MM.yyyy");
                        deal.DeliveryAddress = DeliveryAddressTextBox.Text;
                    }
                    else
                    {
                        deal.DeliveryDate = null;
                        deal.DeliveryAddress = null;
                    }
                    deal.Status = $"{(PaymentStatusComboBox.SelectedItem as ComboBoxItem)?.Content} / {(OrderStatusComboBox.SelectedItem as ComboBoxItem)?.Content}";
                }

                if (deal.CreatedDate == default)
                {
                    deal.CreatedDate = DateTime.Now;
                }
                deal.ModifiedDate = DateTime.Now;

                if (!dealData.Deals.Contains(deal))
                {
                    dealData.Deals.Add(deal);
                }

                DataStorage.SaveDeals();

                System.Diagnostics.Debug.WriteLine($"AddEditDealWindow: После сохранения dealData содержит {dealData?.Deals?.Count ?? 0} сделок.");

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}