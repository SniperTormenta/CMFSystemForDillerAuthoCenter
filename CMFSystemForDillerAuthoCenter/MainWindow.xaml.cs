using CMFSystemForDillerAuthoCenter.Windows;
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


namespace CMFSystemForDillerAuthoCenter
{
    public partial class MainWindow : Window
    {
        private OrderData orderData;
        private const string JsonFilePath = "orders.json";

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            DataContext = orderData;
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(JsonFilePath))
                {
                    string json = File.ReadAllText(JsonFilePath);
                    orderData = JsonConvert.DeserializeObject<OrderData>(json);
                }
                else
                {
                    orderData = new OrderData
                    {
                        Orders = new List<Order>
                        {
                            new Order
                            {
                                Id = "C2-002345 29.01.2023 06:32",
                                Title = "LADA VESTA CROSS WAG...",
                                Status = "Тест-драйв, ООГ",
                                Date = new DateTime(2023, 1, 29, 6, 32, 0),
                                CarModel = "HYUNDAI CRETA II",
                                Price = 2895056,
                                ClientPhone = "+7 (912) 345 67 89",
                                ClientName = "Иванов Иван Иванович",
                                Source = "SMS",
                            },
                            new Order
                            {
                                Id = "C3-002346 17.01.2023 14:02",
                                Title = "SKODA OCTAVIA",
                                Status = "В работе Сергеева",
                                Date = new DateTime(2023, 1, 17, 14, 2, 0),
                                CarModel = "KIA SPORTAGE",
                                Price = 580000,
                                ClientPhone = "+7 (900) 195 48 82",
                                ClientName = "Петров Петр Петрович",
                                Source = "Звонок",
                            },
                            new Order
                            {
                                Id = "C2-002319 18.01.2023 19:00",
                                Title = "NISSAN X-TRAIL",
                                Status = "Тест-драйв, ООГ",
                                Date = new DateTime(2023, 1, 18, 19, 0, 0),
                                CarModel = "MERCEDES-BENZ B180",
                                Price = 5581056,
                                ClientPhone = "+7 (912) 345 67 89",
                                ClientName = "Сидоров Сидор Сидорович",
                                Source = "SMS",
                            },
                            new Order
                            {
                                Id = "C3-002326 28.01.2023 12:45",
                                Title = "NISSAN QASHQAI",
                                Status = "Кредитование, ИП",
                                Date = new DateTime(2023, 1, 28, 12, 45, 0),
                                CarModel = "TOYOTA CAMRY",
                                Price = 4037757,
                                ClientPhone = "+7 (912) 345 67 89",
                                ClientName = "Иванов Иван Иванович",
                                Source = "KM",
                            },
                            new Order
                            {
                                Id = "C3-002349 22.01.2023 14:15",
                                Title = "MERCEDES-BENZ B180",
                                Status = "Интерес, уточнял по наличию и комплектации",
                                Date = new DateTime(2023, 1, 22, 14, 15, 0),
                                CarModel = "LADA LARGUS",
                                Price = 206500,
                                ClientPhone = "+7 (906) 927-77-12",
                                ClientName = "Михаил Дмитриев Николаевич, ИП",
                                Source = "KM",
                            }
                        }
                    };
                    SaveData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
                orderData = new OrderData();
            }
        }

        private void SaveData()
        {
            try
            {
                string json = JsonConvert.SerializeObject(orderData, Formatting.Indented);
                File.WriteAllText(JsonFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveData();
            base.OnClosing(e);
        }

        private void SkadButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WarehouseWindow warehouseWindow = new WarehouseWindow();
            warehouseWindow.Show();
            this.Close();
        }

        private void NewDealsButton_Click(object sender, RoutedEventArgs e)
        {
            var carData = new CarData(); // Загрузите из файла или передайте из другого окна
            var newDealsWindow = new NewDealsWindow(carData);
            newDealsWindow.Show();
        }
    }
}