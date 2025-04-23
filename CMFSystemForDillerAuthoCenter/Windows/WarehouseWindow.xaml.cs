using CMFSystemForDillerAuthoCenter.CallWindow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class WarehouseWindow : Window
    {
        private CarBrandData brandData;
        private CarData carData;
        private const string BrandsJsonFilePath = "carBrands.json";
        private const string CarsJsonFilePath = "cars.json";

        public WarehouseWindow()
        {
            InitializeComponent();
            LoadBrands();
            LoadCars();
            DataContext = new { Brands = brandData.Brands, Cars = carData.Cars };
        }

        private void LoadBrands()
        {
            try
            {
                if (File.Exists(BrandsJsonFilePath))
                {
                    string json = File.ReadAllText(BrandsJsonFilePath);
                    brandData = JsonConvert.DeserializeObject<CarBrandData>(json);
                }
                else
                {
                    brandData = new CarBrandData
                    {
                        Brands = new List<CarBrand>
                        {
                            new CarBrand { Name = "LADA", LogoPath = "pack://application:,,,/Images/lada_logo.png" },
                            new CarBrand { Name = "SKODA", LogoPath = "pack://application:,,,/Images/skoda_logo.png" },
                            new CarBrand { Name = "NISSAN", LogoPath = "pack://application:,,,/Images/nissan_logo.png" },
                            new CarBrand { Name = "MERCEDES-BENZ", LogoPath = "pack://application:,,,/Images/mercedes_logo.png" },
                            new CarBrand { Name = "TOYOTA", LogoPath = "pack://application:,,,/Images/toyota_logo.png" }
                        }
                    };
                    SaveBrands();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке брендов: {ex.Message}");
                brandData = new CarBrandData();
            }
        }

        private void SaveBrands()
        {
            try
            {
                string json = JsonConvert.SerializeObject(brandData, Formatting.Indented);
                File.WriteAllText(BrandsJsonFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении брендов: {ex.Message}");
            }
        }

        private void LoadCars()
        {
            try
            {
                if (File.Exists(CarsJsonFilePath))
                {
                    string json = File.ReadAllText(CarsJsonFilePath);
                    carData = JsonConvert.DeserializeObject<CarData>(json);
                }
                else
                {
                    carData = new CarData
                    {
                        Cars = new List<Car>
                        {
                            new Car
                            {
                                Id = "C001",
                                Name = "LADA VESTA CROSS WAGON",
                                Type = "Легковушка",
                                Category = "B",
                                Brand = "LADA",
                                Model = "VESTA",
                                Year = 2023,
                                Engine = "Бензин",
                                Drive = "Передний",
                                BodyType = "Универсал",
                                EngineVolume = 1.6,
                                Mileage = 5000,
                                Color = "Белый",
                                SteeringWheel = "Слева",
                                Interior = "Ткань",
                                OwnershipDuration = "1 год",
                                OwnerCount = 1,
                                Condition = "Отличное",
                                Price = 1200000,
                                Status = "В продаже",
                                Notes = "Без замечаний",
                                LastOwner = "Иванов И.И.",
                                OwnerCode = "OWN001",
                                LocationCode = "LOC001",
                                PhotoPath = ""
                            },
                            new Car
                            {
                                Id = "C002",
                                Name = "SKODA OCTAVIA",
                                Type = "Легковушка",
                                Category = "C",
                                Brand = "SKODA",
                                Model = "OCTAVIA",
                                Year = 2022,
                                Engine = "Дизель",
                                Drive = "Передний",
                                BodyType = "Лифтбек",
                                EngineVolume = 1.4,
                                Mileage = 15000,
                                Color = "Черный",
                                SteeringWheel = "Слева",
                                Interior = "Кожа",
                                OwnershipDuration = "2 года",
                                OwnerCount = 2,
                                Condition = "Хорошее",
                                Price = 1800000,
                                Status = "В продаже",
                                Notes = "Мелкие царапины",
                                LastOwner = "Петров П.П.",
                                OwnerCode = "OWN002",
                                LocationCode = "LOC002",
                                PhotoPath = ""
                            }
                        }
                    };
                    SaveCars();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке автомобилей: {ex.Message}");
                carData = new CarData();
            }
        }

        private void SaveCars()
        {
            try
            {
                string json = JsonConvert.SerializeObject(carData, Formatting.Indented);
                File.WriteAllText(CarsJsonFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении автомобилей: {ex.Message}");
            }
        }

        private void AddCarButton_Click(object sender, RoutedEventArgs e)
        {
            // Затемнение текущего окна
            this.Opacity = 0.5;
            this.IsEnabled = false;

            var addCarWindow = new AddCarWindow(carData)
            {
                Owner = this
            };

            if (addCarWindow.ShowDialog() == true)
            {
                CarsDataGrid.Items.Refresh();
            }

            // Восстановление текущего окна
            this.Opacity = 1;
            this.IsEnabled = true;
        }

        private void CarsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CarsDataGrid.SelectedItem is Car selectedCar)
            {
                // Затемнение текущего окна
                this.Opacity = 0.5;
                this.IsEnabled = false;

                var editCarWindow = new AddCarWindow(carData, selectedCar)
                {
                    Owner = this,
                    Title = "Редактировать автомобиль"
                };

                if (editCarWindow.ShowDialog() == true)
                {
                    CarsDataGrid.Items.Refresh();
                }

                // Восстановление текущего окна
                this.Opacity = 1;
                this.IsEnabled = true;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveBrands();
            SaveCars();
            base.OnClosing(e);
        }

        private void GoToMainMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}