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

    public partial class NewDealsVariant2 : UserControl
{
    private DealData dealData;
    private CarData carData;
    private Action saveAction;

    public NewDealsVariant2(DealData dealData, CarData carData, Action saveAction)
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
        MessageBox.Show($"NewDealsVariant2: carData содержит {carData?.Cars?.Count ?? 0} автомобилей.");

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
        AppealStatusFilterComboBox.SelectionChanged += FilterAppeals;
        AppealSearchTextBox.TextChanged += FilterAppeals;
        OrderStatusFilterComboBox.SelectionChanged += FilterOrders;
        OrderSearchTextBox.TextChanged += FilterOrders;

        var appealStatuses = dealData.Deals.Where(d => d.Type == "Обращение")
                                          .Select(d => d.Status)
                                          .Distinct()
                                          .ToList();
        foreach (var status in appealStatuses)
        {
            if (!string.IsNullOrEmpty(status))
            {
                AppealStatusFilterComboBox.Items.Add(new ComboBoxItem { Content = status });
            }
        }

        var orderStatuses = dealData.Deals.Where(d => d.Type == "Заказ")
                                          .Select(d => d.Status)
                                          .Distinct()
                                          .ToList();
        foreach (var status in orderStatuses)
        {
            if (!string.IsNullOrEmpty(status))
            {
                OrderStatusFilterComboBox.Items.Add(new ComboBoxItem { Content = status });
            }
        }
    }

    private void FilterAppeals(object sender, EventArgs e)
    {
        var statusFilter = (AppealStatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
        var searchText = AppealSearchTextBox.Text.ToLower();

        var filteredAppeals = dealData.Deals.Where(d =>
            d.Type == "Обращение" &&
            (statusFilter == "Все" || d.Status == statusFilter) &&
            (string.IsNullOrEmpty(searchText) ||
             (d.ClientName != null && d.ClientName.ToLower().Contains(searchText)) ||
             (d.Id != null && d.Id.ToLower().Contains(searchText))))
            .ToList();

        AppealsDataGrid.ItemsSource = filteredAppeals;
    }

    private void FilterOrders(object sender, EventArgs e)
    {
        var statusFilter = (OrderStatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
        var searchText = OrderSearchTextBox.Text.ToLower();

        var filteredOrders = dealData.Deals.Where(d =>
            d.Type == "Заказ" &&
            (statusFilter == "Все" || d.Status == statusFilter) &&
            (string.IsNullOrEmpty(searchText) ||
             (d.ClientName != null && d.ClientName.ToLower().Contains(searchText)) ||
             (d.Id != null && d.Id.ToLower().Contains(searchText))))
            .ToList();

        OrdersDataGrid.ItemsSource = filteredOrders;
    }

    private void AddAppealButton_Click(object sender, RoutedEventArgs e)
    {
        var addDealWindow = new AddEditDealWindow(dealData, carData, null, "Обращение");
        addDealWindow.Owner = Window.GetWindow(this);
        if (addDealWindow.ShowDialog() == true)
        {
            InitializeDataGrids();
            InitializeFilters();
            saveAction?.Invoke();
        }
    }

    private void AddOrderButton_Click(object sender, RoutedEventArgs e)
    {
        var addDealWindow = new AddEditDealWindow(dealData, carData, null, "Заказ");
        addDealWindow.Owner = Window.GetWindow(this);
        if (addDealWindow.ShowDialog() == true)
        {
            InitializeDataGrids();
            InitializeFilters();
            saveAction?.Invoke();
        }
    }

    private void ConvertToOrderButton_Click(object sender, RoutedEventArgs e)
    {
        if (AppealsDataGrid.SelectedItem is Deal selectedAppeal)
        {
            var newOrder = new Deal
            {
                Id = $"D{dealData.Deals.Count + 1:D03}",
                Type = "Заказ",
                ClientName = selectedAppeal.ClientName,
                ClientPhone = selectedAppeal.ClientPhone,
                ClientEmail = selectedAppeal.ClientEmail,
                Date = selectedAppeal.Date,
                Status = "Подтвержден"
            };

            var addOrderWindow = new AddEditDealWindow(dealData, carData, newOrder, "Заказ");
            addOrderWindow.Owner = Window.GetWindow(this);
            addOrderWindow.Title = "Создать заказ из обращения";
            if (addOrderWindow.ShowDialog() == true)
            {
                dealData.Deals.Remove(selectedAppeal);
                InitializeDataGrids();
                InitializeFilters();
                saveAction?.Invoke();
            }
        }
        else
        {
            MessageBox.Show("Пожалуйста, выберите обращение для преобразования в заказ.");
        }
    }

    private void AppealsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (AppealsDataGrid.SelectedItem is Deal selectedDeal)
        {
            var editDealWindow = new AddEditDealWindow(dealData, carData, selectedDeal);
            editDealWindow.Owner = Window.GetWindow(this);
            editDealWindow.Title = "Редактировать обращение";
            if (editDealWindow.ShowDialog() == true)
            {
                InitializeDataGrids();
                InitializeFilters();
                saveAction?.Invoke();
            }
        }
    }

    private void OrdersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (OrdersDataGrid.SelectedItem is Deal selectedDeal)
        {
            var editDealWindow = new AddEditDealWindow(dealData, carData, selectedDeal);
            editDealWindow.Owner = Window.GetWindow(this);
            editDealWindow.Title = "Редактировать заказ";
            if (editDealWindow.ShowDialog() == true)
            {
                InitializeDataGrids();
                InitializeFilters();
                saveAction?.Invoke();
            }
        }
    }
}
}