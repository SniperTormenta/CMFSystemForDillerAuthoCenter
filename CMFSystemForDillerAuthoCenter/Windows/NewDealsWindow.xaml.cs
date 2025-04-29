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
    public partial class NewDealsWindow : Window
    {
        private DealData dealData;
        private CarData carData;
        private ClientStorage clientStorage;
        private UserControl variant1;
        private UserControl variant2;

        public NewDealsWindow(CarData carData, ClientStorage clientStorage)
        {
            InitializeComponent();
            this.carData = DataStorage.CarData;
            this.clientStorage = clientStorage;
            DataStorage.LoadDeals();
            dealData = DataStorage.DealData;
            System.Diagnostics.Debug.WriteLine($"NewDealsWindow: carData содержит {carData?.Cars?.Count ?? 0} автомобилей.");
            System.Diagnostics.Debug.WriteLine($"NewDealsWindow: dealData содержит {dealData?.Deals?.Count ?? 0} сделок.");
            InitializeVariants();
            SetInitialView();
        }

        public NewDealsWindow()
        {
            InitializeComponent();
            DataStorage.LoadCars();
            carData = DataStorage.CarData;
            clientStorage = ClientStorage.Load();
            DataStorage.LoadDeals();
            dealData = DataStorage.DealData;
            System.Diagnostics.Debug.WriteLine($"NewDealsWindow (default): carData содержит {carData?.Cars?.Count ?? 0} автомобилей.");
            System.Diagnostics.Debug.WriteLine($"NewDealsWindow (default): dealData содержит {dealData?.Deals?.Count ?? 0} сделок.");
            InitializeVariants();
            SetInitialView();
        }

        private void InitializeVariants()
        {
            System.Diagnostics.Debug.WriteLine($"InitializeVariants: carData содержит {carData?.Cars?.Count ?? 0} автомобилей.");
            System.Diagnostics.Debug.WriteLine($"InitializeVariants: dealData содержит {dealData?.Deals?.Count ?? 0} сделок.");
            variant1 = new NewDealsVariant1(dealData, carData, DataStorage.SaveDeals);
            variant2 = new NewDealsVariant2(dealData, carData, DataStorage.SaveDeals);
        }

        private void SetInitialView()
        {
            DealsContent.Content = variant1;
        }

        private void ViewToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            DealsContent.Content = variant2;
        }

        private void ViewToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            DealsContent.Content = variant1;
        }

        private void CreateSaleContractButton_Click(object sender, RoutedEventArgs e)
        {
            var createContractWindow = new CreateSaleContractWindow(dealData, carData, clientStorage)
            {
                Owner = this
            };
            createContractWindow.ShowDialog();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            DataStorage.SaveDeals();
            DataStorage.SaveCars();
            clientStorage.Save();
            System.Diagnostics.Debug.WriteLine($"NewDealsWindow OnClosing: Сохранено {dealData?.Deals?.Count ?? 0} сделок.");
            base.OnClosing(e);
        }
    }
}