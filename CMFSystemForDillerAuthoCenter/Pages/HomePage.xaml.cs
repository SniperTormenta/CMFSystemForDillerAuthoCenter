using System;
using System.Windows;
using System.Windows.Controls;

namespace CMFSystemForDillerAuthoCenter.Pages
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            // Обновление текущей даты
            string currentDate = DateTime.Now.ToString("dd MMMM");
            // Здесь нужно найти TextBlock с датой и обновить его
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Сброс всех кнопок
                foreach (var child in ((button.Parent as StackPanel)?.Children ?? new UIElementCollection(null)))
                {
                    if (child is Button filterButton)
                    {
                        filterButton.IsEnabled = true;
                    }
                }

                // Выделение текущей кнопки
                button.IsEnabled = false;

                // Обновление данных в зависимости от фильтра
                UpdateData(button.Content.ToString());
            }
        }

        private void UpdateData(string filter)
        {
            // Здесь будет логика обновления данных в зависимости от выбранного фильтра
            switch (filter)
            {
                case "Сегодня":
                    // Обновить данные за сегодня
                    break;
                case "Неделя":
                    // Обновить данные за неделю
                    break;
                case "Месяц":
                    // Обновить данные за месяц
                    break;
            }
        }
    }
} 