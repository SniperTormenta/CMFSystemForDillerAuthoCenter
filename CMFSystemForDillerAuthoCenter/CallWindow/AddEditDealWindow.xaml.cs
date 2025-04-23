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
    public partial class AddEditDealWindow : Window
    {
        private DealData dealData;
        private CarData carData;
        private Deal deal;
        private string initialType;
        private decimal amount;

        public AddEditDealWindow(DealData dealData, CarData carData, Deal dealToEdit = null, string defaultType = null)
        {
            InitializeComponent();
            this.dealData = dealData;
            this.carData = carData;
            this.deal = dealToEdit ?? new Deal { Id = $"D{dealData.Deals.Count + 1:D03}" };

            InitializeCarComboBox();
            InitializeForm(defaultType);

            if (dealToEdit != null)
            {
                PopulateFields();
            }
        }

        private void InitializeCarComboBox()
        {
            foreach (var car in carData.Cars)
            {
                CarComboBox.Items.Add($"{car.Id}: {car.Brand} {car.Model}");
            }
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
            ClientNameTextBox.Text = deal.ClientName;
            ClientPhoneTextBox.Text = deal.ClientPhone;
            ClientEmailTextBox.Text = deal.ClientEmail;
            if (DateTime.TryParse(deal.Date, out DateTime date))
            {
                DatePicker.SelectedDate = date;
            }
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
                CarComboBox.SelectedItem = CarComboBox.Items.Cast<string>()
                    .FirstOrDefault(i => i.StartsWith(deal.CarId));
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

            if (selectedType == "Обращение")
            {
                ThemeLabel.Visibility = Visibility.Visible;
                ThemeTextBox.Visibility = Visibility.Visible;
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
                OrderStatusLabel.Visibility = Visibility.Collapsed;
                OrderStatusComboBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                ThemeLabel.Visibility = Visibility.Collapsed;
                ThemeTextBox.Visibility = Visibility.Collapsed;
                AppealStatusComboBox.Visibility = Visibility.Collapsed;
                NotesLabel.Visibility = Visibility.Collapsed;
                NotesTextBox.Visibility = Visibility.Collapsed;

                CarLabel.Visibility = Visibility.Visible;
                CarComboBox.Visibility = Visibility.Visible;
                CarImage.Visibility = Visibility.Visible;
                AmountLabel.Visibility = Visibility.Visible;
                AmountTextBox.Visibility = Visibility.Visible;
                PaymentTermsLabel.Visibility = Visibility.Visible;
                PaymentTermsComboBox.Visibility = Visibility.Visible;
                DeliveryCheckBox.Visibility = Visibility.Visible;
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

        private void CarComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCarImage();
        }

        private void UpdateCarImage()
        {
            if (CarComboBox.SelectedItem != null)
            {
                var selectedCarId = CarComboBox.SelectedItem.ToString().Split(':')[0];
                var selectedCar = carData.Cars.FirstOrDefault(c => c.Id == selectedCarId);
                if (selectedCar != null && !string.IsNullOrEmpty(selectedCar.PhotoPath))
                {
                    try
                    {
                        string basePath = AppDomain.CurrentDomain.BaseDirectory;
                        string absolutePath = System.IO.Path.Combine(basePath, selectedCar.PhotoPath);
                        CarImage.Source = new BitmapImage(new Uri(absolutePath, UriKind.Absolute));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                        CarImage.Source = null;
                    }
                }
                else
                {
                    CarImage.Source = null;
                }
            }
            else
            {
                CarImage.Source = null;
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
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]+$");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка обязательных полей
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

                if (!DatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Пожалуйста, выберите дату.");
                    return;
                }

                deal.Type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (deal.Type == "Обращение")
                {
                    if (string.IsNullOrWhiteSpace(ThemeTextBox.Text))
                    {
                        MessageBox.Show("Пожалуйста, укажите тему обращения.");
                        return;
                    }
                }
                else
                {
                    if (CarComboBox.SelectedItem == null)
                    {
                        MessageBox.Show("Пожалуйста, выберите автомобиль.");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(AmountTextBox.Text) || !decimal.TryParse(AmountTextBox.Text, out decimal amount))
                    {
                        MessageBox.Show("Пожалуйста, укажите корректную сумму сделки.");
                        return;
                    }
                }

                // Заполнение данных
                deal.ClientName = ClientNameTextBox.Text;
                deal.ClientPhone = ClientPhoneTextBox.Text;
                deal.ClientEmail = ClientEmailTextBox.Text;
                deal.Date = DatePicker.SelectedDate.Value.ToString("dd.MM.yyyy");
                deal.Notes = NotesTextBox.Text;

                if (deal.Type == "Обращение")
                {
                    deal.Theme = ThemeTextBox.Text;
                    deal.Status = (AppealStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                }
                else
                {
                    deal.CarId = CarComboBox.SelectedItem?.ToString()?.Split(':')[0];
                    deal.Amount = amount; // Используем amount, объявленный в TryParse
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
                    deal.Status = (OrderStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                }

                if (!dealData.Deals.Contains(deal))
                {
                    dealData.Deals.Add(deal);
                }

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