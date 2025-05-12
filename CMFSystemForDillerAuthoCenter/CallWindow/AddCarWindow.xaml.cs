using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace CMFSystemForDillerAuthoCenter.CallWindow
{
    public partial class AddCarWindow : Window
    {
        private Car _car;

        public AddCarWindow(Car carToEdit = null)
        {
            InitializeComponent();
            // Загружаем данные автомобилей при открытии окна
            DataStorage.LoadCars();

            _car = carToEdit ?? new Car
            {
                Id = $"CARD{(DataStorage.CarData.Cars.Count + 1):D03}",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            if (carToEdit != null)
            {
                StsSeriesTextBox.Text = carToEdit.StsSeries;
                StsNumberTextBox.Text = carToEdit.StsNumber;
                OwnerLastNameTextBox.Text = carToEdit.OwnerLastName;
                OwnerFirstNameTextBox.Text = carToEdit.OwnerFirstName;
                OwnerMiddleNameTextBox.Text = carToEdit.OwnerMiddleName;
                OwnerAddressTextBox.Text = carToEdit.OwnerAddress;
                LicensePlateTextBox.Text = carToEdit.LicensePlate;
                LicensePlateRegionTextBox.Text = carToEdit.LicensePlateRegion;
                VinTextBox.Text = carToEdit.Vin;
                BodyNumberTextBox.Text = carToEdit.BodyNumber;
                VehicleCategoryTextBox.Text = carToEdit.VehicleCategory;
                YearTextBox.Text = carToEdit.Year.ToString();
                ColorTextBox.Text = carToEdit.Color;
                EnginePowerTextBox.Text = carToEdit.EnginePower;
                EnvironmentalClassTextBox.Text = carToEdit.EnvironmentalClass;
                MaxPermittedWeightTextBox.Text = carToEdit.MaxPermittedWeight;
                UnladenWeightTextBox.Text = carToEdit.UnladenWeight;
                PtsSeriesTextBox.Text = carToEdit.PtsSeries;
                PtsNumberTextBox.Text = carToEdit.PtsNumber;
                LocationCodeTextBox.Text = carToEdit.SiteCode;
                BrandTextBox.Text = carToEdit.Brand;
                ModelTextBox.Text = carToEdit.Model;

                // Загружаем фотографию, если она есть
                if (!string.IsNullOrEmpty(carToEdit.PhotoPath))
                {
                    try
                    {
                        string basePath = AppDomain.CurrentDomain.BaseDirectory; // Базовая директория приложения
                        string absolutePath;

                        // Проверяем, является ли путь абсолютным
                        if (Path.IsPathRooted(carToEdit.PhotoPath))
                        {
                            absolutePath = carToEdit.PhotoPath; // Если путь абсолютный, используем его
                        }
                        else
                        {
                            // Если путь относительный, преобразуем его в абсолютный
                            absolutePath = Path.GetFullPath(Path.Combine(basePath, carToEdit.PhotoPath));
                        }

                        // Проверяем, существует ли файл
                        if (File.Exists(absolutePath))
                        {
                            // Приводим путь к формату, подходящему для Uri (заменяем \ на /)
                            string uriPath = absolutePath.Replace("\\", "/");
                            CarPhotoImage.Source = new BitmapImage(new Uri(uriPath, UriKind.Absolute));
                        }
                        else
                        {
                            MessageBox.Show($"Файл фотографии по пути {absolutePath} не найден.");
                            CarPhotoImage.Source = null;
                        }
                    }
                    catch (UriFormatException ex)
                    {
                        MessageBox.Show($"Ошибка формата URI: {ex.Message}. Проверьте путь: {carToEdit.PhotoPath}");
                        CarPhotoImage.Source = null;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке существующей фотографии: {ex.Message}");
                        CarPhotoImage.Source = null;
                    }
                }
            }
        }

        private void LoadPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string fileName = Path.GetFileName(openFileDialog.FileName);
                    string destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);

                    // Создаем директорию Images, если она не существует
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                    // Копируем файл в папку Images приложения
                    File.Copy(openFileDialog.FileName, destinationPath, true);

                    // Приводим путь к формату, подходящему для Uri
                    string uriPath = destinationPath.Replace("\\", "/");
                    CarPhotoImage.Source = new BitmapImage(new Uri(uriPath, UriKind.Absolute));

                    // Сохраняем относительный путь
                    _car.PhotoPath = Path.Combine("Images", fileName).Replace("\\", "/");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке фото: {ex.Message}");
                    CarPhotoImage.Source = null;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на заполненность всех обязательных полей
            if (string.IsNullOrWhiteSpace(StsSeriesTextBox.Text) ||
                string.IsNullOrWhiteSpace(StsNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(OwnerLastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(OwnerFirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(OwnerMiddleNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(OwnerAddressTextBox.Text) ||
                string.IsNullOrWhiteSpace(LicensePlateTextBox.Text) ||
                string.IsNullOrWhiteSpace(LicensePlateRegionTextBox.Text) ||
                string.IsNullOrWhiteSpace(VinTextBox.Text) ||
                string.IsNullOrWhiteSpace(BodyNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(VehicleCategoryTextBox.Text) ||
                string.IsNullOrWhiteSpace(YearTextBox.Text) ||
                string.IsNullOrWhiteSpace(ColorTextBox.Text) ||
                string.IsNullOrWhiteSpace(EnginePowerTextBox.Text) ||
                string.IsNullOrWhiteSpace(EnvironmentalClassTextBox.Text) ||
                string.IsNullOrWhiteSpace(MaxPermittedWeightTextBox.Text) ||
                string.IsNullOrWhiteSpace(UnladenWeightTextBox.Text) ||
                string.IsNullOrWhiteSpace(PtsSeriesTextBox.Text) ||
                string.IsNullOrWhiteSpace(PtsNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(LocationCodeTextBox.Text) ||
                string.IsNullOrWhiteSpace(BrandTextBox.Text) ||
                string.IsNullOrWhiteSpace(ModelTextBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля.");
                return;
            }

            // 1. Серия СТС: 2–3 символа (буквы или цифры)
            if (!Regex.IsMatch(StsSeriesTextBox.Text, @"^[A-Za-zА-Яа-я0-9]{2,3}$"))
            {
                MessageBox.Show("Серия СТС должна содержать 2–3 символа (буквы или цифры).");
                return;
            }

            // 2. Номер СТС: 6–7 цифр
            if (!Regex.IsMatch(StsNumberTextBox.Text, @"^\d{6,7}$"))
            {
                MessageBox.Show("Номер СТС должен содержать 6–7 цифр.");
                return;
            }

            // 3. Фамилия владельца: до 50 символов
            if (OwnerLastNameTextBox.Text.Length > 50)
            {
                MessageBox.Show("Фамилия владельца не должна превышать 50 символов.");
                return;
            }

            // 4. Имя владельца: до 50 символов
            if (OwnerFirstNameTextBox.Text.Length > 50)
            {
                MessageBox.Show("Имя владельца не должно превышать 50 символов.");
                return;
            }

            // 5. Отчество владельца: до 50 символов
            if (OwnerMiddleNameTextBox.Text.Length > 50)
            {
                MessageBox.Show("Отчество владельца не должно превышать 50 символов.");
                return;
            }

            // 6. Адрес прописки: до 200 символов
            if (OwnerAddressTextBox.Text.Length > 200)
            {
                MessageBox.Show("Адрес прописки не должен превышать 200 символов.");
                return;
            }

            // 7. Госномер автомобиля (без региона): 6 символов (буквы и цифры)
            if (!Regex.IsMatch(LicensePlateTextBox.Text, @"^[A-Za-zА-Яа-я0-9]{6}$"))
            {
                MessageBox.Show("Госномер автомобиля должен содержать ровно 6 символов (буквы и цифры).");
                return;
            }

            // 8. Регион номера: 2–3 цифры
            if (!Regex.IsMatch(LicensePlateRegionTextBox.Text, @"^\d{2,3}$"))
            {
                MessageBox.Show("Регион номера должен содержать 2–3 цифры.");
                return;
            }

            // 9. VIN-номер: 17 символов (цифры и латинские буквы)
            if (!Regex.IsMatch(VinTextBox.Text, @"^[A-Za-z0-9]{17}$"))
            {
                MessageBox.Show("VIN-номер должен содержать ровно 17 символов (цифры и латинские буквы).");
                return;
            }

            // 10. Номер кузова: до 20 символов
            if (BodyNumberTextBox.Text.Length > 20)
            {
                MessageBox.Show("Номер кузова не должен превышать 20 символов.");
                return;
            }

            // 11. Категория ТС: 1–2 символа
            if (!Regex.IsMatch(VehicleCategoryTextBox.Text, @"^[A-Za-z]{1,2}$"))
            {
                MessageBox.Show("Категория ТС должна содержать 1–2 символа (латинские буквы).");
                return;
            }

            // 12. Год выпуска: 4 цифры
            if (!int.TryParse(YearTextBox.Text, out int year) ||
                year < 1900 || year > DateTime.Now.Year + 1)
            {
                MessageBox.Show("Год выпуска должен быть 4-значным числом в диапазоне от 1900 до текущего года + 1.");
                return;
            }

            // 13. Цвет автомобиля: до 20 символов
            if (ColorTextBox.Text.Length > 20)
            {
                MessageBox.Show("Цвет автомобиля не должен превышать 20 символов.");
                return;
            }

            // 14. Мощность двигателя: до 10 символов
            if (EnginePowerTextBox.Text.Length > 10)
            {
                MessageBox.Show("Мощность двигателя не должна превышать 10 символов.");
                return;
            }

            // 15. Экологический класс: до 10 символов
            if (EnvironmentalClassTextBox.Text.Length > 10)
            {
                MessageBox.Show("Экологический класс не должен превышать 10 символов.");
                return;
            }

            // 16. Разрешённая максимальная масса: до 10 символов
            if (MaxPermittedWeightTextBox.Text.Length > 10)
            {
                MessageBox.Show("Разрешённая максимальная масса не должна превышать 10 символов.");
                return;
            }

            // 17. Масса без нагрузки: до 10 символов
            if (UnladenWeightTextBox.Text.Length > 10)
            {
                MessageBox.Show("Масса без нагрузки не должна превышать 10 символов.");
                return;
            }

            // 18. Серия ПТС: 2–4 символа (буквы и цифры)
            if (!Regex.IsMatch(PtsSeriesTextBox.Text, @"^[A-Za-zА-Яа-я0-9]{2,4}$"))
            {
                MessageBox.Show("Серия ПТС должна содержать 2–4 символа (буквы или цифры).");
                return;
            }

            // 19. Номер ПТС: 6–8 цифр
            if (!Regex.IsMatch(PtsNumberTextBox.Text, @"^\d{6,8}$"))
            {
                MessageBox.Show("Номер ПТС должен содержать 6–8 цифр.");
                return;
            }

            // Дополнительные проверки для Brand и Model (до 50 символов)
            if (BrandTextBox.Text.Length > 50)
            {
                MessageBox.Show("Марка автомобиля не должна превышать 50 символов.");
                return;
            }

            if (ModelTextBox.Text.Length > 50)
            {
                MessageBox.Show("Модель автомобиля не должна превышать 50 символов.");
                return;
            }

            // Сохранение данных
            _car.StsSeries = StsSeriesTextBox.Text;
            _car.StsNumber = StsNumberTextBox.Text;
            _car.OwnerLastName = OwnerLastNameTextBox.Text;
            _car.OwnerFirstName = OwnerFirstNameTextBox.Text;
            _car.OwnerMiddleName = OwnerMiddleNameTextBox.Text;
            _car.OwnerAddress = OwnerAddressTextBox.Text;
            _car.LicensePlate = LicensePlateTextBox.Text;
            _car.LicensePlateRegion = LicensePlateRegionTextBox.Text;
            _car.Vin = VinTextBox.Text;
            _car.BodyNumber = BodyNumberTextBox.Text;
            _car.VehicleCategory = VehicleCategoryTextBox.Text;
            _car.Year = year;
            _car.Color = ColorTextBox.Text;
            _car.EnginePower = EnginePowerTextBox.Text;
            _car.EnvironmentalClass = EnvironmentalClassTextBox.Text;
            _car.MaxPermittedWeight = MaxPermittedWeightTextBox.Text;
            _car.UnladenWeight = UnladenWeightTextBox.Text;
            _car.PtsSeries = PtsSeriesTextBox.Text;
            _car.PtsNumber = PtsNumberTextBox.Text;
            _car.SiteCode = LocationCodeTextBox.Text;
            _car.Brand = BrandTextBox.Text;
            _car.Model = ModelTextBox.Text;
            // PhotoPath уже установлен в LoadPhotoButton_Click
            _car.ModifiedDate = DateTime.Now;

            if (!DataStorage.CarData.Cars.Contains(_car))
            {
                DataStorage.CarData.Cars.Add(_car);
            }

            DataStorage.SaveCars();
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