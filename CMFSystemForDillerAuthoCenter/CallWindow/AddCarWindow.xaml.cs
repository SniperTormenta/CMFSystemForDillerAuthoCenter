using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using Path = System.IO.Path;


namespace CMFSystemForDillerAuthoCenter.CallWindow
{
    public partial class AddCarWindow : Window
    {
        private CarData carData;
        private Car car;

        public AddCarWindow(CarData carData, Car carToEdit = null)
        {
            InitializeComponent();
            this.carData = DataStorage.CarData;
            this.car = carToEdit ?? new Car { Id = $"C{(carData?.Cars?.Count ?? 0) + 1:D03}" };

            InitializeForm();

            if (carToEdit != null)
            {
                PopulateFields();
            }
        }

        private void InitializeForm()
        {
            for (int year = DateTime.Now.Year; year >= 1900; year--)
            {
                YearComboBox.Items.Add(year);
            }
        }

        private void PopulateFields()
        {
            NameTextBox.Text = car.Name;
            TypeComboBox.SelectedItem = TypeComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == car.Type);
            CategoryComboBox.SelectedItem = CategoryComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == car.Category);
            BrandTextBox.Text = car.Brand;
            ModelTextBox.Text = car.Model;
            YearComboBox.SelectedItem = car.Year;
            EngineComboBox.SelectedItem = EngineComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == car.Engine);
            DriveComboBox.SelectedItem = DriveComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == car.Drive);
            BodyTypeComboBox.SelectedItem = BodyTypeComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == car.BodyType);
            EngineVolumeTextBox.Text = car.EngineVolume.ToString();
            MileageTextBox.Text = car.Mileage.ToString();
            ColorTextBox.Text = car.Color;
            SteeringWheelComboBox.SelectedItem = SteeringWheelComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == car.SteeringWheel);
            InteriorTextBox.Text = car.Interior;
            OwnershipDurationTextBox.Text = car.OwnershipDuration;
            OwnerCountTextBox.Text = car.OwnerCount.ToString();
            ConditionTextBox.Text = car.Condition;
            PriceTextBox.Text = car.Price.ToString();
            StatusComboBox.SelectedItem = StatusComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == car.Status);
            NotesTextBox.Text = car.Notes;
            LastOwnerTextBox.Text = car.LastOwner;
            OwnerCodeTextBox.Text = car.OwnerCode;
            LocationCodeTextBox.Text = car.LocationCode;

            if (!string.IsNullOrEmpty(car.PhotoPath))
            {
                try
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string absolutePath = Path.Combine(basePath, car.PhotoPath);
                    CarPhotoImage.Source = new BitmapImage(new Uri(absolutePath, UriKind.Absolute));
                }
                catch
                {
                    CarPhotoImage.Source = null;
                }
            }
        }

        private void LoadPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string fileName = Path.GetFileName(openFileDialog.FileName);
                    string destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                    File.Copy(openFileDialog.FileName, destinationPath, true);
                    CarPhotoImage.Source = new BitmapImage(new Uri(destinationPath, UriKind.Absolute));
                    car.PhotoPath = Path.Combine("Images", fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}");
                    CarPhotoImage.Source = null;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(BrandTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, укажите марку автомобиля.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(ModelTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, укажите модель автомобиля.");
                    return;
                }

                if (YearComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите год выпуска.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(PriceTextBox.Text) || !decimal.TryParse(PriceTextBox.Text, out decimal price))
                {
                    MessageBox.Show("Пожалуйста, укажите корректную цену.");
                    return;
                }

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
                car.Price = price;
                car.Status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                car.Notes = NotesTextBox.Text;
                car.LastOwner = LastOwnerTextBox.Text;
                car.OwnerCode = OwnerCodeTextBox.Text;
                car.LocationCode = LocationCodeTextBox.Text;

                if (!carData.Cars.Contains(car))
                {
                    carData.Cars.Add(car);
                }

                DataStorage.SaveCars();

                System.Diagnostics.Debug.WriteLine($"AddCarWindow: После сохранения carData содержит {carData?.Cars?.Count ?? 0} автомобилей.");

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

        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]+(\.[0-9]+)?$");
        }
    }
}