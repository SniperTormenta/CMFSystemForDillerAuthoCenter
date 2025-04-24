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
    public partial class NewDealsVariant1 : UserControl
    {
        private DealData dealData;
        private CarData carData;
        private Action saveAction;

        public NewDealsVariant1(DealData dealData, CarData carData, Action saveAction)
        {
            InitializeComponent();

            // Проверяем, что dealData не null
            if (dealData == null)
            {
                MessageBox.Show("Ошибка: DealData не инициализирован.");
                this.dealData = new DealData();
            }
            else
            {
                this.dealData = dealData;
            }

            // Убедимся, что Deals инициализирован
            if (this.dealData.Deals == null)
            {
                this.dealData.Deals = new System.Collections.Generic.List<Deal>();
            }

            // Проверяем carData
            this.carData = carData;
            MessageBox.Show($"NewDealsVariant1: carData содержит {carData?.Cars?.Count ?? 0} автомобилей.");

            this.saveAction = saveAction;
            InitializeDataGrids();
            InitializeFilters();
        }

        private void InitializeDataGrids()
        {
            AppealsDataGrid.ItemsSource = dealData.Deals.Where(d => d.Type == "Обращение").ToList();
            OrdersDataGrid.ItemsSource = dealData.Deals.Where(d => d.Type == "Заказ").ToList();
        }

        private void InitializeFilters()
        {
            TypeFilterComboBox.SelectionChanged += FilterDeals;
            StatusFilterComboBox.SelectionChanged += FilterDeals;
            SearchTextBox.TextChanged += FilterDeals;

            var statuses = dealData.Deals.Select(d => d.Status).Distinct().ToList();
            foreach (var status in statuses)
            {
                if (!string.IsNullOrEmpty(status))
                {
                    StatusFilterComboBox.Items.Add(new ComboBoxItem { Content = status });
                }
            }
        }

        private void FilterDeals(object sender, EventArgs e)
        {
            var typeFilter = (TypeFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            var statusFilter = (StatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            var searchText = SearchTextBox.Text.ToLower();

            var filteredAppeals = dealData.Deals.Where(d =>
                d.Type == "Обращение" &&
                (typeFilter == "Все" || typeFilter == "Обращение") &&
                (statusFilter == "Все" || d.Status == statusFilter) &&
                (string.IsNullOrEmpty(searchText) ||
                 (d.ClientName != null && d.ClientName.ToLower().Contains(searchText)) ||
                 (d.Id != null && d.Id.ToLower().Contains(searchText))))
                .ToList();

            AppealsDataGrid.ItemsSource = filteredAppeals;

            var filteredOrders = dealData.Deals.Where(d =>
                d.Type == "Заказ" &&
                (typeFilter == "Все" || typeFilter == "Заказ") &&
                (statusFilter == "Все" || d.Status == statusFilter) &&
                (string.IsNullOrEmpty(searchText) ||
                 (d.ClientName != null && d.ClientName.ToLower().Contains(searchText)) ||
                 (d.Id != null && d.Id.ToLower().Contains(searchText))))
                .ToList();

            OrdersDataGrid.ItemsSource = filteredOrders;
        }

        private void AddDealButton_Click(object sender, RoutedEventArgs e)
        {
            var addDealWindow = new AddEditDealWindow(dealData, carData);
            addDealWindow.Owner = Window.GetWindow(this);
            if (addDealWindow.ShowDialog() == true)
            {
                InitializeDataGrids();
                saveAction?.Invoke();
            }
        }

        private void AppealsDataGrid_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (AppealsDataGrid.SelectedItem is Deal selectedDeal)
            {
                var editDealWindow = new AddEditDealWindow(dealData, carData, selectedDeal);
                editDealWindow.Owner = Window.GetWindow(this);
                editDealWindow.Title = "Редактировать обращение";
                if (editDealWindow.ShowDialog() == true)
                {
                    InitializeDataGrids();
                    saveAction?.Invoke();
                }
            }
        }

        private void OrdersDataGrid_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Deal selectedDeal)
            {
                var editDealWindow = new AddEditDealWindow(dealData, carData, selectedDeal);
                editDealWindow.Owner = Window.GetWindow(this);
                editDealWindow.Title = "Редактировать заказ";
                if (editDealWindow.ShowDialog() == true)
                {
                    InitializeDataGrids();
                    saveAction?.Invoke();
                }
            }
        }
    }
}