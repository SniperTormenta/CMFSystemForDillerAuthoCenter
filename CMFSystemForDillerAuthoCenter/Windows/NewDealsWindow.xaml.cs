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
    public partial class NewDealsWindow : Window
    {
        private DealData dealData;
        private CarData carData;
        private const string DealsJsonFilePath = "deals.json";
        private UserControl variant1;
        private UserControl variant2;

        public NewDealsWindow(CarData carData)
        {
            InitializeComponent();
            this.carData = carData;
            LoadDeals();
            InitializeVariants();
            SetInitialView();
        }

        public NewDealsWindow()
        {
        }

        private void LoadDeals()
        {
            try
            {
                if (File.Exists(DealsJsonFilePath))
                {
                    string json = File.ReadAllText(DealsJsonFilePath);
                    dealData = JsonConvert.DeserializeObject<DealData>(json);
                }
                else
                {
                    dealData = new DealData();
                    SaveDeals();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке сделок: {ex.Message}");
                dealData = new DealData();
            }
        }

        private void SaveDeals()
        {
            try
            {
                string json = JsonConvert.SerializeObject(dealData, Formatting.Indented);
                File.WriteAllText(DealsJsonFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении сделок: {ex.Message}");
            }
        }

        private void InitializeVariants()
        {
            variant1 = new NewDealsVariant1(dealData, carData, SaveDeals);
            variant2 = new NewDealsVariant2(dealData, carData, SaveDeals);
        }

        private void SetInitialView()
        {
            DealsContent.Content = variant1;
            ViewToggleButton.Content = "Объединенный вид";
        }

        private void ViewToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            DealsContent.Content = variant2;
            ViewToggleButton.Content = "Вкладки";
        }

        private void ViewToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            DealsContent.Content = variant1;
            ViewToggleButton.Content = "Объединенный вид";
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveDeals();
            base.OnClosing(e);
        }
    }
}