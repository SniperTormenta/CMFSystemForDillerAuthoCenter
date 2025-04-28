using CMFSystemForDillerAuthoCenter;
using CMFSystemForDillerAuthoCenter.Services;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CMFSystemForDillerAuthoCenter.CallWindow;


namespace CMFSystemForDillerAuthoCenter.Windows
{
    public partial class ReportWindow : Window
    {
        private DealData _dealData;
        private ClientStorage _clientStorage;
        private EmployeeStorage _employeeStorage;
        private EmailService _emailService;

        public ReportWindow(DealData dealData, ClientStorage clientStorage, EmployeeStorage employeeStorage, EmailService emailService)
        {
            InitializeComponent();
            _dealData = dealData;
            _clientStorage = clientStorage;
            _employeeStorage = employeeStorage;
            _emailService = emailService;
        }


        private void PeriodRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Логика обновления DatePicker при необходимости
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = FormatComboBox.SelectedItem.ToString().Contains("Excel") ? "Excel files (*.xlsx)|*.xlsx" : "Word files (*.docx)|*.docx",
                DefaultExt = FormatComboBox.SelectedItem.ToString().Contains("Excel") ? "xlsx" : "docx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                GenerateReport(filePath);
                MessageBox.Show($"Отчет успешно сохранен: {filePath}");
            }
        }

        private void SendEmailButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            // Создаем временный файл для отчета
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"Report_{DateTime.Now:yyyyMMddHHmmss}.{(FormatComboBox.SelectedItem.ToString().Contains("Excel") ? "xlsx" : "docx")}");
            GenerateReport(tempFilePath);

            // Открываем окно отправки email
            var composeWindow = new ComposeEmailWindow(_emailService)
            {
                Owner = this,
                ToTextBox = { Text = "recipient@example.com" },
                SubjectTextBox = { Text = $"Отчет за {PeriodDatePicker.SelectedDate?.ToString("MMMM yyyy")}" },
                BodyTextBox = { Text = "В приложении находится сгенерированный отчет." }
            };
            composeWindow.AddAttachment(tempFilePath); // Используем публичный метод

            if (composeWindow.ShowDialog() == true)
            {
                MessageBox.Show("Отчет успешно отправлен по email.");
            }

            // Удаляем временный файл после отправки
            try
            {
                if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении временного файла: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            if (PeriodDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите период.");
                return false;
            }

            if (!DealsCheckBox.IsChecked.Value && !ClientsCheckBox.IsChecked.Value && !FinancesCheckBox.IsChecked.Value)
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы один тип данных для включения в отчет.");
                return false;
            }

            return true;
        }

        private void GenerateReport(string filePath)
        {
            DateTime selectedDate = PeriodDatePicker.SelectedDate.Value;
            bool isWeekly = WeeklyRadioButton.IsChecked == true;

            // Определяем начало и конец периода
            DateTime startDate, endDate;
            if (isWeekly)
            {
                int daysUntilMonday = ((int)selectedDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                startDate = selectedDate.AddDays(-daysUntilMonday);
                endDate = startDate.AddDays(6);
            }
            else
            {
                startDate = new DateTime(selectedDate.Year, selectedDate.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }

            // Фильтруем сделки и клиентов за период
            var dealsInPeriod = _dealData.Deals
                .Where(d => DateTime.TryParse(d.Date, out DateTime dealDate) && dealDate >= startDate && dealDate <= endDate)
                .ToList();
            var clientsInPeriod = _clientStorage.Clients
                .Where(c => dealsInPeriod.Any(d => d.ClientId == c.Id))
                .ToList();

            // Формируем отчет в зависимости от формата
            if (FormatComboBox.SelectedItem.ToString().Contains("Excel"))
            {
                GenerateExcelReport(filePath, dealsInPeriod, clientsInPeriod, startDate, endDate);
            }
            else
            {
                GenerateWordReport(filePath, dealsInPeriod, clientsInPeriod, startDate, endDate);
            }
        }

        private void GenerateExcelReport(string filePath, List<Deal> deals, List<Client> clients, DateTime startDate, DateTime endDate)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Отчет");

                int currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = $"Отчет за период с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}";
                currentRow += 2;

                // Сделки
                if (DealsCheckBox.IsChecked == true && deals.Any())
                {
                    worksheet.Cell(currentRow, 1).Value = "Сделки";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "Дата";
                    worksheet.Cell(currentRow, 2).Value = "Тип сделки";
                    worksheet.Cell(currentRow, 3).Value = "Клиент";
                    worksheet.Cell(currentRow, 4).Value = "Сумма";
                    worksheet.Cell(currentRow, 5).Value = "Сотрудник";
                    worksheet.Row(currentRow).Style.Font.Bold = true;
                    currentRow++;

                    foreach (var deal in deals)
                    {
                        var employee = _employeeStorage.Employees.FirstOrDefault(e => e.Id == deal.ServicedBy);
                        worksheet.Cell(currentRow, 1).Value = deal.Date;
                        worksheet.Cell(currentRow, 2).Value = deal.Type;
                        worksheet.Cell(currentRow, 3).Value = deal.ClientName;
                        worksheet.Cell(currentRow, 4).Value = deal.Amount;
                        worksheet.Cell(currentRow, 5).Value = employee?.FullName ?? "Не указан";
                        currentRow++;
                    }
                    currentRow++;
                }

                // Клиенты
                if (ClientsCheckBox.IsChecked == true && clients.Any())
                {
                    worksheet.Cell(currentRow, 1).Value = "Клиенты";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "ФИО";
                    worksheet.Cell(currentRow, 2).Value = "Телефон";
                    worksheet.Cell(currentRow, 3).Value = "Email";
                    worksheet.Row(currentRow).Style.Font.Bold = true;
                    currentRow++;

                    foreach (var client in clients)
                    {
                        worksheet.Cell(currentRow, 1).Value = client.ClientName;
                        worksheet.Cell(currentRow, 2).Value = client.Phone;
                        worksheet.Cell(currentRow, 3).Value = client.Email;
                        currentRow++;
                    }
                    currentRow++;
                }

                // Финансы
                if (FinancesCheckBox.IsChecked == true && deals.Any())
                {
                    worksheet.Cell(currentRow, 1).Value = "Финансовые показатели";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    currentRow++;

                    var totalRevenue = deals.Where(d => d.Type == "Заказ").Sum(d => d.Amount);
                    var averageDeal = deals.Any(d => d.Type == "Заказ") ? totalRevenue / deals.Count(d => d.Type == "Заказ") : 0;

                    worksheet.Cell(currentRow, 1).Value = "Общая выручка";
                    worksheet.Cell(currentRow, 2).Value = totalRevenue;
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "Средний чек";
                    worksheet.Cell(currentRow, 2).Value = averageDeal;
                    currentRow++;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(filePath);
            }
        }

        private void GenerateWordReport(string filePath, List<Deal> deals, List<Client> clients, DateTime startDate, DateTime endDate)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Заголовок
                Paragraph titlePara = body.AppendChild(new Paragraph());
                Run titleRun = titlePara.AppendChild(new Run());
                titleRun.AppendChild(new Text($"Отчет за период с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}"));
                titlePara.ParagraphProperties = new ParagraphProperties
                {
                    Justification = new Justification { Val = JustificationValues.Center },
                    SpacingBetweenLines = new SpacingBetweenLines { After = "200" }
                };
                titleRun.RunProperties = new RunProperties { Bold = new Bold(), FontSize = new FontSize { Val = "32" } };

                // Сделки
                if (DealsCheckBox.IsChecked == true && deals.Any())
                {
                    Paragraph dealsHeader = body.AppendChild(new Paragraph());
                    Run dealsHeaderRun = dealsHeader.AppendChild(new Run());
                    dealsHeaderRun.AppendChild(new Text("Сделки"));
                    dealsHeaderRun.RunProperties = new RunProperties { Bold = new Bold(), FontSize = new FontSize { Val = "24" } };

                    Table dealsTable = body.AppendChild(new Table());
                    TableProperties dealsTableProps = dealsTable.AppendChild(new TableProperties(
                        new TableBorders(
                            new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 }
                        )
                    ));
                    TableRow headerRow = dealsTable.AppendChild(new TableRow());
                    AddTableCell(headerRow, "Дата");
                    AddTableCell(headerRow, "Тип сделки");
                    AddTableCell(headerRow, "Клиент");
                    AddTableCell(headerRow, "Сумма");
                    AddTableCell(headerRow, "Сотрудник");

                    foreach (var deal in deals)
                    {
                        var employee = _employeeStorage.Employees.FirstOrDefault(e => e.Id == deal.ServicedBy);
                        TableRow row = dealsTable.AppendChild(new TableRow());
                        AddTableCell(row, deal.Date);
                        AddTableCell(row, deal.Type);
                        AddTableCell(row, deal.ClientName);
                        AddTableCell(row, deal.Amount.ToString());
                        AddTableCell(row, employee?.FullName ?? "Не указан");
                    }
                }

                // Клиенты
                if (ClientsCheckBox.IsChecked == true && clients.Any())
                {
                    Paragraph clientsHeader = body.AppendChild(new Paragraph());
                    Run clientsHeaderRun = clientsHeader.AppendChild(new Run());
                    clientsHeaderRun.AppendChild(new Text("Клиенты"));
                    clientsHeaderRun.RunProperties = new RunProperties { Bold = new Bold(), FontSize = new FontSize { Val = "24" } };

                    Table clientsTable = body.AppendChild(new Table());
                    TableProperties clientsTableProps = clientsTable.AppendChild(new TableProperties(
                        new TableBorders(
                            new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 }
                        )
                    ));
                    TableRow clientsHeaderRow = clientsTable.AppendChild(new TableRow());
                    AddTableCell(clientsHeaderRow, "ФИО");
                    AddTableCell(clientsHeaderRow, "Телефон");
                    AddTableCell(clientsHeaderRow, "Email");

                    foreach (var client in clients)
                    {
                        TableRow row = clientsTable.AppendChild(new TableRow());
                        AddTableCell(row, client.ClientName);
                        AddTableCell(row, client.Phone);
                        AddTableCell(row, client.Email);
                    }
                }

                // Финансы
                if (FinancesCheckBox.IsChecked == true && deals.Any())
                {
                    Paragraph financesHeader = body.AppendChild(new Paragraph());
                    Run financesHeaderRun = financesHeader.AppendChild(new Run());
                    financesHeaderRun.AppendChild(new Text("Финансовые показатели"));
                    financesHeaderRun.RunProperties = new RunProperties { Bold = new Bold(), FontSize = new FontSize { Val = "24" } };

                    Table financesTable = body.AppendChild(new Table());
                    TableProperties financesTableProps = financesTable.AppendChild(new TableProperties(
                        new TableBorders(
                            new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                            new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 }
                        )
                    ));
                    TableRow financesHeaderRow = financesTable.AppendChild(new TableRow());
                    AddTableCell(financesHeaderRow, "Показатель");
                    AddTableCell(financesHeaderRow, "Значение");

                    var totalRevenue = deals.Where(d => d.Type == "Заказ").Sum(d => d.Amount);
                    var averageDeal = deals.Any(d => d.Type == "Заказ") ? totalRevenue / deals.Count(d => d.Type == "Заказ") : 0;

                    TableRow revenueRow = financesTable.AppendChild(new TableRow());
                    AddTableCell(revenueRow, "Общая выручка");
                    AddTableCell(revenueRow, totalRevenue.ToString());

                    TableRow avgDealRow = financesTable.AppendChild(new TableRow());
                    AddTableCell(avgDealRow, "Средний чек");
                    AddTableCell(avgDealRow, averageDeal.ToString());
                }
            }
        }

        private void AddTableCell(TableRow row, string text)
        {
            TableCell cell = row.AppendChild(new TableCell());
            cell.AppendChild(new Paragraph(new Run(new Text(text))));
            cell.AppendChild(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = "2400" }));
        }
    }
}