using CMFSystemForDillerAuthoCenter;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

namespace CMFSystemForDillerAuthoCenter.CallWindow
{
    public partial class CreateSaleContractWindow : Window
    {
        private DealData _dealData;
        private CarData _carData;
        private ClientStorage _clientStorage;
        private Deal _selectedDeal;
        private Car _selectedCar;
        private bool _previewTextBoxErrorShown = false;

        public CreateSaleContractWindow(DealData dealData, CarData carData, ClientStorage clientStorage)
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in InitializeComponent: {ex.Message}");
                MessageBox.Show($"Ошибка при инициализации окна: {ex.Message}");
                return;
            }

            if (PreviewTextBox == null && !_previewTextBoxErrorShown)
            {
                System.Diagnostics.Debug.WriteLine("Error: PreviewTextBox is null after InitializeComponent.");
                MessageBox.Show("Ошибка: PreviewTextBox не найден в XAML.");
                _previewTextBoxErrorShown = true;
                return;
            }

            if (ContractDatePicker == null)
            {
                System.Diagnostics.Debug.WriteLine("Error: ContractDatePicker is null after InitializeComponent.");
                MessageBox.Show("Ошибка: ContractDatePicker не найден в XAML.");
                return;
            }

            if (DealComboBox == null)
            {
                System.Diagnostics.Debug.WriteLine("Error: DealComboBox is null after InitializeComponent.");
                MessageBox.Show("Ошибка: DealComboBox не найден в XAML.");
                return;
            }

            _dealData = dealData ?? DataStorage.DealData;
            _carData = carData ?? DataStorage.CarData;
            _clientStorage = clientStorage ?? ClientStorage.Load() ?? new ClientStorage();

            InitializeDealComboBox();
            InitializeCarComboBox();
            UpdatePreview();
        }

        private void InitializeDealComboBox()
        {
            if (_dealData == null || !_dealData.Deals.Any())
            {
                MessageBox.Show("Список заказов пуст. Создайте заказ перед формированием договора.");
                DealComboBox.IsEnabled = false;
                return;
            }

            var deals = _dealData.Deals.Where(d => d.Type == "Заказ").ToList();
            System.Diagnostics.Debug.WriteLine($"Number of deals: {deals.Count}");
            foreach (var deal in deals)
            {
                var car = _carData.Cars.FirstOrDefault(c => c.Id == deal.CarId);
                deal.DisplayInfo = $"{deal.ClientName ?? "Неизвестный клиент"} - {car?.DisplayName ?? "Неизвестный автомобиль"}";
                System.Diagnostics.Debug.WriteLine($"Deal: Id={deal.Id}, ClientId={deal.ClientId}, ClientName={deal.ClientName}, DisplayInfo={deal.DisplayInfo}");
            }
            DealComboBox.ItemsSource = deals;
            DealComboBox.DisplayMemberPath = "DisplayInfo";
            DealComboBox.SelectedIndex = 0;
            DealComboBox_SelectionChanged(null, null);
        }

        private void InitializeCarComboBox()
        {
            if (_carData == null || !_carData.Cars.Any())
            {
                MessageBox.Show("Список автомобилей пуст. Добавьте автомобили перед формированием договора.");
                CarComboBox.IsEnabled = false;
                return;
            }

            CarComboBox.ItemsSource = _carData.Cars;
            CarComboBox.DisplayMemberPath = "DisplayName";
            CarComboBox.SelectedIndex = 0;
            UpdateCarFields();
        }

        private void DealComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDeal = DealComboBox.SelectedItem as Deal;
            System.Diagnostics.Debug.WriteLine($"DealComboBox_SelectionChanged: Selected Deal Id={_selectedDeal?.Id ?? "null"}");

            if (_selectedDeal != null)
            {
                BuyerNameTextBox.Text = _selectedDeal.ClientName ?? "Неизвестный покупатель";
                BuyerLivingAddressTextBox.Text = "";
                BuyerRegisteredAddressTextBox.Text = "";
                BuyerPassportTextBox.Text = "";

                var car = _carData.Cars.FirstOrDefault(c => c.Id == _selectedDeal.CarId);
                if (car != null)
                {
                    CarComboBox.SelectedItem = car;
                }
                System.Diagnostics.Debug.WriteLine($"Selected Deal: Id={_selectedDeal?.Id ?? "null"}, Client: {BuyerNameTextBox.Text}, Car: {car?.DisplayName ?? "null"}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Selected Deal is null.");
                BuyerNameTextBox.Text = "Неизвестный покупатель";
                BuyerLivingAddressTextBox.Text = "";
                BuyerRegisteredAddressTextBox.Text = "";
                BuyerPassportTextBox.Text = "";
            }
            UpdatePreview();
        }

        private void CarComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCar = CarComboBox.SelectedItem as Car;
            UpdateCarFields();
            UpdatePreview();
        }

        private void UpdateCarFields()
        {
            if (_selectedCar != null)
            {
                EngineNumberTextBox.Text = _selectedCar.EngineNumber ?? "";
                VinTextBox.Text = _selectedCar.Vin ?? "";
                MileageTextBox.Text = _selectedCar.Mileage ?? "";
            }
            else
            {
                EngineNumberTextBox.Text = "";
                VinTextBox.Text = "";
                MileageTextBox.Text = "";
            }
        }

        private void UpdatePreview()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Договор купли-продажи автомобиля");

            string contractDate = "не указано";
            if (ContractDatePicker != null)
            {
                contractDate = ContractDatePicker.SelectedDate?.ToString("dd.MM.yyyy") ?? "не указано";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error: ContractDatePicker is null in UpdatePreview.");
            }
            sb.AppendLine($"№ {(ContractNumberTextBox != null ? ContractNumberTextBox.Text : "не указано")} от {contractDate}");

            sb.AppendLine();
            sb.AppendLine($"Продавец: ООО 'АвтоСалон Чебурнет'");
            sb.AppendLine($"Адрес: {(SellerAddressTextBox != null ? SellerAddressTextBox.Text : "не указано")}");
            sb.AppendLine($"ИНН: {(SellerInnTextBox != null ? SellerInnTextBox.Text : "не указано")}");
            sb.AppendLine();
            sb.AppendLine($"Покупатель: {(BuyerNameTextBox != null ? BuyerNameTextBox.Text : "не указано")}");
            sb.AppendLine($"Адрес проживания: {(BuyerLivingAddressTextBox != null ? BuyerLivingAddressTextBox.Text : "не указано")}");
            sb.AppendLine($"Адрес регистрации: {(BuyerRegisteredAddressTextBox != null ? BuyerRegisteredAddressTextBox.Text : "не указано")}");
            sb.AppendLine($"Паспортные данные: {(BuyerPassportTextBox != null ? BuyerPassportTextBox.Text : "не указано")}");
            sb.AppendLine();
            sb.AppendLine($"Автомобиль: {(_selectedCar != null ? _selectedCar.DisplayName : "не выбран")} (VIN: {_selectedCar?.Vin ?? "не указан"})");
            sb.AppendLine($"№ двигателя: {(EngineNumberTextBox != null ? EngineNumberTextBox.Text : "не указано")}");
            sb.AppendLine($"Пробег: {(MileageTextBox != null ? MileageTextBox.Text : "не указано")}");
            sb.AppendLine();
            sb.AppendLine($"Продавец: ООО 'АвтоСалон Чебурнет' ___________________________");
            sb.AppendLine($"Покупатель: {(BuyerSignatureTextBox != null ? BuyerSignatureTextBox.Text : "не указано")}");

            if (PreviewTextBox != null)
            {
                PreviewTextBox.Text = sb.ToString();
            }
            else if (!_previewTextBoxErrorShown)
            {
                System.Diagnostics.Debug.WriteLine("Error: PreviewTextBox is null in UpdatePreview.");
                MessageBox.Show("Ошибка: PreviewTextBox не найден в XAML.");
                _previewTextBoxErrorShown = true;
            }
        }

        private void GenerateContractButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDeal == null || _selectedCar == null)
            {
                MessageBox.Show("Выберите заказ и автомобиль.");
                return;
            }

            if (string.IsNullOrWhiteSpace(BuyerNameTextBox?.Text) ||
                string.IsNullOrWhiteSpace(BuyerLivingAddressTextBox?.Text) ||
                string.IsNullOrWhiteSpace(BuyerRegisteredAddressTextBox?.Text) ||
                string.IsNullOrWhiteSpace(BuyerPassportTextBox?.Text) ||
                string.IsNullOrWhiteSpace(EngineNumberTextBox?.Text) ||
                string.IsNullOrWhiteSpace(MileageTextBox?.Text))
            {
                MessageBox.Show("Заполните все обязательные поля.");
                return;
            }

            // Сохранение договора в Word-документ на рабочий стол
            try
            {
                // Получаем путь к рабочему столу
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                // Формируем имя файла с текущей датой и временем
                string fileName = $"Contract_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                string filePath = Path.Combine(desktopPath, fileName);

                // Создаем Word-документ
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
                {
                    // Добавляем основную часть документа
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Разбиваем текст из PreviewTextBox на строки
                    string[] lines = PreviewTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    // Добавляем каждую строку как отдельный абзац
                    foreach (string line in lines)
                    {
                        Paragraph para = body.AppendChild(new Paragraph());
                        Run run = para.AppendChild(new Run());
                        run.AppendChild(new Text(line));
                    }

                    // Сохраняем документ
                    wordDocument.Save();
                }

                MessageBox.Show($"Договор успешно сохранён на рабочий стол: {fileName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving Word document: {ex.Message}");
                MessageBox.Show($"Ошибка при сохранении договора: {ex.Message}");
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void ContractDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview();
        }
    }
}