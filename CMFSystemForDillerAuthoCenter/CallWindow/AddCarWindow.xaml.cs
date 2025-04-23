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
    public partial class AddCarWindow : Window
    {
        private Car car;
        private CarData carData;

        public AddCarWindow(CarData carData, Car carToEdit = null)
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации окна: {ex.Message}");
                return;
            }

            this.carData = carData;
            this.car = carToEdit ?? new Car { Id = $"C{carData.Cars.Count + 1:D03}" };

            // Заполнение ComboBox для года выпуска (1999–2025)
            for (int year = 1999; year <= 2025; year++)
            {
                YearComboBox.Items.Add(year);
            }

            // Если редактирование, заполняем поля
            if (carToEdit != null)
            {
                NameTextBox.Text = car.Name ?? string.Empty;
                TypeComboBox.SelectedItem = TypeComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == car.Type);
                CategoryComboBox.SelectedItem = CategoryComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == car.Category);
                BrandTextBox.Text = car.Brand ?? string.Empty;
                ModelTextBox.Text = car.Model ?? string.Empty;
                YearComboBox.SelectedItem = car.Year;
                EngineComboBox.SelectedItem = EngineComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == car.Engine);
                DriveComboBox.SelectedItem = DriveComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == car.Drive);
                BodyTypeComboBox.SelectedItem = BodyTypeComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == car.BodyType);
                EngineVolumeTextBox.Text = car.EngineVolume.ToString();
                MileageTextBox.Text = car.Mileage.ToString();
                ColorTextBox.Text = car.Color ?? string.Empty;
                SteeringWheelComboBox.SelectedItem = SteeringWheelComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == car.SteeringWheel);
                InteriorTextBox.Text = car.Interior ?? string.Empty;
                OwnershipDurationTextBox.Text = car.OwnershipDuration ?? string.Empty;
                OwnerCountTextBox.Text = car.OwnerCount.ToString();
                ConditionTextBox.Text = car.Condition ?? string.Empty;
                PriceTextBox.Text = car.Price.ToString();
                StatusComboBox.SelectedItem = StatusComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == car.Status);
                NotesTextBox.Text = car.Notes ?? string.Empty;
                LastOwnerTextBox.Text = car.LastOwner ?? string.Empty;
                OwnerCodeTextBox.Text = car.OwnerCode ?? string.Empty;
                LocationCodeTextBox.Text = car.LocationCode ?? string.Empty;
                if (!string.IsNullOrEmpty(car.PhotoPath))
                {
                    try
                    {
                        CarPhotoImage.Source = new BitmapImage(new Uri(car.PhotoPath));
                    }
                    catch
                    {
                        CarPhotoImage.Source = null;
                    }
                }
            }
        }

        private void LoadPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                car.PhotoPath = openFileDialog.FileName;
                CarPhotoImage.Source = new BitmapImage(new Uri(car.PhotoPath));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                car.Name = NameTextBox.Text;
                car.Type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                car.Category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                car.Brand = BrandTextBox.Text;
                car.Model = ModelTextBox.Text;
                car.Year = YearComboBox.SelectedItem != null ? Convert.ToInt32(YearComboBox.SelectedItem) : 0;
                car.Engine = (EngineComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                car.Drive = (DriveComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                car.BodyType = (BodyTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                car.EngineVolume = double.TryParse(EngineVolumeTextBox.Text, out double engineVolume) ? engineVolume : 0;
                car.Mileage = int.TryParse(MileageTextBox.Text, out int mileage) ? mileage : 0;
                car.Color = ColorTextBox.Text;
                car.SteeringWheel = (SteeringWheelComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                car.Interior = InteriorTextBox.Text;
                car.OwnershipDuration = OwnershipDurationTextBox.Text;
                car.OwnerCount = int.TryParse(OwnerCountTextBox.Text, out int ownerCount) ? ownerCount : 0;
                car.Condition = ConditionTextBox.Text;
                car.Price = decimal.TryParse(PriceTextBox.Text, out decimal price) ? price : 0;
                car.Status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                car.Notes = NotesTextBox.Text;
                car.LastOwner = LastOwnerTextBox.Text;
                car.OwnerCode = OwnerCodeTextBox.Text;
                car.LocationCode = LocationCodeTextBox.Text;

                // Если это новый автомобиль, добавляем его в список
                if (!carData.Cars.Contains(car))
                {
                    carData.Cars.Add(car);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }
    }
}