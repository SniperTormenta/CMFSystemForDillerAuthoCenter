using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.Win32;
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

using OpenXmlParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using OpenXmlRun = DocumentFormat.OpenXml.Wordprocessing.Run;
using OpenXmlBold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using OpenXmlText = DocumentFormat.OpenXml.Wordprocessing.Text;


namespace CMFSystemForDillerAuthoCenter.CallWindow
{
    public partial class CreateSaleContractWindow : Window
    {
        private DealData _dealData;
        private CarData _carData;
        private ClientStorage _clientStorage;
        private Deal _selectedDeal;
        private Client _selectedClient;

        public CreateSaleContractWindow(DealData dealData, CarData carData, ClientStorage clientStorage)
        {
            InitializeComponent();
            _dealData = dealData;
            _carData = carData;
            _clientStorage = clientStorage;
            InitializeDealComboBox();
        }

        private void InitializeDealComboBox()
        {
            var orders = _dealData.Deals
                .Where(d => d.Type == "Заказ" && d.OrderStatus == "Закрыт")
                .ToList();
            DealComboBox.ItemsSource = orders;
            DealComboBox.DisplayMemberPath = "DisplayInfo";
            if (orders.Any())
            {
                DealComboBox.SelectedIndex = 0;
            }
        }

        private void DealComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDeal = DealComboBox.SelectedItem as Deal;
            if (_selectedDeal != null)
            {
                _selectedClient = _clientStorage.Clients.FirstOrDefault(c => c.Id == _selectedDeal.ClientId);
                if (_selectedClient != null)
                {
                    BuyerLivingAddressTextBox.Text = _selectedClient.LivingAddress ?? _selectedDeal.DeliveryAddress ?? "Не указан";
                    BuyerRegisteredAddressTextBox.Text = _selectedClient.RegisteredAddress ?? _selectedDeal.DeliveryAddress ?? "Не указан";
                    BuyerPassportTextBox.Text = _selectedClient.PassportDetails ?? "Серия АА № 1234567, выдан 01.01.2000";
                }
                else
                {
                    BuyerLivingAddressTextBox.Text = _selectedDeal.DeliveryAddress ?? "Не указан";
                    BuyerRegisteredAddressTextBox.Text = _selectedDeal.DeliveryAddress ?? "Не указан";
                    BuyerPassportTextBox.Text = "Серия АА № 1234567, выдан 01.01.2000";
                }

                var car = _carData.Cars.FirstOrDefault(c => c.Id == _selectedDeal.CarId);
                if (car != null)
                {
                    EngineNumberTextBox.Text = car.EngineNumber ?? "Не указан";
                    ChassisNumberTextBox.Text = car.ChassisNumber ?? "Не указан";
                    MileageTextBox.Text = car.Mileage ?? "0 км";
                }
            }
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (_selectedDeal == null) return;

            var car = _carData.Cars.FirstOrDefault(c => c.Id == _selectedDeal.CarId);
            var sb = new StringBuilder();
            sb.AppendLine($"Дата: {DateTime.Now:dd.MM.yyyy}");
            sb.AppendLine($"Место заключения: г. Москва");
            sb.AppendLine($"Продавец: {SellerNameTextBox.Text}");
            sb.AppendLine($"Адрес продавца: {SellerAddressTextBox.Text}");
            sb.AppendLine($"Паспорт/ИНН продавца: {SellerPassportTextBox.Text}");
            sb.AppendLine($"Покупатель: {_selectedDeal.ClientName}");
            sb.AppendLine($"Адрес проживания покупателя: {BuyerLivingAddressTextBox.Text}");
            sb.AppendLine($"Адрес регистрации покупателя: {BuyerRegisteredAddressTextBox.Text}");
            sb.AppendLine($"Паспорт покупателя: {BuyerPassportTextBox.Text}");
            sb.AppendLine($"Автомобиль: {(car != null ? $"{car.Brand} {car.Model} ({car.Year})" : "Не указан")}");
            sb.AppendLine($"VIN: {(car?.Vin ?? "Не указан")}");
            sb.AppendLine($"№ двигателя: {EngineNumberTextBox.Text}");
            sb.AppendLine($"№ шасси: {ChassisNumberTextBox.Text}");
            sb.AppendLine($"№ кузова: {(car?.BodyNumber ?? "Не указан")}");
            sb.AppendLine($"Цвет: {(car?.Color ?? "Не указан")}");
            sb.AppendLine($"Пробег: {MileageTextBox.Text}");
            sb.AppendLine($"Гос. номер: {(car != null ? $"{car.LicensePlate} {car.LicensePlateRegion}" : "Не указан")}");
            sb.AppendLine($"Свидетельство о регистрации: {(car != null ? $"{car.StsSeries} № {car.StsNumber}" : "Не указан")}");
            sb.AppendLine($"Сумма сделки: {_selectedDeal.Amount} руб.");
            PreviewTextBox.Text = sb.ToString();
        }

        private void GenerateContractButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDeal == null)
            {
                MessageBox.Show("Пожалуйста, выберите заказ.");
                return;
            }

            if (string.IsNullOrWhiteSpace(SellerNameTextBox.Text) || string.IsNullOrWhiteSpace(SellerAddressTextBox.Text) ||
                string.IsNullOrWhiteSpace(SellerPassportTextBox.Text) || string.IsNullOrWhiteSpace(BuyerLivingAddressTextBox.Text) ||
                string.IsNullOrWhiteSpace(BuyerRegisteredAddressTextBox.Text) || string.IsNullOrWhiteSpace(BuyerPassportTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Word files (*.docx)|*.docx",
                DefaultExt = "docx",
                FileName = $"Договор_купли-продажи_{_selectedDeal.Id}_{DateTime.Now:yyyyMMdd}.docx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                GenerateContract(saveFileDialog.FileName);
                MessageBox.Show($"Договор успешно сохранен: {saveFileDialog.FileName}");
                DialogResult = true;
                Close();
            }
        }

        private void GenerateContract(string filePath)
        {
            var car = _carData.Cars.FirstOrDefault(c => c.Id == _selectedDeal.CarId);
            if (_selectedDeal == null || car == null)
            {
                MessageBox.Show("Ошибка: Не удалось загрузить данные сделки или автомобиля.");
                return;
            }

            string contractNumber = _selectedDeal.Id;
            string date = DateTime.Now.ToString("dd.MM.yyyy");
            string place = "г. Москва";
            string amountInWords = NumberToWords((long)_selectedDeal.Amount);

            using (WordprocessingDocument doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Заголовок
                AddParagraph(body, "Договор купли-продажи автомобиля", true, 32, JustificationValues.Center);
                AddParagraph(body, $"Договор купли-продажи транспортного средства N {contractNumber}", false, 24, JustificationValues.Center);
                AddParagraph(body, $"«{date}» 20__ года                                      {place}", false, 24, JustificationValues.Left);
                AddParagraph(body, "", false, 24); // Пустая строка

                // Стороны
                AddParagraph(body, $"Мы, гр. {SellerNameTextBox.Text}, проживающий (ая) по адресу {SellerAddressTextBox.Text},", false, 24);
                AddParagraph(body, $"зарегистрированный (ая) по адресу {SellerAddressTextBox.Text},", false, 24);
                AddParagraph(body, $"Удостоверение личности: {SellerPassportTextBox.Text},", false, 24);
                AddParagraph(body, $"именуемый (ая) в дальнейшем «Продавец»,", false, 24);
                AddParagraph(body, $"и гр. {_selectedDeal.ClientName}, проживающий (ая) по адресу {BuyerLivingAddressTextBox.Text},", false, 24);
                AddParagraph(body, $"зарегистрированный (ая) по адресу {BuyerRegisteredAddressTextBox.Text},", false, 24);
                AddParagraph(body, $"Удостоверение личности: {BuyerPassportTextBox.Text},", false, 24);
                AddParagraph(body, $"именуемый (ая) в дальнейшем «Покупатель», заключили настоящий договор о нижеследующем:", false, 24);
                AddParagraph(body, "", false, 24); // Пустая строка

                // Пункт 1: Предмет договора
                AddParagraph(body, "1. Продавец передает в собственность покупателя (продает), а Покупатель принимает (покупает) и оплачивает транспортное средство:", false, 24);
                AddParagraph(body, $"Марка, модель ТС: {car.Brand} {car.Model}", false, 24);
                AddParagraph(body, $"Идентификационный номер (VIN): {car.Vin ?? "Не указан"}", false, 24);
                AddParagraph(body, $"Год выпуска: {car.Year}", false, 24);
                AddParagraph(body, $"№ двигателя: {EngineNumberTextBox.Text}", false, 24);
                AddParagraph(body, $"№ шасси (рамы): {ChassisNumberTextBox.Text}", false, 24);
                AddParagraph(body, $"№ кузова: {car.BodyNumber ?? "Не указан"}", false, 24);
                AddParagraph(body, $"Цвет: {car.Color ?? "Не указан"}", false, 24);
                AddParagraph(body, $"Пробег: {MileageTextBox.Text}", false, 24);
                AddParagraph(body, $"Государственный регистрационный знак: {car.LicensePlate} {car.LicensePlateRegion}", false, 24);
                AddParagraph(body, $"Свидетельство о регистрации ТС: {car.StsSeries} № {car.StsNumber}", false, 24);
                AddParagraph(body, $"Выдано: {place}", false, 24);
                AddParagraph(body, "", false, 24); // Пустая строка

                // Пункт 2: Подтверждение права собственности
                AddParagraph(body, $"2. Указанное в п. 1 транспортное средство, принадлежит Продавцу на праве собственности, что подтверждает паспорт транспортного средства, серии {(car.PtsSeries ?? "Не указан")} №{(car.PtsNumber ?? "Не указан")}, выданный {place}, «{date}»", false, 24);
                AddParagraph(body, "", false, 24); // Пустая строка

                // Пункт 3: Заявление продавца
                AddParagraph(body, "3. Со слов Продавца отчуждаемое транспортное средство никому не продано, не заложено, в споре и под запрещением (арестом) не состоит.", false, 24);
                AddParagraph(body, "", false, 24); // Пустая строка

                // Пункт 4: Стоимость
                AddParagraph(body, $"4. Стоимость указанного в п. 1 транспортного средства согласована Покупателем и Продавцом и составляет: {_selectedDeal.Amount} ({amountInWords} руб. 00 коп.)", false, 24);
                AddParagraph(body, "", false, 24); // Пустая строка

                // Пункт 5: Расчеты
                AddParagraph(body, $"5. Покупатель в оплату за приобретенное транспортное средство передал Продавцу, а Продавец получил денежные средства {_selectedDeal.Amount} ({amountInWords} руб. 00 коп.)", false, 24);
                AddParagraph(body, "", false, 24); // Пустая строка

                // Пункт 6: Переход права собственности
                AddParagraph(body, "6. Право собственности на транспортное средство, указанное в п. 1 договора переходит к Покупателю с момента подписания настоящего договора.", false, 24);
                AddParagraph(body, "", false, 24); // Пустая строка

                // Пункт 7: Количество экземпляров
                AddParagraph(body, "7. Настоящий договор составлен в трех экземплярах (по одному каждой из сторон и один для оформления в ГИБДД).", false, 24);
                AddParagraph(body, "", false, 24); // Пустая строка

                // Подписи
                AddParagraph(body, "Продавец                       Покупатель", true, 24);
                AddParagraph(body, "Деньги получил, транспортное средство передал.          Деньги передал, транспортное средство получил.", false, 24);
                AddParagraph(body, "_____________________________________          ________________________________________", false, 24);
                AddParagraph(body, $"(подпись и ФИО: {SellerNameTextBox.Text})                    (подпись и ФИО: {_selectedDeal.ClientName})", false, 24);
                AddParagraph(body, $"Тел. {_selectedDeal.ClientPhone ?? "Не указан"}                            Тел. {_selectedDeal.ClientPhone ?? "Не указан"}", false, 24);
            }
        }

        private void AddParagraph(Body body, string text, bool bold, int fontSize)
        {
            AddParagraph(body, text, bold, fontSize, JustificationValues.Left);
        }

        private void AddParagraph(Body body, string text, bool bold, int fontSize, JustificationValues justification)
        {
            OpenXmlParagraph para = body.AppendChild(new OpenXmlParagraph());
            OpenXmlRun run = para.AppendChild(new OpenXmlRun());
            run.AppendChild(new OpenXmlText(text));
            para.ParagraphProperties = new ParagraphProperties
            {
                Justification = new Justification { Val = justification },
                SpacingBetweenLines = new SpacingBetweenLines { After = "200" }
            };
            run.RunProperties = new RunProperties
            {
                Bold = bold ? new OpenXmlBold() : null,
                FontSize = new FontSize { Val = fontSize.ToString() }
            };
        }

        private string NumberToWords(long number)
        {
            if (number == 0) return "ноль";

            string[] units = { "", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };
            string[] teens = { "", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать" };
            string[] tens = { "", "десять", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто" };
            string[] hundreds = { "", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот" };
            string[] thousands = { "", "тысяча", "тысячи", "тысяч" };
            string[] millions = { "", "миллион", "миллиона", "миллионов" };

            string result = "";
            int level = 0;

            while (number > 0)
            {
                int part = (int)(number % 1000);
                number /= 1000;

                if (part == 0)
                {
                    level++;
                    continue;
                }

                string partResult = "";
                int hundred = part / 100;
                int ten = (part % 100) / 10;
                int unit = part % 10;

                if (hundred > 0)
                    partResult += hundreds[hundred] + " ";

                if (ten == 1 && unit > 0)
                    partResult += teens[unit] + " ";
                else
                {
                    if (ten > 0)
                        partResult += tens[ten] + " ";
                    if (unit > 0 || part == 0)
                        partResult += units[unit] + " ";
                }

                if (level == 1)
                {
                    if (part == 1)
                        partResult += thousands[1];
                    else if (unit >= 2 && unit <= 4)
                        partResult += thousands[2];
                    else
                        partResult += thousands[3];
                }
                else if (level == 2)
                {
                    if (part == 1)
                        partResult += millions[1];
                    else if (unit >= 2 && unit <= 4)
                        partResult += millions[2];
                    else
                        partResult += millions[3];
                }

                result = partResult + (result.Length > 0 ? " " : "") + result;
                level++;
            }

            return result.Trim();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public static class DealExtensions
    {
        public static string DisplayInfo(this Deal deal)
        {
            return $"{deal.Id}: {deal.ClientName} ({deal.Date})";
        }
    }
}