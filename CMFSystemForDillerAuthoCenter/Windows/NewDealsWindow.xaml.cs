using CMFSystemForDillerAuthoCenter.CallWindow;
using CMFSystemForDillerAuthoCenter.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CMFSystemForDillerAuthoCenter.Windows
{
    public partial class NewDealsWindow : Window
    {
        private CarData _carData;
        private ClientStorage _clientStorage;
        private DealData _dealData;
        private EmployeeStorage _employeeStorage;
        private EmailService _emailService;
        private UserControl variant1;
        private UserControl variant2;

        public NewDealsWindow(CarData carData, ClientStorage clientStorage, EmployeeStorage employeeStorage = null, EmailService emailService = null)
        {
            InitializeComponent();
            _carData = carData ?? DataStorage.CarData;
            _clientStorage = clientStorage ?? throw new ArgumentNullException(nameof(clientStorage));
            _employeeStorage = employeeStorage ?? new EmployeeStorage();
            _emailService = emailService ?? new EmailService();
            DataStorage.LoadDeals();
            _dealData = DataStorage.DealData;
            System.Diagnostics.Debug.WriteLine($"NewDealsWindow: _carData содержит {_carData?.Cars?.Count ?? 0} автомобилей.");
            System.Diagnostics.Debug.WriteLine($"NewDealsWindow: _dealData содержит {_dealData?.Deals?.Count ?? 0} сделок.");
            System.Diagnostics.Debug.WriteLine($"NewDealsWindow: _clientStorage содержит {_clientStorage?.Clients?.Count ?? 0} клиентов.");
            InitializeVariants();
            SetInitialView();
        }

        public NewDealsWindow()
        {
            InitializeComponent();
            DataStorage.LoadCars();
            _carData = DataStorage.CarData;
            _clientStorage = ClientStorage.Load() ?? new ClientStorage();
            _employeeStorage = new EmployeeStorage();
            _emailService = new EmailService();
            DataStorage.LoadDeals();
            _dealData = DataStorage.DealData;
            System.Diagnostics.Debug.WriteLine($"NewDealsWindow (default): _carData содержит {_carData?.Cars?.Count ?? 0} автомобилей.");
            System.Diagnostics.Debug.WriteLine($"NewDealsWindow (default): _dealData содержит {_dealData?.Deals?.Count ?? 0} сделок.");
            System.Diagnostics.Debug.WriteLine($"NewDealsWindow (default): _clientStorage содержит {_clientStorage?.Clients?.Count ?? 0} клиентов.");
            InitializeVariants();
            SetInitialView();
        }

        private void InitializeVariants()
        {
            System.Diagnostics.Debug.WriteLine($"InitializeVariants: _carData содержит {_carData?.Cars?.Count ?? 0} автомобилей.");
            System.Diagnostics.Debug.WriteLine($"InitializeVariants: _dealData содержит {_dealData?.Deals?.Count ?? 0} сделок.");
            variant1 = new NewDealsVariant1(_dealData, _carData, DataStorage.SaveDeals);
            variant2 = new NewDealsVariant2(_dealData, _carData, DataStorage.SaveDeals);
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
            var createContractWindow = new CreateSaleContractWindow(_dealData, _carData, _clientStorage)
            {
                Owner = this
            };
            createContractWindow.ShowDialog();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            DataStorage.SaveDeals();
            DataStorage.SaveCars();
            _clientStorage.Save();
            System.Diagnostics.Debug.WriteLine($"NewDealsWindow OnClosing: Сохранено {_dealData?.Deals?.Count ?? 0} сделок.");
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
            if (_clientStorage == null)
            {
                MessageBox.Show("Ошибка: ClientStorage не инициализирован.");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"NewDealsButton_Click: Передача _clientStorage с {_clientStorage.Clients?.Count ?? 0} клиентами.");
            var newDealsWindow = new NewDealsWindow(_carData, _clientStorage, _employeeStorage, _emailService);
            newDealsWindow.Show();
            Close();
        }

        private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            var emailWindow = new EmailWindow();
            emailWindow.Show();
        }

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            var clientsWindow = new ClientsWindow();
            clientsWindow.Show();
            Close();
        }

        private void CalendareButton_Click(object sender, RoutedEventArgs e)
        {
            if (_dealData == null || _clientStorage == null || _employeeStorage == null || _emailService == null)
            {
                MessageBox.Show("Ошибка: Необходимые данные не инициализированы.");
                return;
            }

            var reportWindow = new ReportWindow(_dealData, _clientStorage, _employeeStorage, _emailService)
            {
                Owner = this
            };
            reportWindow.ShowDialog();
        }
    }
}