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
using Path = System.IO.Path;

namespace CMFSystemForDillerAuthoCenter.Windows
{
    public partial class WarehouseWindow : Window
    {
        private CarData carData;

        public WarehouseWindow()
        {
            InitializeComponent();
            DataStorage.LoadCars();
            carData = DataStorage.CarData;
            CarsDataGrid.ItemsSource = carData?.Cars;
            System.Diagnostics.Debug.WriteLine($"WarehouseWindow: carData содержит {carData?.Cars?.Count ?? 0} автомобилей.");
        }

        private void AddCarButton_Click(object sender, RoutedEventArgs e)
        {
            var addCarWindow = new AddCarWindow(carData);
            addCarWindow.Owner = this;
            if (addCarWindow.ShowDialog() == true)
            {
                CarsDataGrid.ItemsSource = null;
                CarsDataGrid.ItemsSource = carData?.Cars;
                System.Diagnostics.Debug.WriteLine($"После добавления: carData содержит {carData?.Cars?.Count ?? 0} автомобилей.");
            }
        }

        private void EditCarButton_Click(object sender, RoutedEventArgs e)
        {
            if (CarsDataGrid.SelectedItem is Car selectedCar)
            {
                var editCarWindow = new AddCarWindow(carData, selectedCar);
                editCarWindow.Owner = this;
                editCarWindow.Title = "Редактировать автомобиль";
                if (editCarWindow.ShowDialog() == true)
                {
                    CarsDataGrid.ItemsSource = null;
                    CarsDataGrid.ItemsSource = carData?.Cars;
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
                var editCarWindow = new AddCarWindow(carData, selectedCar);
                editCarWindow.Owner = this;
                editCarWindow.Title = "Редактировать автомобиль";
                if (editCarWindow.ShowDialog() == true)
                {
                    CarsDataGrid.ItemsSource = null;
                    CarsDataGrid.ItemsSource = carData?.Cars;
                }
            }
        }

        private void GoToMainMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void OpenNewDealsWindow_Click(object sender, RoutedEventArgs e)
        {
            var newDealsWindow = new NewDealsWindow(carData);
            newDealsWindow.Show();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            DataStorage.SaveCars();
            base.OnClosing(e);
        }
    }
}