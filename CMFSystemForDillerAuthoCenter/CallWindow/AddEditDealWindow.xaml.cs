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
    public partial class AddEditDealWindow : Window
    {
        private DealData dealData;
        private CarData carData;
        private Deal deal;
        private string initialType;

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
            DateTextBox.Text = deal.Date;
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
                AmountTextBox.Text = deal.Amount.ToString();
                PaymentTermsComboBox.SelectedItem = PaymentTermsComboBox.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString() == deal.PaymentTerms);
                DeliveryDateTextBox.Text = deal.DeliveryDate;
                DeliveryAddressTextBox.Text = deal.DeliveryAddress;
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

                CarLabel.Visibility = Visibility.Collapsed;
                CarComboBox.Visibility = Visibility.Collapsed;
                AmountLabel.Visibility = Visibility.Collapsed;
                AmountTextBox.Visibility = Visibility.Collapsed;
                PaymentTermsLabel.Visibility = Visibility.Collapsed;
                PaymentTermsComboBox.Visibility = Visibility.Collapsed;
                DeliveryDateLabel.Visibility = Visibility.Collapsed;
                DeliveryDateTextBox.Visibility = Visibility.Collapsed;
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

                CarLabel.Visibility = Visibility.Visible;
                CarComboBox.Visibility = Visibility.Visible;
                AmountLabel.Visibility = Visibility.Visible;
                AmountTextBox.Visibility = Visibility.Visible;
                PaymentTermsLabel.Visibility = Visibility.Visible;
                PaymentTermsComboBox.Visibility = Visibility.Visible;
                DeliveryDateLabel.Visibility = Visibility.Visible;
                DeliveryDateTextBox.Visibility = Visibility.Visible;
                DeliveryAddressLabel.Visibility = Visibility.Visible;
                DeliveryAddressTextBox.Visibility = Visibility.Visible;
                OrderStatusLabel.Visibility = Visibility.Visible;
                OrderStatusComboBox.Visibility = Visibility.Visible;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                deal.Type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                deal.ClientName = ClientNameTextBox.Text;
                deal.ClientPhone = ClientPhoneTextBox.Text;
                deal.ClientEmail = ClientEmailTextBox.Text;
                deal.Date = string.IsNullOrEmpty(DateTextBox.Text) ? DateTime.Now.ToString("dd.MM.yyyy") : DateTextBox.Text;
                deal.Notes = NotesTextBox.Text;

                if (deal.Type == "Обращение")
                {
                    deal.Theme = ThemeTextBox.Text;
                    deal.Status = (AppealStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                }
                else
                {
                    deal.CarId = CarComboBox.SelectedItem?.ToString()?.Split(':')[0];
                    deal.Amount = decimal.TryParse(AmountTextBox.Text, out decimal amount) ? amount : 0;
                    deal.PaymentTerms = (PaymentTermsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    deal.DeliveryDate = DeliveryDateTextBox.Text;
                    deal.DeliveryAddress = DeliveryAddressTextBox.Text;
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