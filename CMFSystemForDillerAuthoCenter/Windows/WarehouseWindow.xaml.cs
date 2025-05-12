using CMFSystemForDillerAuthoCenter.CallWindow;
using CMFSystemForDillerAuthoCenter.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Path = System.IO.Path;

namespace CMFSystemForDillerAuthoCenter.Windows
{
    public partial class WarehouseWindow : Window
    {
        private DealData _dealData;
        private ClientStorage _clientStorage;
        private EmployeeStorage _employeeStorage;
        private EmailService _emailService;

        public WarehouseWindow()
        {
            InitializeComponent();
            DataStorage.LoadCars();
            CarsDataGrid.ItemsSource = DataStorage.CarData.Cars; // Используем DataStorage напрямую
            System.Diagnostics.Debug.WriteLine($"WarehouseWindow: DataStorage.CarData содержит {DataStorage.CarData.Cars.Count} автомобилей.");

            // Подписка на событие перехвата клавиш
            PreviewKeyDown += WarehouseWindow_PreviewKeyDown;
        }

        private void WarehouseWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Проверка комбинации Ctrl+Alt+A
            if (e.Key == Key.A && Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftAlt))
            {
                e.Handled = true; // Отмечаем, что событие обработано
                LoadJsonFile();
            }
        }

        private void LoadJsonFile()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Выберите JSON-файл с данными автомобилей"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string jsonContent = File.ReadAllText(openFileDialog.FileName);
                    var loadedCars = JsonConvert.DeserializeObject<List<Car>>(jsonContent);

                    if (loadedCars != null && loadedCars.Any())
                    {
                        // Очищаем существующие данные и заменяем их новыми
                        DataStorage.CarData.Cars.Clear();
                        DataStorage.CarData.Cars.AddRange(loadedCars);
                        CarsDataGrid.ItemsSource = null;
                        CarsDataGrid.ItemsSource = DataStorage.CarData.Cars;
                        DataStorage.SaveCars(); // Сохраняем обновленные данные
                        System.Diagnostics.Debug.WriteLine($"Загружено {loadedCars.Count} автомобилей.");
                        MessageBox.Show($"Успешно загружено {loadedCars.Count} автомобилей. Все предыдущие данные стерты.");
                    }
                    else
                    {
                        MessageBox.Show("Файл JSON не содержит данных об автомобилях.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}");
                }
            }
        }

        private void AddCarButton_Click(object sender, RoutedEventArgs e)
        {
            var addCarWindow = new AddCarWindow(); // Больше не передаём carData
            addCarWindow.Owner = this;
            if (addCarWindow.ShowDialog() == true)
            {
                CarsDataGrid.ItemsSource = null;
                CarsDataGrid.ItemsSource = DataStorage.CarData.Cars;
                System.Diagnostics.Debug.WriteLine($"После добавления: DataStorage.CarData содержит {DataStorage.CarData.Cars.Count} автомобилей.");
            }
        }

        private void EditCarButton_Click(object sender, RoutedEventArgs e)
        {
            if (CarsDataGrid.SelectedItem is Car selectedCar)
            {
                var editCarWindow = new AddCarWindow(selectedCar); // Больше не передаём carData
                editCarWindow.Owner = this;
                editCarWindow.Title = "Редактировать автомобиль";
                if (editCarWindow.ShowDialog() == true)
                {
                    CarsDataGrid.ItemsSource = null;
                    CarsDataGrid.ItemsSource = DataStorage.CarData.Cars;
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите автомобиль для редактирования.");
            }
        }

        private void CarsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CarsDataGrid.SelectedItem is Car selectedCar)
            {
                var editCarWindow = new AddCarWindow(selectedCar); // Больше не передаём carData
                editCarWindow.Owner = this;
                editCarWindow.Title = "Редактировать автомобиль";
                if (editCarWindow.ShowDialog() == true)
                {
                    CarsDataGrid.ItemsSource = null;
                    CarsDataGrid.ItemsSource = DataStorage.CarData.Cars;
                }
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            DataStorage.SaveCars();
            base.OnClosing(e);
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
            var newDealsWindow = new NewDealsWindow(DataStorage.CarData, _clientStorage);
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
    }
}