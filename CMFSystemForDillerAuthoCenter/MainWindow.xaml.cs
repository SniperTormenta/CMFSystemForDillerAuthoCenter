﻿using CMFSystemForDillerAuthoCenter.CallWindow;
using CMFSystemForDillerAuthoCenter.Windows;
using CMFSystemForDillerAuthoCenter;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CMFSystemForDillerAuthoCenter.Services;


namespace CMFSystemForDillerAuthoCenter
{
    public partial class MainWindow : Window
    {
        private List<Deal> appeals;
        private List<Deal> orders;
        private ClientStorage _clientStorage;
        private EmployeeStorage _employeeStorage;
        private DealData _dealData;
        private EmailService _emailService;

        public List<Deal> Appeals
        {
            get { return appeals; }
            set { appeals = value; }
        }

        public List<Deal> Orders
        {
            get { return orders; }
            set { orders = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            _clientStorage = ClientStorage.Load();
            _employeeStorage = EmployeeStorage.Load();
            _dealData = DataStorage.DealData ?? new DealData();
            _emailService = new EmailService();
            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            DataStorage.LoadDeals();
            DataStorage.LoadCars();

            var deals = DataStorage.DealData.Deals;
            appeals = deals.Where(d => d.Type == "Обращение").ToList();
            orders = deals.Where(d => d.Type == "Заказ").Select(d => new Deal
            {
                Id = d.Id,
                Type = d.Type,
                ClientName = d.ClientName,
                ClientPhone = d.ClientPhone,
                ClientEmail = d.ClientEmail,
                Date = d.Date,
                Theme = d.Theme,
                Notes = d.Notes,
                Status = d.Status,
                CarId = d.CarId,
                CarInfo = GetCarInfo(d.CarId),
                Amount = d.Amount,
                PaymentTerms = d.PaymentTerms,
                IsDeliveryRequired = d.IsDeliveryRequired,
                DeliveryDate = d.DeliveryDate,
                DeliveryAddress = d.DeliveryAddress
            }).ToList();

            //System.Diagnostics.Debug.WriteLine($"MainWindow: Загружено {appeals.Count} обращений и {orders.Count} заказов.");
        }

        private string GetCarInfo(string carId)
        {
            var car = DataStorage.CarData.Cars.FirstOrDefault(c => c.Id == carId);
            return car != null ? $"{car.Brand} {car.Model} ({car.Year})" : "Неизвестный автомобиль";
        }

        private void EditAppealButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Deal deal)
            {
                var editWindow = new AddEditDealWindow(DataStorage.DealData, _clientStorage, _employeeStorage, deal);
                editWindow.Owner = this;
                if (editWindow.ShowDialog() == true)
                {
                    LoadData();
                    AppealsListView.Items.Refresh();
                }
            }
        }

        private void EditOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Deal deal)
            {
                var editWindow = new AddEditDealWindow(DataStorage.DealData, _clientStorage, _employeeStorage, deal);
                editWindow.Owner = this;
                if (editWindow.ShowDialog() == true)
                {
                    LoadData();
                    OrdersListView.Items.Refresh();
                }
            }
        }

        private void ViewAppealDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Deal deal)
            {
                string details = $"Обращение #{deal.Id}\n" +
                                $"Тема: {deal.Theme}\n" +
                                $"Клиент: {deal.ClientName}\n" +
                                $"Телефон: {deal.ClientPhone}\n" +
                                $"Email: {deal.ClientEmail}\n" +
                                $"Дата: {deal.Date}\n" +
                                $"Статус: {deal.Status}\n" +
                                $"Комментарии: {deal.Notes}";
                MessageBox.Show(details, "Подробности обращения");
            }
        }

        private void ViewOrderDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Deal deal)
            {
                string deliveryInfo = deal.IsDeliveryRequired
                    ? $"\nТребуется доставка: Да\nДата доставки: {deal.DeliveryDate}\nАдрес доставки: {deal.DeliveryAddress}"
                    : "\nТребуется доставка: Нет";
                string details = $"Заказ #{deal.Id}\n" +
                                $"Машина: {GetCarInfo(deal.CarId)}\n" +
                                $"Клиент: {deal.ClientName}\n" +
                                $"Телефон: {deal.ClientPhone}\n" +
                                $"Email: {deal.ClientEmail}\n" +
                                $"Дата: {deal.Date}\n" +
                                $"Цена: {deal.Amount}\n" +
                                $"Условия оплаты: {deal.PaymentTerms}\n" +
                                $"Статус: {deal.Status}\n" +
                                $"Комментарии: {deal.Notes}" +
                                deliveryInfo;
                MessageBox.Show(details, "Подробности заказа");
            }
        }



        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            DataStorage.SaveDeals();
            DataStorage.SaveCars();
            _clientStorage.Save();
            _employeeStorage.Save();
            base.OnClosing(e);
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